
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace DXVcs2Git.Core.Git {
    public class ObservableProcess : IObservableProcess {
        readonly object gate = (object)42;
        readonly AsyncSubject<Unit> exit = new AsyncSubject<Unit>();
        readonly ReplaySubject<string> output = new ReplaySubject<string>();
        readonly ReplaySubject<string> combinedOutput = new ReplaySubject<string>();
        readonly ReplaySubject<string> error = new ReplaySubject<string>();
        const int processTimeoutMilliseconds = 3600000;
        readonly IObserver<string> input;

        public IObserver<string> Input
        {
            get
            {
                return this.input;
            }
        }

        public IObservable<string> CombinedOutput
        {
            get
            {
                return (IObservable<string>)this.combinedOutput;
            }
        }

        public IObservable<string> Output
        {
            get
            {
                return (IObservable<string>)this.output;
            }
        }

        public IObservable<string> Error
        {
            get
            {
                return (IObservable<string>)this.error;
            }
        }

        public ObservableProcess(ProcessStartInfo startInfo, bool throwOnNonZeroExitCode = true)
          : this(new ProcessWrapper(startInfo), throwOnNonZeroExitCode) {
        }

        public ObservableProcess(ProcessWrapper process, bool throwOnNonZeroExitCode) {
            ObservableProcess observableProcess = this;
            process.OutputDataReceived += new DataReceivedEventHandler(this.OnReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(this.OnErrorReceived);
            try {
                process.Start();
            }
            catch (Exception ex) {
                this.error.OnNext(ex.Message);
                this.exit.OnError(ex);
                this.input = (IObserver<string>)new ReplaySubject<string>();
                this.Cleanup(process);
                return;
            }
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            input = Observer.Create<string>(x => {
                try {
                    process.StandardInput.WriteLine(x);
                    process.StandardInput.Flush();
                }
                catch (Exception ex) {
                    Log.Error(@"Error occurred while trying to write to the standard input", ex);
                    if (ex.IsCriticalException())
                        throw;
                    else
                        ObservableProcess.LogProcessError(process.GetProcessLogInfo(), ex, process);
                }
            }, () => { });
            Observable.Start(() => observableProcess.ObserveProcess(process, throwOnNonZeroExitCode));
        }

        private void ObserveProcess(ProcessWrapper process, bool throwOnNonZeroExitCode) {
            try {
                bool completed = false;
                int exitCode = this.GetExitCode(process, ref completed);
                if (exitCode != 0 && throwOnNonZeroExitCode) {
                    this.exit.OnError(completed
                        ? (Exception)new ProcessException(exitCode, string.Join("\n", TaskObservableExtensions.ToTask<string[]>(Observable.ToArray<string>((IObservable<string>)this.combinedOutput)).Result))
                        : (Exception)new ProcessTimeoutException(3600000));
                }
                else {
                    this.exit.OnNext(Unit.Default);
                    this.exit.OnCompleted();
                }
            }
            catch (Exception ex) {
                ObservableProcess.LogProcessError("Failed to Observer Process", ex, process);
                throw;
            }
        }

        private int GetExitCode(ProcessWrapper process, ref bool completed) {
            try {
                //if (LoggerExtensions.GetResultOrWarnException<bool>(ObservableProcess.log, (Func<bool>)(() => process.WaitForExit(3600000)), false, "Failed to wait for exit: " + ProcessExtensions.GetProcessLogInfo(process))) {
                //    completed = true;
                //    return process.ExitCode;
                //}
                //LoggerExtensions.DoOrWarnException(ObservableProcess.log, new Action(process.KillProcessTree), "Failed to kill the process tree." + ProcessExtensions.GetProcessLogInfo(process));
                this.error.OnNext("Process timed out");
                return -1;
            }
            finally {
                this.Cleanup(process);
            }
        }

        private void Cleanup(ProcessWrapper process) {
            this.output.OnCompleted();
            this.error.OnCompleted();
            this.combinedOutput.OnCompleted();
            process.OutputDataReceived -= new DataReceivedEventHandler(this.OnReceived);
            process.ErrorDataReceived -= new DataReceivedEventHandler(this.OnReceived);
            try {
                process.Close();
            }
            catch (Exception ex) {
                if (!ex.IsCriticalException())
                    return;
                Log.Error(string.Format(CultureInfo.InvariantCulture, "ASSERT! Error closing the process '{0}'", new object[] { process.GetProcessLogInfo() }), ex);
                throw;
            }
        }

        public IDisposable Subscribe(IObserver<Unit> observer) {
            return this.exit.Subscribe(observer);
        }

        private void OnReceived(object s, DataReceivedEventArgs e) {
            if (e.Data == null)
                return;
            this.NotifyData((IObserver<string>)this.output, e.Data);
        }

        private void OnErrorReceived(object s, DataReceivedEventArgs e) {
            if (e.Data == null)
                return;
            this.NotifyData((IObserver<string>)this.error, e.Data);
        }

        private void NotifyData(IObserver<string> observable, string data) {
            lock (this.gate) {
                observable.OnNext(data);
                this.combinedOutput.OnNext(data);
            }
        }

        private static void LogProcessError(string message, Exception exception, ProcessWrapper process) {
            string message1 = string.Format((IFormatProvider)CultureInfo.InvariantCulture, "ASSERT: " + message + " " + process.GetProcessLogInfo());
            Log.Error(message1, exception);
        }
    }
}


