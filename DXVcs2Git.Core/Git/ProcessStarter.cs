using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.Core.Git {
    public class ProcessStarter {
        public ProcessStarter() {
        }

        public ProcessWrapper Start(ProcessStartInfo processStartInfo) {
            return new ProcessWrapper(processStartInfo, true);
        }

        public ProcessWrapper Start(string fileName, string arguments) {
            Process process = Process.Start(fileName, arguments);
            if (process == null && (fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || !string.IsNullOrEmpty(arguments)))
                throw new InvalidOperationException("Failed to start executable");
            return new ProcessWrapper(process);
        }

        public ProcessWrapper OpenToEdit(string path) {
            path = new FileInfo(path).FullName;
            ProcessStartInfo processStartInfo = new ProcessStartInfo(path) {
                ErrorDialog = false,
                Verb = "Edit",
                UseShellExecute = true
            };
            try {
                return this.Start(processStartInfo);
            }
            catch (Win32Exception) {
                return this.Start(new ProcessStartInfo(ProcessStarter.isSafeFile(path) ? path : Path.GetDirectoryName(path)) {
                    ErrorDialog = false,
                    UseShellExecute = true
                });
            }
        }

        private static bool isSafeFile(string path) {
            string[] strArray = new string[9]
            {
        ".sln",
        ".csproj",
        ".vbproj",
        ".fsproj",
        ".gif",
        ".png",
        ".jpg",
        ".jpeg",
        ".bmp"
            };
            string ext = Path.GetExtension(path);
            return Enumerable.Any<string>((IEnumerable<string>)strArray, (Func<string, bool>)(x => x.Equals(ext, StringComparison.OrdinalIgnoreCase)));
        }

        public IObservableProcess StartObservableProcess(ProcessStartInfo startInfo, bool throwOnNonZeroExitCode) {
            return (IObservableProcess)new ObservableProcess(startInfo, throwOnNonZeroExitCode);
        }

        //public IPipableObservableProcess StartObservablePipedProcess(ProcessStartInfo startInfo, bool throwOnNonZeroExitCode) {
        //    Ensure.ArgumentNotNull((object)startInfo, "startInfo");
        //    return (IPipableObservableProcess)new PipableObservableProcess(startInfo, throwOnNonZeroExitCode);
        //}
    }
}
