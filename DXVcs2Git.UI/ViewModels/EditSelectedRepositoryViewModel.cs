﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DXVcs2Git.Core;
using DXVcs2Git.Core.Git;
using DXVcs2Git.Core.GitLab;

namespace DXVcs2Git.UI.ViewModels {
    public class EditSelectedRepositoryViewModel : ViewModelBase {
        new RepositoriesViewModel Parameter { get { return (RepositoriesViewModel)base.Parameter; } }
        public bool IsInitialized {
            get { return GetProperty(() => IsInitialized); }
            private set { SetProperty(() => IsInitialized, value); }
        }
        public BranchViewModel SelectedBranch {
            get { return GetProperty(() => SelectedBranch); }
            set { SetProperty(() => SelectedBranch, value); }
        }
        public ICommand CreateMergeRequestCommand { get; private set; }
        public ICommand EditMergeRequestCommand { get; private set; }
        public ICommand CloseMergeRequestCommand { get; private set; }
        public bool HasMergeRequest {
            get { return GetProperty(() => HasMergeRequest); }
            private set { SetProperty(() => HasMergeRequest, value); }
        }
        public bool IsMyMergeRequest { get; private set; }
        public bool HasChanged { get; private set; }
        public bool HasEditableMergeRequest {
            get { return GetProperty(() => HasEditableMergeRequest); }
            private set { SetProperty(() => HasEditableMergeRequest, value, HasEditableMergeRequestChanged); }
        }
        bool AlwaysSure { get { return Core.Configuration.ConfigSerializer.GetConfig().AlwaysSure; } }
        void HasEditableMergeRequestChanged() {
            if (SelectedBranch != null)
                SelectedBranch.IsInEditingMergeRequest = HasEditableMergeRequest;
            Parameter.Refresh();
        }

        IMessageBoxService MessageBoxService { get { return this.GetService<IMessageBoxService>("MessageBoxService"); } }

        public EditSelectedRepositoryViewModel() {
            CreateMergeRequestCommand = DelegateCommandFactory.Create(CreateMergeRequest, CanCreateMergeRequest);
            EditMergeRequestCommand = DelegateCommandFactory.Create(ProcessEditMergeRequest, CanEditMergeRequest);
            CloseMergeRequestCommand = DelegateCommandFactory.Create(CloseMergeRequest, CanCloseMergeRequest);
        }
        bool CanCloseMergeRequest() {
            return Parameter.IsInitialized && IsInitialized && SelectedBranch != null && HasMergeRequest && IsMyMergeRequest;
        }
        bool CanEditMergeRequest() {
            return Parameter.IsInitialized && IsInitialized && SelectedBranch != null && HasMergeRequest && IsMyMergeRequest && HasChanged;
        }
        void ProcessEditMergeRequest() {
            HasEditableMergeRequest = true;
            Parameter.Refresh();
        }
        bool CanCreateMergeRequest() {
            return Parameter.IsInitialized && IsInitialized && SelectedBranch != null && !HasMergeRequest;
        }
        public void CreateMergeRequest() {
            Parameter.Refresh();
            if (!CanCreateMergeRequest())
                return;

            var message = SelectedBranch.Branch.Commit.Message;
            string title = CalcMergeRequestTitle(message);
            string description = CalcMergeRequestDescription(message);
            string targetBranch = CalcTargetBranch(SelectedBranch.Name);
            if (targetBranch == null)
                return;
            SelectedBranch.CreateMergeRequest(title, description, null, SelectedBranch.Name, targetBranch);
            ProcessEditMergeRequest();
            Refresh();
        }
        public void Update() {
            Refresh();
        }
        public void Refresh() {
            IsInitialized = Parameter.Return(x => x.IsInitialized, () => false);
            SelectedBranch = Parameter.With(x => x.SelectedRepository).With(x => x.SelectedBranch);
            HasMergeRequest = SelectedBranch.Return(x => x.MergeRequest != null, () => false);
            HasEditableMergeRequest = SelectedBranch?.IsInEditingMergeRequest ?? false;
            if (HasMergeRequest) {
                HasChanged = SelectedBranch.Return(x => x.MergeRequest.Changes.Any(), () => false);
            }
            else {
                HasChanged = false;
            }
            IsMyMergeRequest = HasMergeRequest;
        }
        string CalcTargetBranch(string name) {
            GitRepoConfig repoConfig = SelectedBranch.Repository.RepoConfig;
            if (repoConfig != null)
                return repoConfig.TargetBranch;
            return Parameter.ProtectedBranches.FirstOrDefault(x => name.StartsWith(x.Name)).With(x => x.Name);
        }
        string CalcServiceName() {
            GitRepoConfig repoConfig = SelectedBranch.Repository.RepoConfig;
            if (repoConfig != null)
                return repoConfig.DefaultServiceName;
            return null;
        }
        public void UpdateMergeRequest(EditMergeRequestData data) {
            if (AlwaysSure || MessageBoxService.Show("Are you sure?", "Update merge request", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                SelectedBranch.UpdateMergeRequest(CalcMergeRequestTitle(data.Comment), CalcMergeRequestDescription(data.Comment), data.AssignToService ? CalcServiceName() : SelectedBranch.MergeRequest.Assignee);
                SelectedBranch.UpdateMergeRequest(CalcOptionsComment(data.Options));
                CloseEditableMergeRequest();
                Parameter.Refresh();
            }
        }
        string CalcOptionsComment(MergeRequestOptions options) {
            return MergeRequestOptions.ConvertToString(options);
        }
        public void CloseMergeRequest() {
            if (AlwaysSure || MessageBoxService.Show("Are you sure?", "Close merge request", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                SelectedBranch.CloseMergeRequest();
                CloseEditableMergeRequest();
                Parameter.Refresh();
            }
        }
        void CloseEditableMergeRequest() {
            HasEditableMergeRequest = false;
        }
        string CalcMergeRequestDescription(string message) {
            var changes = message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            changes.Skip(1).ForEach(x => sb.AppendLine(x.ToString()));
            return sb.ToString();
        }
        string CalcMergeRequestTitle(string message) {
            var changes = message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var title = changes.FirstOrDefault();
            return title;
        }
        protected override void OnParentViewModelChanged(object parentViewModel) {
            base.OnParentViewModelChanged(parentViewModel);
            Refresh();
        }
        public void CancelEditMergeRequest() {
            HasEditableMergeRequest = false;
            Parameter.Refresh();
        }
    }
}
