using System.Text;
using System.Text.Json;
using ITSupportNative.DeviceAgent.Execution;

namespace ITSupportNative.IntegrationTests;

public sealed class SevenZipAdapterTests : IDisposable
{
    private readonly string _testDirectory = Path.Combine(
        Path.GetTempPath(),
        "ITSupportNative.AdapterTests",
        Guid.NewGuid().ToString("N"));

    [Fact]
    public void VersionedManifestMatchesCompiledAllowlist()
    {
        string repositoryRoot = FindRepositoryRoot();
        string manifestPath = Path.Combine(
            repositoryRoot,
            "deploy",
            "local-demo",
            "manifests",
            "seven-zip-26.01-x64.json");
        using JsonDocument manifest = JsonDocument.Parse(File.ReadAllBytes(manifestPath));
        JsonElement root = manifest.RootElement;

        Assert.Equal(1, root.GetProperty("schemaVersion").GetInt32());
        Assert.Equal(
            SevenZip2601X64Definition.Artifact.ArtifactId,
            root.GetProperty("artifactId").GetString());
        Assert.Equal(
            SevenZip2601X64Definition.Artifact.FileName,
            root.GetProperty("fileName").GetString());
        Assert.Equal(
            SevenZip2601X64Definition.Artifact.Length,
            root.GetProperty("length").GetInt64());
        Assert.Equal(
            SevenZip2601X64Definition.Artifact.Sha256,
            root.GetProperty("sha256").GetString());
        Assert.Equal(
            SevenZip2601X64Definition.ProductCode,
            root.GetProperty("productCode").GetString());
        Assert.Equal(
            SevenZip2601X64Definition.AdapterId,
            root.GetProperty("adapterId").GetString());
        Assert.Equal("not-signed", root.GetProperty("signature").GetString());
        Assert.Equal("development-only", root.GetProperty("environment").GetString());
    }

