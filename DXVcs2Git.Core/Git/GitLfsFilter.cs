using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using LibGit2Sharp;

namespace DXVcs2Git.Core.Git {
    public interface IObservableProcess {
        IObservable<string> CombinedOutput { get; }
        IObservable<string> Output { get; }
        IObservable<string> Error { get; }
        IObserver<string> Input { get; }
    }

    public interface IPipableObservableProcess : IObservable<Unit> {
        Stream StandardInput { get; }
        Stream StandardOutput { get; }
    }

    public interface IGitShell {
        IObservableProcess Execute(string arguments, bool throwOnNonZeroExitCode);
        IObservableProcess Execute(string arguments, string workingDirectory, bool throwOnNonZeroExitCode);
        IPipableObservableProcess ExecutePipedProcess(string arguments, string workingDirectory, bool throwOnNonZeroExitCode);
    }

    public class GitLfsFilter : Filter {
        static readonly FilterAttributeEntry FilterAttribute = new FilterAttributeEntry("lfs");
        static readonly string FilterName = "git-lfs-filter";
        IGitShell Shell { get; }
        public GitLfsFilter(IGitShell shell) : base(FilterName, new List<FilterAttributeEntry> {FilterAttribute}) {
            Shell = shell;
        }
        protected override void Clean(string path, string repositoryWorkingDirectory, Stream input, Stream output) {
            CommandArguments command = new CommandArguments();
            command.AddArg("lfs");
            command.AddArg("clean");
            this.ExecuteMediaFilter(command, repositoryWorkingDirectory, input, output);
        }
        protected override void Smudge(string path, string repositoryWorkingDirectory, Stream input, Stream output) {
            CommandArguments command = new CommandArguments();
            command.AddArg("lfs");
            command.AddArg("smudge");
            this.ExecuteMediaFilter(command, repositoryWorkingDirectory, input, output);
        }
        int ExecuteMediaFilter(CommandArguments command, string workingDirectory, Stream inputBufferReader, Stream outputBufferWriter) {
            IPipableObservableProcess This = Shell.ExecutePipedProcess(command.ToString(), workingDirectory, true);
            IObservable<Unit> inUnit = WriteGitBufInputToStandardIn(This.StandardInput, inputBufferReader);
            IObservable<Unit> outUnit = WriteStandardOutToGitBufOutput(This.StandardOutput, outputBufferWriter);
            This.SelectUnit().Merge(inUnit).Merge(outUnit).Wait();
            return 0;
        }
        static IObservable<Unit> WriteStandardOutToGitBufOutput(Stream streamReader, Stream inputStream) {
            return Observable.Defer(() => Observable.Start(() => streamReader.CopyTo(inputStream)));
        }
        static IObservable<Unit> WriteGitBufInputToStandardIn(Stream streamWriter, Stream inputStream) {
            return Observable.Defer(() => Observable.Start(() => {
                try {
                    inputStream.CopyTo(streamWriter);
                    streamWriter.Close();
                }
                catch (Exception ex) {
                    if (ex.IsCriticalException())
                        throw;
                    Log.Error("Error occurred while trying to write memory stream to the standard input", ex);
                }
            }));
        }
        static IObservable<Unit> TryReparseProcessException(IPipableObservableProcess pipable) {
            return pipable.Catch<Unit, ProcessException>(Observable.Throw<Unit>);
        }
    }
}