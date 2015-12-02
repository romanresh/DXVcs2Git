using System;
using System.Collections.Generic;
using System.Linq;
using NGitLab;
using NGitLab.Models;

namespace DXVcs2Git.Git {
    public class GitLabWrapper {
        readonly GitLabClient client;
        public GitLabWrapper(string server, string token) {
            client = GitLabClient.Connect(server, token);
        }
        public Project FindProject(string project) {
            return client.Projects.Accessible.FirstOrDefault(x => x.HttpUrl == project);
        }
        public IEnumerable<MergeRequest> GetMergeRequests(Project project, Func<MergeRequest, bool> mergeRequestsHandler = null) {
            mergeRequestsHandler = mergeRequestsHandler ?? (x => true);
            IMergeRequestClient mergeRequestsClient = client.GetMergeRequest(project.Id);
            return mergeRequestsClient.AllInState(MergeRequestState.opened).Where(x => mergeRequestsHandler(x));
        }
        public IEnumerable<MergeRequestFileData> GetMergeRequestChanges(MergeRequest mergeRequest) {
            IMergeRequestClient mergeRequestsClient = client.GetMergeRequest(mergeRequest.ProjectId);
            IMergeRequestChangesClient changesClient = mergeRequestsClient.Changes(mergeRequest.Id);
            return changesClient.Changes.Files;
        }
        public MergeRequest ProcessMergeRequest(MergeRequest mergeRequest, string comment) {
            IMergeRequestClient mergeRequestsClient = client.GetMergeRequest(mergeRequest.ProjectId);
            return mergeRequestsClient.Accept(mergeRequest.Id, new MergeCommitMessage {Message = comment});
        }
        public MergeRequest UpdateMergeRequest(MergeRequest mergeRequest, string autoMergeFailedComment) {
            IMergeRequestClient mergeRequestsClient = client.GetMergeRequest(mergeRequest.ProjectId);
            return mergeRequestsClient.Update(mergeRequest.Id, new MergeRequestUpdate {
                Description = autoMergeFailedComment,
                AssigneeId = mergeRequest.Assignee.Id
            });
        }
        public MergeRequest UpdateMergeRequestTitleAndDescription(MergeRequest mergeRequest, string title, string description) {
            IMergeRequestClient mergeRequestsClient = client.GetMergeRequest(mergeRequest.ProjectId);
            return mergeRequestsClient.Update(mergeRequest.Id, new MergeRequestUpdate {
                Description = description,
                Title = title
            });
        }
        public MergeRequest CloseMergeRequest(MergeRequest mergeRequest) {
            IMergeRequestClient mergeRequestsClient = client.GetMergeRequest(mergeRequest.ProjectId);
            return mergeRequestsClient.Update(mergeRequest.Id, new MergeRequestUpdate {NewState = "close"});
        }
        public MergeRequest CreateMergeRequest(Project project, string title, string description, string user, string sourceBranch, string targetBranch) {
            IMergeRequestClient mergeRequestClient = this.client.GetMergeRequest(project.Id);
            MergeRequest mergeRequest = mergeRequestClient.Create(new MergeRequestCreate {
                Title = title,
                SourceBranch = sourceBranch,
                TargetBranch = targetBranch,
                TargetProjectId = project.Id
            });
            return UpdateMergeRequestTitleAndDescription(mergeRequest, title, description);
        }
        public MergeRequest ReopenMergeRequest(MergeRequest mergeRequest, string autoMergeFailedComment) {
            IMergeRequestClient mergeRequestsClient = client.GetMergeRequest(mergeRequest.ProjectId);
            return mergeRequestsClient.Update(mergeRequest.Id, new MergeRequestUpdate {NewState = "reopen", Description = autoMergeFailedComment});
        }
        public IEnumerable<User> GetUsers() {
            IUserClient usersClient = this.client.Users;
            return usersClient.All.ToList();
        }
        public void RegisterUser(string userName, string displayName, string email) {
            IUserClient userClient = this.client.Users;
            UserUpsert userUpsert = new UserUpsert {Username = userName, Name = displayName, Email = email, Password = new Guid().ToString()};
            userClient.Create(userUpsert);
        }
        public IEnumerable<Branch> GetBranches(Project project) {
            IRepositoryClient repo = this.client.GetRepository(project.Id);
            IBranchClient branchesClient = repo.Branches;
            return branchesClient.All;
        }
        public MergeRequest UpdateMergeRequestAssignee(MergeRequest mergeRequest, User user) {
            IMergeRequestClient mergeRequestsClient = client.GetMergeRequest(mergeRequest.ProjectId);
            return mergeRequestsClient.Update(mergeRequest.Id, new MergeRequestUpdate {AssigneeId = user.Id});
        }
    }
}