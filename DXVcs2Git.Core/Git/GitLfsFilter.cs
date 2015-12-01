using System.Collections.Generic;
using LibGit2Sharp;

namespace DXVcs2Git.Core.Git {
    public interface IGitProcessExecutor {
        
    }

    public class GitLfsFilter : Filter {
        static readonly FilterAttributeEntry FilterAttribute = new FilterAttributeEntry("filter=lfs");
        static readonly string FilterName = "git-lfs-filter";
        IGitProcessExecutor Shell { get; }
        public GitLfsFilter(IGitProcessExecutor shell) : base(FilterName, new List<FilterAttributeEntry>() { FilterAttribute }) {
            Shell = shell;
        }
    }


    //    private static readonly Logger log = LogManager.GetCurrentClassLogger();
    //    private static readonly FilterAttributeEntry filterAttribute = new FilterAttributeEntry("filter=lfs");
    //    private const string filterName = "git-lfs-filter";
    //    private readonly IGitProcessExecutor gitShell;

    //    [ImportingConstructor]
    //    public GitLfsFilter(IGitProcessExecutor shell)
    //      : base("git-lfs-filter", (IEnumerable<FilterAttributeEntry>)new List<FilterAttributeEntry>()
    //      {
    //    GitLfsFilter.filterAttribute
    //      }) {
    //        Ensure.ArgumentNotNull((object)shell, "shell");
    //        this.gitShell = shell;
    //    }

    //    public virtual Stream SmudgePointerFile(string path, string workingDirectory, Stream pointerFileInput) {
    //        Ensure.ArgumentNotNullOrEmptyString(path, "path");
    //        Ensure.ArgumentNotNull((object)pointerFileInput, "pointerFileInput");
    //        MemoryStream memoryStream = new MemoryStream();
    //        try {
    //            this.Smudge(path, workingDirectory, pointerFileInput, (Stream)memoryStream);
    //            memoryStream.Position = 0L;
    //        }
    //        catch (Exception ex) {
    //            GitLfsFilter.log.Error("Could not smudge pointer file", ex);
    //            throw new GitLfsSmudgeFailedException("Could not smudge pointer file", path, ex);
    //        }
    //        return (Stream)memoryStream;
    //    }

    //    protected override int Clean(string path, string repositoryWorkingDirectory, Stream input, Stream output) {
    //        CommandArguments command = new CommandArguments();
    //        command.AddArg("lfs");
    //        command.AddArg("clean");
    //        return this.ExecuteMediaFilter(command, repositoryWorkingDirectory, input, output);
    //    }

    //    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "path")]
    //    protected override int Smudge(string path, string repositoryWorkingDirectory, Stream inputBufferReader, Stream outputBufferWriter) {
    //        CommandArguments command = new CommandArguments();
    //        command.AddArg("lfs");
    //        command.AddArg("smudge");
    //        return this.ExecuteMediaFilter(command, repositoryWorkingDirectory, inputBufferReader, outputBufferWriter);
    //    }

    //    private int ExecuteMediaFilter(CommandArguments command, string workingDirectory, Stream inputBufferReader, Stream outputBufferWriter) {
    //        IPipableObservableProcess This = this.gitShell.ExecutePipedProcess(command.ToString(), workingDirectory, true);
    //        IObservable<Unit> second1 = GitLfsFilter.WriteGitBufInputToStandardIn(This.StandardInput, inputBufferReader);
    //        IObservable<Unit> second2 = GitLfsFilter.WriteStandardOutToGitBufOutput(This.StandardOutput, outputBufferWriter);
    //        Observable.Wait<Unit>(Observable.Merge<Unit>(Observable.Merge<Unit>(ObservableEx.SelectUnit<Unit>(GitProcessErrorFilter.TryReparseProcessException(This)), second1), second2));
    //        return 0;
    //    }

    //    private static IObservable<Unit> WriteStandardOutToGitBufOutput(Stream streamReader, Stream inputStream) {
    //        return Observable.Defer<Unit>((Func<IObservable<Unit>>)(() => Observable.Start((Action)(() => streamReader.CopyTo(inputStream)), RxApp.TaskpoolScheduler)));
    //    }

    //    private static IObservable<Unit> WriteGitBufInputToStandardIn(Stream streamWriter, Stream inputStream) {
    //        return Observable.Defer<Unit>((Func<IObservable<Unit>>)(() => Observable.Start((Action)(() =>
    //        {
    //            try {
    //                inputStream.CopyTo(streamWriter);
    //                streamWriter.Close();
    //            }
    //            catch (Exception ex) {
    //                if (ExceptionExtensions.IsCriticalException(ex))
    //                    throw;
    //                else
    //                    GitLfsFilter.log.Error("Error occurred while trying to write memory stream to the standard input", ex);
    //            }
    //        }), RxApp.TaskpoolScheduler)));
    //    }
    //}
}
