using System.Text;
using System.Text.Json;
using ITSupportNative.Contracts.Conversation;

namespace ITSupportNative.ContractTests;

public sealed class ConversationChannelContractTests
{
    [Fact]
    public void SharedValidFixturesPassTheStrictContract()
    {
        using JsonDocument fixture = LoadFixture();

        foreach (JsonElement testCase in fixture.RootElement
            .GetProperty("validInputs")
            .EnumerateArray())
        {
            byte[] payload = Encoding.UTF8.GetBytes(
                testCase.GetProperty("input").GetRawText());

            ConversationChannelInput input =
                ConversationChannelJson.DeserializeInput(payload);

            Assert.Equal(ConversationChannelProtocol.Version, input.Version);
        }
    }

    [Fact]
    public void SharedInvalidFixturesFailClosed()
    {
        using JsonDocument fixture = LoadFixture();

        foreach (JsonElement testCase in fixture.RootElement
            .GetProperty("invalidInputs")
            .EnumerateArray())
        {
            byte[] payload = Encoding.UTF8.GetBytes(
                testCase.GetProperty("input").GetRawText());

            Exception? exception = Record.Exception(
                () => ConversationChannelJson.DeserializeInput(payload));

            Assert.True(
                exception is JsonException or InvalidDataException,
                $"Fixture {testCase.GetProperty("name").GetString()} did not fail closed.");
        }
    }

    [Fact]
    public void OutputRejectsExecutableOrUnknownResultShapes()
    {
        var output = new ConversationChannelOutput(
            ConversationChannelProtocol.Version,
            "message-1",
            "correlation-1",
            "session-1",
            "query",
            "device.execute",
            Decision: null,
            Request: null,
            Status: null,
            Error: null);

        Assert.Throws<InvalidDataException>(
            () => ConversationChannelJson.SerializeOutput(output));
    }

    private static JsonDocument LoadFixture()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            string candidate = Path.Combine(
                directory.FullName,
                "tests",
                "Fixtures",
                "conversation-channel-v1.json");
            if (File.Exists(candidate))
            {
                return JsonDocument.Parse(File.ReadAllBytes(candidate));
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(
            "The shared conversation channel fixture was not found.");
    }
}
