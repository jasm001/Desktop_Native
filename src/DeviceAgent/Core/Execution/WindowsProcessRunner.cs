using System.Diagnostics;

namespace ITSupportNative.DeviceAgent.Execution;

public sealed class WindowsProcessRunner : IProcessRunner
{
    public async Task<ProcessExecutionResult> RunAsync(
        ProcessExecutionRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = request.FileName,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            },
        };
        foreach (string argument in request.Arguments)
        {
            process.StartInfo.ArgumentList.Add(argument);
        }

        try
        {
            if (!process.Start())
            {
                return ProcessExecutionResult.StartFailed();
            }
        }
        catch (Exception exception) when (
            exception is InvalidOperationException
            or System.ComponentModel.Win32Exception)
        {
            return ProcessExecutionResult.StartFailed();
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        using var timeout = new CancellationTokenSource(request.Timeout);
        try
        {
            await process.WaitForExitAsync(timeout.Token);
            return ProcessExecutionResult.Completed(process.ExitCode);
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested)
        {
            TryTerminate(process);
            return ProcessExecutionResult.TimedOut();
        }
    }

    private static void TryTerminate(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit();
            }
        }
        catch (Exception exception) when (
            exception is InvalidOperationException
            or System.ComponentModel.Win32Exception
            or NotSupportedException)
        {
            // The caller still receives a typed timeout without process details.
        }
    }
}
