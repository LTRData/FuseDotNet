using FuseDotNet.Native;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace FuseDotNet;

public class FuseService : IDisposable
{
    public event EventHandler? Stopped;

    public event EventHandler<ThreadExceptionEventArgs>? Error;

    public IFuseOperations Operations { get; }

    public string? MountPoint => _args.LastOrDefault();

    private readonly string[] _args;

    public bool Running => ServiceThread?.IsAlive ?? false;

    protected Thread? ServiceThread { get; private set; }

    public FuseService(IFuseOperations operations, string[] args)
    {
        Operations = operations;
        _args = args;
    }

    public void Start()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }

        ServiceThread = new(ServiceThreadProcedure)
        {
            Name = "FuseService"
        };

        ServiceThread.Start();
    }

    private void ServiceThreadProcedure()
    {
        try
        {
            Operations.Mount(_args);

            OnDismounted(EventArgs.Empty);
        }
        catch (Exception ex)
        {
            OnError(new(ex));
        }
        finally
        {
            (Operations as IDisposable)?.Dispose();
        }
    }

    public void WaitExit()
    {
        if (ServiceThread == null ||
            ServiceThread.ManagedThreadId == Environment.CurrentManagedThreadId)
        {
            return;
        }

        ServiceThread.Join();
    }

    protected virtual void OnError(ThreadExceptionEventArgs e) => Error?.Invoke(this, e);

    protected virtual void OnDismounted(EventArgs e) => Stopped?.Invoke(this, e);

    #region IDisposable Support
    public bool IsDisposed => is_disposed != 0;

    int is_disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (Interlocked.Exchange(ref is_disposed, 1) == 0)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                if (ServiceThread != null && ServiceThread.IsAlive &&
                    MountPoint != null && !string.IsNullOrWhiteSpace(MountPoint))
                {
                    Trace.WriteLine($"Requesting dismount for Fuse file system '{MountPoint}'");

                    _ = NativeMethods.unmount(MountPoint, 0);

                    if (ServiceThread.ManagedThreadId != Environment.CurrentManagedThreadId)
                    {
                        Trace.WriteLine($"Waiting for Fuse file system '{MountPoint}' service thread to stop");

                        ServiceThread.Join();

                        Trace.WriteLine($"Fuse file system '{MountPoint}' service thread stopped.");
                    }
                }

                (Operations as IDisposable)?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.

            // TODO: set large fields to null.
            ServiceThread = null;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    ~FuseService()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        GC.SuppressFinalize(this);
    }
    #endregion
}
