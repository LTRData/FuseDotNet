using System;
using System.Diagnostics;

namespace FuseDotNet.Logging;

/// <summary>
/// Ignore all log messages.
/// </summary>
public class NullLogger : ILogger
{
    /// <inheritdoc />
    public bool DebugEnabled => false;

    /// <inheritdoc />
    public void Debug(FormattableString message)
    {
    }

    /// <inheritdoc />
    public void Error(FormattableString message) => Trace.WriteLine(message?.ToString());

    /// <inheritdoc />
    public void Fatal(FormattableString message) => Trace.WriteLine(message?.ToString());

    /// <inheritdoc />
    public void Info(FormattableString message)
    {
    }

    /// <inheritdoc />
    public void Warn(FormattableString message)
    {
    }
}
