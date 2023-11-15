using System;
using System.Diagnostics;

namespace FuseDotNet.Logging;

/// <summary>
/// Write log using OutputDebugString 
/// </summary>
/// <remarks>
/// To see the output in visual studio 
/// Project + %Properties, Debug tab, check "Enable unmanaged code debugging".
/// </remarks> 
/// <remarks>
/// Initializes a new instance of the <see cref="DebugViewLogger"/> class.
/// </remarks>
/// <param name="loggerName">Optional name to be added to each log line.</param>
public class DebugViewLogger(string loggerName = "") : ILogger
{
    private readonly string _loggerName = loggerName;

    /// <inheritdoc />
    public bool DebugEnabled => true;

    /// <inheritdoc />
    public void Debug(FormattableString message) => WriteMessageToDebugView("debug", message);

    /// <inheritdoc />
    public void Info(FormattableString message) => WriteMessageToDebugView("info", message);

    /// <inheritdoc />
    public void Warn(FormattableString message) => WriteMessageToDebugView("warn", message);

    /// <inheritdoc />
    public void Error(FormattableString message) => WriteMessageToDebugView("error", message);

    /// <inheritdoc />
    public void Fatal(FormattableString message) => WriteMessageToDebugView("fatal", message);

    private void WriteMessageToDebugView(string category, FormattableString message) =>
        Trace.WriteLine(message.FormatMessageForLogging(category, _loggerName));
}
