using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
namespace CathedralLibrary.Services;
public sealed record LibraryEvent(long Id, DateTime AtUtc, string Kind);
public sealed class LibraryEventFeed
{
    private long _sequence;
    private readonly ConcurrentQueue<LibraryEvent> _recent = new();
    private readonly object _gate = new();
    private TaskCompletionSource<long> _changed = NewSignal();
    private static TaskCompletionSource<long> NewSignal() => new(TaskCreationOptions.RunContinuationsAsynchronously);
    public long Current => Interlocked.Read(ref _sequence);
    public LibraryEvent[] Since(long after) => _recent.Where(x => x.Id > after).ToArray();
    public void Publish(string kind = "library-changed")
    {
        TaskCompletionSource<long> previous;
        LibraryEvent evt;
        lock (_gate)
        {
            var id = Interlocked.Increment(ref _sequence);
            evt = new LibraryEvent(id, DateTime.UtcNow, kind);
            _recent.Enqueue(evt);
            while (_recent.Count > 100) _recent.TryDequeue(out _);
            previous = _changed;
            _changed = NewSignal();
        }
        previous.TrySetResult(evt.Id);
    }
    public async Task<long> WaitAsync(long after, TimeSpan timeout, CancellationToken ct)
    {
        Task<long> signal;
        lock (_gate)
        {
            if (Current > after) return Current;
            signal = _changed.Task;
        }
        try { return await signal.WaitAsync(timeout, ct); }
        catch (TimeoutException) { return Current; }
    }
}
