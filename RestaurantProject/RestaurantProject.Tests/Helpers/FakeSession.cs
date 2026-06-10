using Microsoft.AspNetCore.Http;

namespace Restaurant.Tests.Helpers;

/// <summary>
/// Minimal in-memory ISession for testing the session-backed cart
/// without spinning up the full ASP.NET pipeline.
/// </summary>
public class FakeSession : ISession
{
    private readonly Dictionary<string, byte[]> _store = new();

    public bool IsAvailable => true;
    public string Id => "fake-session";
    public IEnumerable<string> Keys => _store.Keys;

    public void Clear() => _store.Clear();
    public Task CommitAsync(CancellationToken token = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken token = default) => Task.CompletedTask;
    public void Remove(string key) => _store.Remove(key);
    public void Set(string key, byte[] value) => _store[key] = value;

    public bool TryGetValue(string key, out byte[] value)
    {
        return _store.TryGetValue(key, out value!);
    }
}
