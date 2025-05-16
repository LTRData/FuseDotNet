using System.Globalization;

namespace FuseDotNet.Logging;

/// <summary>
/// Write log using OutputDebugString 
/// </summary>
/// <remarks>
/// To see the output in debugger
/// In Visual Studio: Project + %Properties, Debug tab, check "Enable unmanaged code debugging".
/// </remarks> 
/// <param name="loggerName">Optional name to be added to each log line.</param>
/// <param name="dateTimeFormatInfo">An object that supplies format information for DateTime.</param>
public class DebugViewLogger(string loggerName = "", DateTimeFormatInfo? dateTimeFormatInfo = null) : ILogger
{
    /// <inheritdoc />
    public bool DebugEnabled => true;

    /// <inheritdoc />
    public void Debug(string message, params object[] args) => WriteMessageToDebugView("debug", message, args);

    /// <inheritdoc />
    public void Info(string message, params object[] args) => WriteMessageToDebugView("info", message, args);

    /// <inheritdoc />
    public void Warn(string message, params object[] args) => WriteMessageToDebugView("warn", message, args);

    /// <inheritdoc />
    public void Error(string message, params object[] args) => WriteMessageToDebugView("error", message, args);

    /// <inheritdoc />
    public void Fatal(string message, params object[] args) => WriteMessageToDebugView("fatal", message, args);

    private void WriteMessageToDebugView(string category, string message, params object[] args)
    {
        if (args?.Length > 0)
        {
            message = string.Format(message, args);
        }

        System.Diagnostics.Debug.WriteLine(message.FormatMessageForLogging(category, loggerName, dateTimeFormatInfo));
    }
}