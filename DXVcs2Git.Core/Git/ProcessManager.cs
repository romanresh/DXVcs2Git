using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.Core.Git {
    // Decompiled with JetBrains decompiler
    // Type: GitHub.PortableGit.Helpers.MsysGitProcessManager
    // Assembly: GitHub.PortableGit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    // MVID: 42A54A6F-3B9C-4837-B827-37B76CD34DD3
    // Assembly location: C:\Users\litvinov\AppData\Local\Apps\2.0\H3M474R0.E4B\Y5WXQCJ3.3PK\gith..tion_317444273a93ac29_0003.0000_12384c781d7f8ad4\GitHub.PortableGit.dll

    namespace GitHub.PortableGit.Helpers {
        public class ProcessManager {
            private readonly ProcessStarter processManager;

            public ProcessManager(ProcessStarter processManager) {
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

}
