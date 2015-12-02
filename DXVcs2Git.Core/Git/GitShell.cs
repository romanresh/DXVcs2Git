using System;
namespace DXVcs2Git.Core.Git {
    public interface IObservableProcess {
        IObservable<string> CombinedOutput { get; }
        IObservable<string> Output { get; }
        IObservable<string> Error { get; }
        IObserver<string> Input { get; }
    }
    public class GitShell : IGitProcessExecutor {
        readonly ProcessManager processManager;
        //private readonly IPortableGitManager portableGitManager;
        public GitShell(ProcessManager processManager /*IPortableGitManager portableGitManager*/) {
            //Ensure.ArgumentNotNull((object)processManager, "processManager");
            //Ensure.ArgumentNotNull((object)portableGitManager, "portableGitManager");
            this.processManager = processManager;
            //this.portableGitManager = portableGitManager;
        }

        public IObservableProcess Execute(string arguments, bool throwOnNonZeroExitCode) {
            return this.Execute(arguments, (string)null, throwOnNonZeroExitCode);
        }

        public IObservableProcess Execute(string arguments, string workingDirectory, bool throwOnNonZeroExitCode) {
            //IObservableProcess observableProcess = this.processManager.StartProcess(this.portableGitManager.GitExecutablePath, arguments, workingDirectory, throwOnNonZeroExitCode, false);
            //observableProcess.Input.OnCompleted();
            //return observableProcess;
            return null;
        }

        //public IObservable<ProgressResult> ExecuteProgress(string command, string defaultText, string workingDir = null, int firstSegment = 10) {
        //    return Observable.StartWith<ProgressResult>(Observable.Select<Tuple<int, string>, ProgressResult>(this.ExecuteCommandWithProgress(command, workingDir, firstSegment), (Func<Tuple<int, string>, ProgressResult>)(progress => {
        //        if (progress.Item1 < firstSegment && string.IsNullOrEmpty(progress.Item2))
        //            return new ProgressResult(progress.Item1, defaultText);
        //        return new ProgressResult(progress.Item1, progress.Item2);
        //    })), new ProgressResult[1]
        //    {
        //new ProgressResult(0, defaultText)
        //    });
        //}

        //public IPipableObservableProcess ExecutePipedProcess(string command, string workingDirectory, bool throwOnNonZeroExitCode) {
        //    return this.processManager.StartObservableRedirectedProcess(this.portableGitManager.GitExecutablePath, command, workingDirectory, throwOnNonZeroExitCode, true);
        //}

        //private IObservable<Tuple<int, string>> ExecuteCommandWithProgress(string command, string workingDir = null, int firstSegment = 10) {
        //    return Observable.Defer<Tuple<int, string>>((Func<IObservable<Tuple<int, string>>>)(() => {
        //        int num = (100 - firstSegment) / 3;
        //        int secondSegment = firstSegment + num;
        //        int thirdSegment = firstSegment + num * 2;
        //        int val = 0;
        //        IObservableProcess This = this.Execute(command, workingDir, true);
        //        IObservable<Tuple<int, string>> observable = Observable.Merge<Tuple<int, string>>(Observable.SelectMany<string, Tuple<int, string>>(This.CombinedOutput, (Func<string, IObservable<Tuple<int, string>>>)(x => {
        //            if (x.StartsWith("remote: Compressing objects:", StringComparison.Ordinal))
        //                val = Math.Max(firstSegment, firstSegment + GitProgressTextParser.PercentageFromGitCloneProgress(x) / 3);
        //            if (x.StartsWith("Receiving objects:", StringComparison.Ordinal))
        //                val = Math.Max(secondSegment, secondSegment + GitProgressTextParser.PercentageFromGitCloneProgress(x) / 3);
        //            if (x.StartsWith("Resolving deltas:", StringComparison.Ordinal))
        //                val = Math.Max(thirdSegment, thirdSegment + GitProgressTextParser.PercentageFromGitCloneProgress(x) / 3);
        //            return Observable.Return<Tuple<int, string>>(new Tuple<int, string>(Math.Min(100, val), x));
        //        })), Observable.Select<Unit, Tuple<int, string>>(GitProcessErrorFilter.TryReparseProcessException(This), (Func<Unit, Tuple<int, string>>)(_ => new Tuple<int, string>(100, string.Empty))));
        //        return Observable.Concat<Tuple<int, string>>(Observable.TakeUntil<Tuple<int, string>, Tuple<int, string>>(Observable.Select<long, Tuple<int, string>>(Observable.Where<long>(Observable.Timer(DateTimeOffset.MinValue, TimeSpan.FromMilliseconds(300.0), RxApp.TaskpoolScheduler), (Func<long, bool>)(x => x < (long)firstSegment)), (Func<long, Tuple<int, string>>)(x => new Tuple<int, string>((int)x, string.Empty))), Observable.Where<Tuple<int, string>>(observable, (Func<Tuple<int, string>, bool>)(x => x.Item1 > 0))), observable);
        //    }));
        //}
    }
}
