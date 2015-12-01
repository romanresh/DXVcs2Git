using System;
using System.Threading;

namespace DXVcs2Git.Core.Git {
    public sealed class SafeDisposable : IDisposable {
        int disposed;
        readonly IDisposable disposable;

        public SafeDisposable(IDisposable disposable) {
            this.disposable = disposable;
        }

        public void Dispose() {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return;
            this.disposable.Dispose();
        }
    }
}
