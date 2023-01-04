using System;
using System.Globalization;
using System.Text;

namespace FuseDotNet.Logging;

/// <summary>
/// Extension functions to log messages.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Format a log message.
    /// </summary>
    /// <param name="message">Message to format.</param>
    /// <param name="category">Optional category to add to the log message.</param>
    /// <param name="loggerName">Optional log name to at to the log message.</param>
    /// <returns>A formated log message.</returns>
    public static string FormatMessageForLogging(
        this FormattableString message,
        string? category = null,
        string loggerName = "") => message.FormatMessageForLogging(addDateTime: false, threadId: null, category, loggerName);

    /// <summary>
    /// Format a log message.
    /// </summary>
    /// <param name="message">Message to format.</param>
    /// <param name="addDateTime">If date and time shout be added to the log message.</param>
    /// <param name="category">Optional category to add to the log message.</param>
    /// <param name="loggerName">Optional log name to at to the log message.</param>
    /// <returns>A formated log message.</returns>
    public static string FormatMessageForLogging(
        this FormattableString message,
        bool addDateTime = false,
        int? threadId = null,
        string? category = null,
        string loggerName = "")
    {
        var stringBuilder = new StringBuilder();

        if (addDateTime)
        {
            stringBuilder.AppendFormat("{0} - ", DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        if (threadId.HasValue)
        {
            stringBuilder.Append($"[{threadId.Value:###0}] ");
        }

        if (!string.IsNullOrEmpty(loggerName))
        {
            stringBuilder.Append(loggerName).Append(' ');
        }

        if (!string.IsNullOrEmpty(category))
        {
            stringBuilder.Append(category).Append(' ');
        }

        stringBuilder.Append(message.ToString(CultureInfo.InvariantCulture));

        return stringBuilder.ToString();
    }
}
