using System.Diagnostics;
using System.IO;

namespace DXVcs2Git.Core.Git {
        public class GitProcessManager {
            readonly ProcessStarter processManager;
            public GitProcessManager(ProcessStarter processManager) {
                this.processManager = processManager;
            }
            public IObservableProcess StartProcess(string executableFileName, string arguments, string workingDirectory, bool throwOnNonZeroExitCode, bool ensureGitExtracted) {
                ProcessStartInfo startInfo = this.SetUpStartInfo(executableFileName, arguments, workingDirectory ?? Path.GetTempPath());
                startInfo.FileName = executableFileName;
                return this.processManager.StartObservableProcess(startInfo, throwOnNonZeroExitCode);
            }

            //public IPipableObservableProcess StartObservableRedirectedProcess(string executableFileName, string arguments, string workingDirectory, bool throwOnNonZeroExitCode, bool ensureGitExtracted) {
            //    Ensure.ArgumentNotNull((object)executableFileName, "executableFileName");
            //    if (ensureGitExtracted) {
            //        try {
            //            FuncExtensions.Retry<Unit>((Func<Unit>)(() => Observable.Wait<Unit>(this.gitEnvironment.EnsureGitExtracted())), 2);
            //        }
            //        catch (Exception ex) {
            //            ProcessManager.log.Error("Oh that's bad. Could not extract Git. Going to let this propagate.", ex);
            //        }
            //    }
            //    ProcessStartInfo startInfo = this.SetUpStartInfo(executableFileName, arguments, workingDirectory ?? Path.GetTempPath());
            //    startInfo.FileName = OperatingSystemBridgeExtensions.FindExecutableInPath(this.operatingSystem, executableFileName, startInfo.EnvironmentVariables["PATH"]) ?? executableFileName;
            //    return this.processManager.StartObservablePipedProcess(startInfo, throwOnNonZeroExitCode);
            //}

            public ProcessStartInfo SetUpStartInfo(string executableFileName, string arguments, string workingDirectory) {
                return new ProcessStartInfo(executableFileName, arguments) {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = executableFileName
                };
            }
        }
    }
