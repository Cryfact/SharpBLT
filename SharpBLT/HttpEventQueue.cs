namespace SharpBLT;

public struct HttpEventData
{
    internal int id;
    internal IntPtr L;
    internal int functionReference;
    internal int progressReference;
    internal string url;
}

internal class HttpEventQueue : Singleton<HttpEventQueue>
{
    private readonly Queue<Tuple<Action<HttpEventData, long, long>, HttpEventData, long, long>> _progressQueue = new();
    private readonly Queue<Tuple<Action<HttpEventData, byte[]>, HttpEventData, byte[]>> _doneQueue = new();

    private readonly object _lock = new();

    public void QueueProgressEvent(Action<HttpEventData, long, long> action, HttpEventData data, long progress, long total)
    {
        lock (_lock)
        {
            _progressQueue.Enqueue(new(action, data, progress, total));
        }
    }

    public void QueueDoneEvent(Action<HttpEventData, byte[]> action, HttpEventData data, byte[] result)
    {
        lock (_lock)
        {
            _doneQueue.Enqueue(new(action, data, result));
        }
    }

    public void ProcessEvents()
    {
        lock (_lock)
        {
            while (_progressQueue.TryDequeue(out Tuple<Action<HttpEventData, long, long>, HttpEventData, long, long>? tuple))
            {
                tuple.Item1.Invoke(tuple.Item2, tuple.Item3, tuple.Item4);
            }
            while (_doneQueue.TryDequeue(out Tuple<Action<HttpEventData, byte[]>, HttpEventData, byte[]>? tuple))
            {
                tuple.Item1.Invoke(tuple.Item2, tuple.Item3);
            }
        }
    }
}
