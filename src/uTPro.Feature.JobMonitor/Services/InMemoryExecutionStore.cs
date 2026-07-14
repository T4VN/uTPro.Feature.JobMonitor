using System.Collections.Concurrent;
using uTPro.Feature.JobMonitor.Models;

namespace uTPro.Feature.JobMonitor.Services;

/// <summary>
/// In-memory telemetry store backed by a bounded per-job ring buffer.
/// Records live for the lifetime of the host process and reset on restart.
/// </summary>
public sealed class InMemoryExecutionStore : IExecutionStore
{
    private const int DefaultCapacity = 50;

    private readonly int _capacity;
    private readonly ConcurrentDictionary<string, JobHistory> _histories = new(StringComparer.Ordinal);

    public InMemoryExecutionStore() : this(DefaultCapacity) { }

    public InMemoryExecutionStore(int capacity)
        => _capacity = capacity > 0 ? capacity : DefaultCapacity;

    public StorageMode Mode => StorageMode.InMemory;

    public void Add(ExecutionRecord record)
    {
        var history = _histories.GetOrAdd(record.JobKey, _ => new JobHistory(_capacity));
        history.Add(record);
    }

    public void Update(ExecutionRecord record)
    {
        if (_histories.TryGetValue(record.JobKey, out var history))
        {
            history.Replace(record);
        }
    }

    public ExecutionRecord? GetLatest(string jobKey)
        => _histories.TryGetValue(jobKey, out var history) ? history.Latest() : null;

    public IReadOnlyList<ExecutionRecord> GetHistory(string jobKey)
        => _histories.TryGetValue(jobKey, out var history) ? history.Snapshot() : Array.Empty<ExecutionRecord>();

    /// <summary>A bounded, most-recent-first buffer of records for one job.</summary>
    private sealed class JobHistory(int capacity)
    {
        private readonly int _capacity = capacity;
        private readonly LinkedList<ExecutionRecord> _items = new();
        private readonly Lock _gate = new();

        public void Add(ExecutionRecord record)
        {
            lock (_gate)
            {
                _items.AddFirst(record);
                while (_items.Count > _capacity)
                {
                    _items.RemoveLast();
                }
            }
        }

        public void Replace(ExecutionRecord record)
        {
            lock (_gate)
            {
                for (var node = _items.First; node is not null; node = node.Next)
                {
                    if (node.Value.RunId == record.RunId)
                    {
                        node.Value = record;
                        return;
                    }
                }
            }
        }

        public ExecutionRecord? Latest()
        {
            lock (_gate)
            {
                return _items.First?.Value;
            }
        }

        public IReadOnlyList<ExecutionRecord> Snapshot()
        {
            lock (_gate)
            {
                return _items.ToArray();
            }
        }
    }
}