    [Fact]
    public async Task LocalArtifactSourceRequiresExactLengthAndHash()
    {
        Directory.CreateDirectory(_testDirectory);
        string filePath = Path.Combine(_testDirectory, "package.msi");
        byte[] bytes = Encoding.ASCII.GetBytes("fixed-package");
        await File.WriteAllBytesAsync(filePath, bytes);
        string hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes));
        var source = new LocalDevelopmentArtifactSource(_testDirectory);

        SoftwareArtifactResolution valid = await source.ResolveAsync(
            new("package", "package.msi", bytes.Length, hash),
            CancellationToken.None);
        SoftwareArtifactResolution wrongLength = await source.ResolveAsync(
            new("package", "package.msi", bytes.Length + 1, hash),
            CancellationToken.None);
        SoftwareArtifactResolution wrongHash = await source.ResolveAsync(
            new("package", "package.msi", bytes.Length, new string('0', 64)),
            CancellationToken.None);

        Assert.True(valid.IsAvailable);
        Assert.Equal(SoftwareArtifactResolutionCode.LengthMismatch, wrongLength.Code);
        Assert.Equal(SoftwareArtifactResolutionCode.HashMismatch, wrongHash.Code);
    }

    [Fact]
    public async Task PreflightFailsClosedOutsideLocalDemo()
    {
        SevenZip2601X64Adapter adapter = CreateAdapter(
            executionProfile: "disabled",
            detectorResults: [SoftwareDetectionResult.Absent()]);

        SoftwareExecutionResult result = await adapter.PreflightAsync(
            CancellationToken.None);

        Assert.Equal(SoftwareExecutionResultCode.ProfileDisabled, result.Code);
    }

    [Fact]
    public async Task PreflightRejectsUnsupportedPlatformBeforeArtifactResolution()
    {
        var source = new StubArtifactSource(
            SoftwareArtifactResolution.Available(@"C:\mirror\7z2601-x64.msi"));
        SevenZip2601X64Adapter adapter = CreateAdapter(
            detectorResults: [SoftwareDetectionResult.Absent()],
            artifactSource: source,
            platform: new StubExecutionPlatform(IsWindowsX64: false));

        SoftwareExecutionResult result = await adapter.PreflightAsync(
            CancellationToken.None);

        Assert.Equal(SoftwareExecutionResultCode.UnsupportedPlatform, result.Code);
        Assert.Equal(0, source.CallCount);
    }

    [Fact]
    public async Task InstallIsIdempotentWhenExactVersionIsDetected()
    {
        var runner = new RecordingProcessRunner(ProcessExecutionResult.Completed(0));
        SevenZip2601X64Adapter adapter = CreateAdapter(
            detectorResults:
            [
                SoftwareDetectionResult.Installed(
                    SevenZip2601X64Definition.PackageVersion),
            ],
            processRunner: runner);

        SoftwareExecutionResult result = await adapter.InstallAsync(
            CancellationToken.None);

        Assert.Equal(SoftwareExecutionResultCode.AlreadyInDesiredState, result.Code);
        Assert.Empty(runner.Requests);
    }

    [Fact]
    public async Task InstallUsesOnlyFixedMsiArgumentsAndVerifies()
    {
        var runner = new RecordingProcessRunner(ProcessExecutionResult.Completed(0));
        SevenZip2601X64Adapter adapter = CreateAdapter(
            detectorResults:
            [
                SoftwareDetectionResult.Absent(),
                SoftwareDetectionResult.Installed(
                    SevenZip2601X64Definition.PackageVersion),
            ],
            processRunner: runner);

        SoftwareExecutionResult result = await adapter.InstallAsync(
            CancellationToken.None);

        Assert.Equal(SoftwareExecutionResultCode.Succeeded, result.Code);
        ProcessExecutionRequest request = Assert.Single(runner.Requests);
        Assert.Equal("msiexec.exe", Path.GetFileName(request.FileName));
        Assert.Equal(
            ["/i", @"C:\mirror\7z2601-x64.msi", "/qn", "/norestart"],
            request.Arguments);
        Assert.Equal(SevenZip2601X64Definition.OperationTimeout, request.Timeout);
    }

    [Theory]
    [InlineData(
        SoftwareArtifactResolutionCode.SourceUnavailable,
        SoftwareExecutionResultCode.ArtifactUnavailable)]
    [InlineData(
        SoftwareArtifactResolutionCode.LengthMismatch,
        SoftwareExecutionResultCode.ArtifactLengthMismatch)]
    [InlineData(
        SoftwareArtifactResolutionCode.HashMismatch,
        SoftwareExecutionResultCode.ArtifactHashMismatch)]
    public async Task InstallRejectsMirrorAndIntegrityFailures(
        SoftwareArtifactResolutionCode artifactCode,
        SoftwareExecutionResultCode expectedCode)
    {
        var runner = new RecordingProcessRunner(ProcessExecutionResult.Completed(0));
        SevenZip2601X64Adapter adapter = CreateAdapter(
            detectorResults: [SoftwareDetectionResult.Absent()],
            artifactSource: new StubArtifactSource(
                SoftwareArtifactResolution.Rejected(artifactCode)),
            processRunner: runner);

        SoftwareExecutionResult result = await adapter.InstallAsync(
            CancellationToken.None);

        Assert.Equal(expectedCode, result.Code);
        Assert.Empty(runner.Requests);
    }

    [Fact]
    public async Task InstallReturnsTypedTimeout()
    {
        SevenZip2601X64Adapter adapter = CreateAdapter(
            detectorResults: [SoftwareDetectionResult.Absent()],
            processRunner: new RecordingProcessRunner(
                ProcessExecutionResult.TimedOut()));

        SoftwareExecutionResult result = await adapter.InstallAsync(
            CancellationToken.None);

        Assert.Equal(SoftwareExecutionResultCode.TimedOut, result.Code);
    }

    [Fact]
    public async Task InstallReturnsControlledProcessStartFailure()
    {
        SevenZip2601X64Adapter adapter = CreateAdapter(
            detectorResults: [SoftwareDetectionResult.Absent()],
            processRunner: new RecordingProcessRunner(
                ProcessExecutionResult.StartFailed()));

        SoftwareExecutionResult result = await adapter.InstallAsync(
            CancellationToken.None);

        Assert.Equal(SoftwareExecutionResultCode.ProcessStartFailed, result.Code);
    }

    [Theory]
    [InlineData(1603, SoftwareExecutionResultCode.ExitCodeRejected)]
    [InlineData(1641, SoftwareExecutionResultCode.RebootInitiated)]
    public async Task InstallRejectsUndeclaredOrUnsafeExitCodes(
        int exitCode,
        SoftwareExecutionResultCode expectedCode)
    {
        SevenZip2601X64Adapter adapter = CreateAdapter(
            detectorResults: [SoftwareDetectionResult.Absent()],
            processRunner: new RecordingProcessRunner(
                ProcessExecutionResult.Completed(exitCode)));

        SoftwareExecutionResult result = await adapter.InstallAsync(
            CancellationToken.None);

        Assert.Equal(expectedCode, result.Code);
        Assert.Equal(exitCode, result.ExitCode);
    }

    [Fact]
    public async Task InstallPreservesTypedRebootRequiredResultAfterVerification()
    {
        SevenZip2601X64Adapter adapter = CreateAdapter(
            detectorResults:
            [
                SoftwareDetectionResult.Absent(),
                SoftwareDetectionResult.Installed(
                    SevenZip2601X64Definition.PackageVersion),
            ],
            processRunner: new RecordingProcessRunner(
                ProcessExecutionResult.Completed(3010)));

        SoftwareExecutionResult result = await adapter.InstallAsync(
            CancellationToken.None);

        Assert.Equal(SoftwareExecutionResultCode.SucceededRebootRequired, result.Code);
        Assert.True(result.RebootRequired);
    }

    [Fact]
    public async Task InstallFailsWhenPostconditionCannotBeVerified()
    {
        SevenZip2601X64Adapter adapter = CreateAdapter(
            detectorResults:
            [
                SoftwareDetectionResult.Absent(),
                SoftwareDetectionResult.Absent(),
            ],
            processRunner: new RecordingProcessRunner(
                ProcessExecutionResult.Completed(0)));

        SoftwareExecutionResult result = await adapter.InstallAsync(
            CancellationToken.None);

        Assert.Equal(SoftwareExecutionResultCode.VerificationFailed, result.Code);
    }

    [Fact]
    public async Task UninstallUsesFixedProductCodeAndVerifiesAbsence()
    {
        var runner = new RecordingProcessRunner(ProcessExecutionResult.Completed(0));
        SevenZip2601X64Adapter adapter = CreateAdapter(
            detectorResults:
            [
                SoftwareDetectionResult.Installed(
                    SevenZip2601X64Definition.PackageVersion),
                SoftwareDetectionResult.Absent(),
            ],
            processRunner: runner);

        SoftwareExecutionResult result = await adapter.UninstallAsync(
            CancellationToken.None);

        Assert.Equal(SoftwareExecutionResultCode.Succeeded, result.Code);
        ProcessExecutionRequest request = Assert.Single(runner.Requests);
        Assert.Equal(
            [
                "/x",
                SevenZip2601X64Definition.ProductCode,
                "/qn",
                "/norestart",
            ],
            request.Arguments);
    }

    [Fact]
    public async Task UninstallIsIdempotentWhenProductIsAbsent()
    {
        var runner = new RecordingProcessRunner(ProcessExecutionResult.Completed(0));
        SevenZip2601X64Adapter adapter = CreateAdapter(
            detectorResults: [SoftwareDetectionResult.Absent()],
            processRunner: runner);

        SoftwareExecutionResult result = await adapter.UninstallAsync(
            CancellationToken.None);

        Assert.Equal(SoftwareExecutionResultCode.AlreadyInDesiredState, result.Code);
        Assert.Empty(runner.Requests);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    private static SevenZip2601X64Adapter CreateAdapter(
        IReadOnlyList<SoftwareDetectionResult> detectorResults,
        string executionProfile = "local-demo",
        ISoftwareArtifactSource? artifactSource = null,
        IProcessRunner? processRunner = null,
        IExecutionPlatform? platform = null)
    {
        return new(
            executionProfile,
            artifactSource ?? new StubArtifactSource(
                SoftwareArtifactResolution.Available(@"C:\mirror\7z2601-x64.msi")),
            new SequenceProductDetector(detectorResults),
            processRunner ?? new RecordingProcessRunner(
                ProcessExecutionResult.Completed(0)),
            platform ?? new StubExecutionPlatform(IsWindowsX64: true));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "ITSupportNative.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Repository root was not found.");
    }

    private sealed class StubArtifactSource(
        SoftwareArtifactResolution resolution) : ISoftwareArtifactSource
    {
        public int CallCount { get; private set; }

        public Task<SoftwareArtifactResolution> ResolveAsync(
            SoftwareArtifactDefinition artifact,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            return Task.FromResult(resolution);
        }
    }

    private sealed class SequenceProductDetector(
        IReadOnlyList<SoftwareDetectionResult> results) : ISoftwareProductDetector
    {
        private readonly Queue<SoftwareDetectionResult> _results = new(results);

        public Task<SoftwareDetectionResult> DetectAsync(
            string productCode,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Assert.Equal(SevenZip2601X64Definition.ProductCode, productCode);
            return Task.FromResult(_results.Dequeue());
        }
    }

    private sealed class RecordingProcessRunner(
        ProcessExecutionResult result) : IProcessRunner
    {
        public List<ProcessExecutionRequest> Requests { get; } = [];

        public Task<ProcessExecutionResult> RunAsync(
            ProcessExecutionRequest request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Requests.Add(request);
            return Task.FromResult(result);
        }
    }

    private sealed record StubExecutionPlatform(bool IsWindowsX64)
        : IExecutionPlatform;
}
