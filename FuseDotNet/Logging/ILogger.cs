using System;
/// <summary>
/// Namespace for interface and classes related to logging.
/// </summary>
namespace FuseDotNet.Logging;

/// <summary>
/// The %Logger interface.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Gets a value indicating whether the logger wishes to receive debug messages.
    /// </summary>
    bool DebugEnabled { get; }

    /// <summary>
    /// Log a debug message
    /// </summary>
    /// <param name="message">The message to write to the log</param>
    /// <param name="args">Arguments to use to format the <paramref name="message"/></param>
    void Debug(FormattableString message);

    /// <summary>
    /// Log a info message
    /// </summary>
    /// <param name="message">The message to write to the log</param>
    /// <param name="args">Arguments to use to format the <paramref name="message"/></param>
    void Info(FormattableString message);

    /// <summary>
    /// Log a warning message
    /// </summary>
    /// <param name="message">The message to write to the log</param>
    /// <param name="args">Arguments to use to format the <paramref name="message"/></param>
    void Warn(FormattableString message);

    /// <summary>
    /// Log a error message
    /// </summary>
    /// <param name="message">The message to write to the log</param>
    /// <param name="args">Arguments to use to format the <paramref name="message"/></param>
    void Error(FormattableString message);

    /// <summary>
    /// Log a fatal error message
    /// </summary>
    /// <param name="message">The message to write to the log</param>
    /// <param name="args">Arguments to use to format the <paramref name="message"/></param>
    void Fatal(FormattableString message);
}
