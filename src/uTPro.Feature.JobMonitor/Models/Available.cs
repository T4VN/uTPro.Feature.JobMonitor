namespace uTPro.Feature.JobMonitor.Models;

/// <summary>
/// Represents a value that may not be readable (for example a Legacy_Job timing value).
/// A value is meaningful only when <see cref="IsAvailable"/> is <c>true</c>.
/// Serialized to the client as <c>{ "available": bool, "value": &lt;T|null&gt; }</c>.
/// </summary>
public readonly record struct Available<T>(bool IsAvailable, T? Value)
{
    /// <summary>An available value.</summary>
    public static Available<T> Of(T value) => new(true, value);

    /// <summary>An explicitly unavailable value.</summary>
    public static Available<T> Unavailable => new(false, default);
}
