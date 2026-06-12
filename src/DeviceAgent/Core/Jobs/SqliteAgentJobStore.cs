using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;

namespace ITSupportNative.DeviceAgent.Jobs;

public sealed class SqliteAgentJobStore
    : IAgentJobStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
        },
    };

    private readonly string _connectionString;

    public SqliteAgentJobStore(string stateFilePath)
    {
        string normalizedPath = NormalizePath(stateFilePath);
        string directory = Path.GetDirectoryName(normalizedPath)
            ?? throw new InvalidOperationException("The state database requires a parent directory.");
        Directory.CreateDirectory(directory);

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = normalizedPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Private,
            Pooling = false,
        }.ToString();
    }

    public async Task<IReadOnlyList<AgentJobRecord>> LoadAsync(
        CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await EnsureSchemaAsync(connection, cancellationToken);

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT payload_json
            FROM agent_jobs
            ORDER BY created_at_utc, job_id;
            """;

        var jobs = new List<AgentJobRecord>();
        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            string payload = reader.GetString(0);
            AgentJobRecord? job = JsonSerializer.Deserialize<AgentJobRecord>(
                payload,
                SerializerOptions);
            if (job is null)
            {
                throw new InvalidDataException("A persisted agent job could not be deserialized.");
            }

            jobs.Add(job);
        }

        return jobs;
    }

    public async Task SaveAsync(
        IReadOnlyList<AgentJobRecord> jobs,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(jobs);

        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await EnsureSchemaAsync(connection, cancellationToken);

        await using SqliteTransaction transaction =
            (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);
        await using (SqliteCommand clear = connection.CreateCommand())
        {
            clear.Transaction = transaction;
            clear.CommandText = "DELETE FROM agent_jobs;";
            await clear.ExecuteNonQueryAsync(cancellationToken);
        }

        foreach (AgentJobRecord job in jobs)
        {
            await using SqliteCommand insert = connection.CreateCommand();
            insert.Transaction = transaction;
            insert.CommandText = """
                INSERT INTO agent_jobs (job_id, created_at_utc, payload_json)
                VALUES ($jobId, $createdAtUtc, $payloadJson);
                """;
            insert.Parameters.AddWithValue("$jobId", job.JobId);
            insert.Parameters.AddWithValue(
                "$createdAtUtc",
                job.CreatedAt.UtcDateTime.ToString("O", System.Globalization.CultureInfo.InvariantCulture));
            insert.Parameters.AddWithValue(
                "$payloadJson",
                JsonSerializer.Serialize(job, SerializerOptions));
            await insert.ExecuteNonQueryAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }

    private static async Task EnsureSchemaAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS agent_jobs (
                job_id TEXT NOT NULL PRIMARY KEY,
                created_at_utc TEXT NOT NULL,
                payload_json TEXT NOT NULL
            );
            """;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static string NormalizePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return Path.GetFullPath(path);
    }
}
