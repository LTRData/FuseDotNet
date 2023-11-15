using System;

namespace FuseDotNet.Logging;

/// <summary>
/// Log to the console.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
/// </remarks>
/// <param name="loggerName">Optional name to be added to each log line.</param>
public class ConsoleLogger(string loggerName = "") : ILogger
{
    private readonly string _loggerName = loggerName;

    /// <inheritdoc />        
    public bool DebugEnabled => true;

    /// <inheritdoc />
    public void Debug(FormattableString message) => WriteMessage(ConsoleColor.DarkCyan, message);

    /// <inheritdoc />
    public void Info(FormattableString message) => WriteMessage(ConsoleColor.Cyan, message);

    /// <inheritdoc />
    public void Warn(FormattableString message) => WriteMessage(ConsoleColor.DarkYellow, message);

    /// <inheritdoc />
    public void Error(FormattableString message) => WriteMessage(ConsoleColor.Red, message);

    /// <inheritdoc />
    public void Fatal(FormattableString message) => WriteMessage(ConsoleColor.Red, message);

    private void WriteMessage(ConsoleColor newColor, FormattableString message) => WriteMessage(
            message.FormatMessageForLogging(addDateTime: true, threadId: Environment.CurrentManagedThreadId, loggerName: _loggerName),
            newColor);

    private static readonly object _lock = new();

    private static void WriteMessage(string message, ConsoleColor newColor)
    {
        lock (_lock)
        {
            var origForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = newColor;
            Console.WriteLine(message);
            Console.ForegroundColor = origForegroundColor;
        }
    }
}
