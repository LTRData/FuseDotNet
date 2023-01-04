using System;
using System.Diagnostics;
using System.Globalization;

namespace FuseDotNet.Logging;

/// <summary>
/// Write all log messages to the <see cref="Trace"/>.
/// </summary>
public class TraceLogger : ILogger
{
    /// <inheritdoc />
    public bool DebugEnabled => true;

    /// <inheritdoc />
    public void Debug(FormattableString message) => Trace.TraceInformation(message.ToString(CultureInfo.InvariantCulture));

    /// <inheritdoc />
    public void Info(FormattableString message) => Trace.TraceInformation(message.ToString(CultureInfo.InvariantCulture));

    /// <inheritdoc />
    public void Warn(FormattableString message) => Trace.TraceWarning(message.ToString(CultureInfo.InvariantCulture));

    /// <inheritdoc />
    public void Error(FormattableString message) => Trace.TraceError(message.ToString(CultureInfo.InvariantCulture));

    /// <inheritdoc />
    public void Fatal(FormattableString message) => Trace.TraceError(message.ToString(CultureInfo.InvariantCulture));
}
