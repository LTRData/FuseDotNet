using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FuseDotNet.Logging;

/// <summary>
/// Write log using OutputDebugString 
/// </summary>
/// <remarks>
/// To see the output in visual studio 
/// Project + %Properties, Debug tab, check "Enable unmanaged code debugging".
/// </remarks> 
public class DebugViewLogger : ILogger
{
    private readonly string _loggerName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugViewLogger"/> class.
    /// </summary>
    /// <param name="loggerName">Optional name to be added to each log line.</param>
    public DebugViewLogger(string loggerName = "")
    {
        _loggerName = loggerName;
    }

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
