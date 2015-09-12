﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DXVcs2Git;
using DXVcs2Git.DXVcs;
using DXVcs2Git.Tests;
using DXVcs2Git.Tests.TestHelpers;
using DXVCS;
using LibGit2Sharp;

namespace DXVcs2Git.Console {
    internal class Program {
        static Credentials credentials;
        static string path = @"z:\test\";
        static string testUrl = "http://litvinov-lnx/tester/testxpfall.git";
        static void Main(string[] args) {
            var repo = DXVcsConectionHelper.Connect(DefaultConfig.Config.AuxPath);
            List<string> branches = new List<string>() {
                @"$/2014.2/XPF/DevExpress.Xpf.Core",
                @"$/2015.1/XPF/DevExpress.Xpf.Core",
                @"$/2015.2/XPF/DevExpress.Xpf.Core",
            };

            System.Console.WriteLine($"========   Start generating branch timings   ===========");
            List<DateTime> branchesCreatedTime = branches.Select(x => {
                var history = repo.GetProjectHistory(x, true);
                return history.First(IsBranchCreatedTimeStamp).ActionDate;
            }).Concat(new[] {DateTime.Now}).ToList();
            System.Console.WriteLine($"========   Completed generating branch timings   ===========");
            System.Console.WriteLine($"========   Start generating project history   ===========");
            var resultHistory = Enumerable.Empty<HistoryItem>();
            DateTime previousStamp = branchesCreatedTime[0];
            for (int i = 0; i < branches.Count; i++) {
                DateTime currentStamp = branchesCreatedTime[i + 1];
                string branch = branches[i];
                System.Console.WriteLine($"========   Start generating project history for brahch {branch}  ===========");
                var history = repo.GetProjectHistory(branch, true, previousStamp, currentStamp);
                System.Console.WriteLine($"========   Completed generating project history for brahch {branch}  ===========");

                var projectHistory = CalcProjectHistory(history).Where(x => x.TimeStamp >= previousStamp && x.TimeStamp <= currentStamp).ToList();
                foreach (var historyItem in projectHistory) {
                    historyItem.Branch = branch;
                }
                resultHistory = resultHistory.Concat(projectHistory);
                previousStamp = currentStamp;
            }
            resultHistory = resultHistory.ToList();
            System.Console.WriteLine($"========   Completed generating project history   ===========");
            InitUserCredentials();
            GitWrapper gitRepo = new GitWrapper(path, testUrl, credentials);
            System.Console.WriteLine($"========   Start updating git repo    ===========");
            gitRepo.Fetch();
            System.Console.WriteLine($"========   Startup git fetch completed   ===========");
            foreach (var item in resultHistory) {
                System.Console.WriteLine($"========   Start updating project {item.Branch} {item.TimeStamp} ===========");
                CleanUpDir(path);
                repo.GetProject(item.Branch, path, item.TimeStamp);
                System.Console.WriteLine($"========   Completed updating project   ===========");
                if (IsDirEmpty(path)) {
                    System.Console.WriteLine($"========   No history for {item.Branch}  {item.TimeStamp}  ===========");
                    continue;
                }
                PreprocessRepo(path);

                gitRepo.Fetch();
                gitRepo.Stage("*");

                string user = item.History.First().User;
                gitRepo.Commit(CalcComment(item), user, item.TimeStamp);

                gitRepo.Push("master");
            }
        }
        static void CleanUpDir(string path) {
            string gitPath = Path.Combine(path, ".git");
            foreach (var dir in Directory.EnumerateDirectories(path)) {
                if (dir == gitPath)
                    continue;
                Directory.Delete(dir, true);
                foreach (var file in Directory.EnumerateFiles(path)) {
                    File.Delete(file);
                }
            }
        }
        static void PreprocessRepo(string path) {
            string gitPath = Path.Combine(path, ".git");
            foreach (var dir in Directory.EnumerateDirectories(path)) {
                if (dir == gitPath)
                    continue;
                PreprocessRepo(dir);
                if (!Directory.EnumerateFiles(dir).Any())
                    AddEmptyGitIgnore(dir);
            }
        }
        static void AddEmptyGitIgnore(string path) {
            using (var file = File.Create(Path.Combine(path, ".gitignore"))) {
            }
        }
        static bool IsDirEmpty(string path) {
            return !Directory.EnumerateDirectories(path).Any(x => {
                string dirName = Path.GetFileName(x);
                return dirName != ".git";
            });
        }
        static string CalcComment(HistoryItem item) {
            var messageItem = item.History.FirstOrDefault(x => !string.IsNullOrEmpty(x.Message));
            if (!string.IsNullOrEmpty(messageItem.Message))
                return messageItem.Message;
            var commentItem = item.History.FirstOrDefault(x => !string.IsNullOrEmpty(x.Comment));
            if (!string.IsNullOrEmpty(commentItem.Comment))
                return commentItem.Comment;
            return string.Empty;
        }
        static void InitUserCredentials() {
            var user = new UsernamePasswordCredentials();
            user.Username = Constants.Identity.Name;
            user.Password = "q1w2e3r4t5y6";
            credentials = user;
            System.Console.WriteLine($"========   User Initialized   ===========");
        }
        static bool IsBranchCreatedTimeStamp(ProjectHistoryInfo x) {
            return x.Message != null && x.Message.ToLowerInvariant() == "create";
        }
        static IEnumerable<HistoryItem> CalcProjectHistory(IEnumerable<ProjectHistoryInfo> history) {
            return history.Reverse().GroupBy(x => x.ActionDate).OrderBy(x => x.First().ActionDate).Select(x => new HistoryItem(x.First().ActionDate, x.ToList()));
        }
    }

}