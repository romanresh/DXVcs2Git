﻿namespace DXVcs2Git.Core.Git {
    public class GitRepoConfig {
        public string Name { get; set; }
        public string FarmTaskName { get; set; }
        public string FarmSyncTaskName { get; set; }
        public string DefaultServiceName { get; set; }
        public string TargetBranch { get; set; }
        public string Server { get; set; }
    }
}
