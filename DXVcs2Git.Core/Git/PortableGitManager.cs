//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Reflection;

//public class PortableGitManager {
//        private readonly Lazy<string> gitExecutablePath;
//        private readonly Lazy<string> gitEtcDirPath;
//        //private readonly Lazy<IFile> systemConfigFile;
//        //private readonly IEmbeddedResource embeddedSystemConfigFile;

//        public string GitExecutablePath
//        {
//            get
//            {
//                return this.gitExecutablePath.Value;
//            }
//        }

//        public string EtcDirectoryPath
//        {
//            get
//            {
//                return this.gitEtcDirPath.Value;
//            }
//        }

//        public PortableGitManager()
//          : this(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) {
//        }

//        public PortableGitManager(string installLocation)
//          : this((IEmbeddedResource)new EmbeddedResource(typeof(PortableGitManager).Assembly, "GitHub.PortableGit.Resources.gitconfig", "gitconfig"), installLocation) {
//        }

//        public PortableGitManager(IOperatingSystem operatingSystem, IEmbeddedResource embeddedSystemConfigFile)
//          : this(operatingSystem, embeddedSystemConfigFile, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) {
//        }

//        public PortableGitManager(IOperatingSystem operatingSystem, IEmbeddedResource embeddedSystemConfigFile, string installLocation)
//          : base(operatingSystem, installLocation) {
//            PortableGitManager portableGitManager = this;
//            Ensure.ArgumentNotNull((object)embeddedSystemConfigFile, "embeddedSystemConfigFile");
//            this.embeddedSystemConfigFile = embeddedSystemConfigFile;
//            this.gitExecutablePath = new Lazy<string>((Func<string>)(() =>
//            {
//                // ISSUE: explicit non-virtual call
//                string path = Path.Combine(__nonvirtual(portableGitManager.GetPortableGitDestinationDirectory(false)), "bin", "git.exe");
//                if (!operatingSystem.FileExists(path))
//                    PortableGitManager.log.Error((IFormatProvider)CultureInfo.InvariantCulture, "git.exe doesn't exist at '{0}'", path);
//                return path;
//            }));
//            this.gitEtcDirPath = new Lazy<string>((Func<string>)(() => Path.Combine(this.GetPortableGitDestinationDirectory(false), "etc")));
//            // ISSUE: explicit non-virtual call
//            this.systemConfigFile = new Lazy<IFile>((Func<IFile>)(() => operatingSystem.GetFile(Path.Combine(__nonvirtual(portableGitManager.EtcDirectoryPath), "gitconfig"))));
//        }

//        public IObservable<ProgressResult> ExtractGitIfNeeded() {
//            return Observable.Defer<ProgressResult>((Func<IObservable<ProgressResult>>)(() => this.ExtractPackageIfNeeded("PortableGit.7z", new Action(this.KillAllSSHAgent), new int?(3280))));
//        }

//        public bool IsExtracted() {
//            return this.IsPackageExtracted();
//        }

//        public string GetPortableGitDestinationDirectory(bool createIfNeeded = false) {
//            return this.GetPackageDestinationDirectory(createIfNeeded, true);
//        }

//        public IObservable<IFile> EnsureSystemConfigFileExtracted() {
//            IFile file = this.systemConfigFile.Value;
//            if (file.Exists)
//                return Observable.Return<IFile>(file);
//            this.embeddedSystemConfigFile.ExtractToFile(this.EtcDirectoryPath);
//            file.Refresh();
//            return Observable.Return<IFile>(file);
//        }

//        protected override string GetExpectedVersion() {
//            return "c2ba306e536fdf878271f7fe636a147ff37326ad";
//        }

//        protected override string GetPathToCanary(string rootDir) {
//            return Path.Combine(rootDir ?? "C:\\__NOTHERE", "bin", "git.exe");
//        }

//        protected override string GetPackageName() {
//            return "PortableGit";
//        }

//        protected void KillAllSSHAgent() {
//            EnumerableEx.ForEach<Process>(Enumerable.Where<Process>((IEnumerable<Process>)Process.GetProcesses(), (Func<Process, bool>)(x => StringExtensions.Contains(x.ProcessName, "ssh-agent", StringComparison.OrdinalIgnoreCase))), (Action<Process>)(x =>
//            {
//                try {
//                    x.Kill();
//                }
//                catch (Exception ex) {
//                    Win32Exception win32Exception = ex as Win32Exception;
//                    if (win32Exception != null && win32Exception.NativeErrorCode == 5)
//                        return;
//                    PortableGitManager.log.Info(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Failed to kill ssh-agent with PID {0}", new object[1]
//                    {
//            (object) x.Id
//                    }), ex);
//                }
//            }));
//        }

//        string IPortableGitManager.GetPackageNameWithVersion() {
//            return this.GetPackageNameWithVersion();
//        }
//    }
//}
