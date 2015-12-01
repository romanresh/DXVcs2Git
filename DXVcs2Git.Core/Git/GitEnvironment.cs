//namespace DXVcs2Git.Core.Git {
//    namespace GitHub.PortableGit.Helpers {
//        [Export(typeof(IGitEnvironment))]
//        public class GitEnvironment : IGitEnvironment {
//            private readonly IPortableGitManager portableGitManager;
//            private readonly IEnvironment environment;
//            private readonly ISshAgentBridge sshAgentBridge;
//            private readonly IGitLfsManager gitLfsManager;

//            [ImportingConstructor]
//            public GitEnvironment(IPortableGitManager portableGitManager, IGitLfsManager gitLfsManager, IEnvironment environment, ISshAgentBridge sshAgentBridge) {
//                Ensure.ArgumentNotNull((object)portableGitManager, "portableGitManager");
//                Ensure.ArgumentNotNull((object)gitLfsManager, "GitLfsManager");
//                Ensure.ArgumentNotNull((object)environment, "environment");
//                Ensure.ArgumentNotNull((object)sshAgentBridge, "sshAgentBridge");
//                this.portableGitManager = portableGitManager;
//                this.gitLfsManager = gitLfsManager;
//                this.environment = environment;
//                this.sshAgentBridge = sshAgentBridge;
//            }

//            public void SetUpEnvironment(ProcessStartInfo psi, string workingDirectory, bool internalUseOnly) {
//                Ensure.ArgumentNotNull((object)psi, "psi");
//                string str1 = (string)this.environment.UserProfilePath;
//                string str2 = internalUseOnly ? this.environment.ExpandEnvironmentVariables("%SystemRoot%\\System32") : this.environment.GetEnvironmentVariable("PATH");
//                string destinationDirectory1 = this.portableGitManager.GetPortableGitDestinationDirectory(false);
//                string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
//                string str3 = this.environment.ExpandEnvironmentVariables("%SystemRoot%\\Microsoft.NET\\Framework\\v4.0.30319");
//                string destinationDirectory2 = this.gitLfsManager.GetGitLfsDestinationDirectory();
//                string path1 = this.environment.ExpandEnvironmentVariables(this.environment.Is64BitOperatingSystem ? "%ProgramFiles(x86)%\\MSBuild\\12.0\\bin" : "%ProgramFiles%\\MSBuild\\12.0\\bin");
//                if (File.Exists(Path.Combine(path1, "msbuild.exe")))
//                    str3 = path1;
//                psi.EnvironmentVariables["github_shell"] = "true";
//                psi.EnvironmentVariables["git_install_root"] = destinationDirectory1;
//                psi.EnvironmentVariables["github_git"] = destinationDirectory1;
//                psi.EnvironmentVariables["PLINK_PROTOCOL"] = "ssh";
//                psi.EnvironmentVariables["TERM"] = "msys";
//                psi.EnvironmentVariables["PATH"] = string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}\\cmd;{0}\\bin;{1};{2};{3};{4}", (object)destinationDirectory1, (object)directoryName, (object)destinationDirectory2, (object)str3, (object)str2);
//                psi.EnvironmentVariables["HOME"] = str1;
//                psi.EnvironmentVariables["TMP"] = psi.EnvironmentVariables["TEMP"] = Path.GetTempPath();
//                psi.EnvironmentVariables["EDITOR"] = this.environment.GetEnvironmentVariable("EDITOR") ?? "GitPad";
//                string environmentVariable1 = this.environment.GetEnvironmentVariable("HTTP_PROXY");
//                if (!string.IsNullOrWhiteSpace(environmentVariable1))
//                    psi.EnvironmentVariables["HTTP_PROXY"] = environmentVariable1;
//                string environmentVariable2 = this.environment.GetEnvironmentVariable("HTTPS_PROXY");
//                if (!string.IsNullOrWhiteSpace(environmentVariable2))
//                    psi.EnvironmentVariables["HTTPS_PROXY"] = environmentVariable2;
//                SshAgentProcessInfo runningSshAgentInfo = this.sshAgentBridge.GetRunningSshAgentInfo();
//                if (runningSshAgentInfo != null) {
//                    psi.EnvironmentVariables["SSH_AGENT_PID"] = runningSshAgentInfo.ProcessId;
//                    psi.EnvironmentVariables["SSH_AUTH_SOCK"] = runningSshAgentInfo.AuthSocket;
//                }
//                GitEnvironment.log.Info<string, string>("Process set up with this SSH Agent info: {0}:{1}", psi.EnvironmentVariables["SSH_AGENT_PID"] ?? "(null)", psi.EnvironmentVariables["SSH_AUTH_SOCK"] ?? "(null)");
//                if (internalUseOnly) {
//                    psi.EnvironmentVariables["GIT_PAGER"] = "cat";
//                    psi.EnvironmentVariables["LC_ALL"] = "C";
//                    psi.EnvironmentVariables["GIT_ASKPASS"] = "true";
//                    psi.EnvironmentVariables["DISPLAY"] = "localhost:1";
//                    psi.EnvironmentVariables["SSH_ASKPASS"] = "true";
//                    psi.EnvironmentVariables["GIT_SSH"] = "ssh-noprompt";
//                    psi.StandardOutputEncoding = Encoding.UTF8;
//                    psi.StandardErrorEncoding = Encoding.UTF8;
//                }
//                GitEnvironment.log.Info((IFormatProvider)CultureInfo.InvariantCulture, "PATH is {0}", psi.EnvironmentVariables["PATH"]);
//                psi.WorkingDirectory = workingDirectory ?? str1;
//            }

//            public IObservable<Unit> EnsureGitExtracted() {
//                return ObservableEx.AsCompletion<ProgressResult>(ObservableEx.ContinueAfter<ProgressResult, ProgressResult>(this.portableGitManager.ExtractGitIfNeeded(), new Func<IObservable<ProgressResult>>(this.gitLfsManager.ExtractGitLfsIfNeeded)));
//            }
//        }
//    }

//}
