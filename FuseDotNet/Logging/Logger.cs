using System;

namespace FuseDotNet.Logging;

/// <summary>
/// Handle log messages with callbacks
/// </summary>
/// <param name="debug">An <see cref="Action{T}"/> that are called when a debug log message are arriving.</param>
/// <param name="info">An <see cref="Action{T}"/> that are called when a information log message are arriving</param>
/// <param name="warn">An <see cref="Action{T}"/> that are called when a warning log message are arriving</param>
/// <param name="error">An <see cref="Action{T}"/> that are called when a error log message are arriving</param>
/// <param name="fatal">An <see cref="Action{T}"/> that are called when a fatal error log message are arriving</param>
public class Logger(
    Action<string, object[]> debug,
    Action<string, object[]> info,
    Action<string, object[]> warn,
    Action<string, object[]> error,
    Action<string, object[]> fatal) : ILogger
{

    /// <inheritdoc />
    public bool DebugEnabled => debug != null;

    /// <inheritdoc />
    public void Debug(string message, params object[] args) => debug(message, args);

    /// <inheritdoc />
    public void Info(string message, params object[] args) => info(message, args);

    /// <inheritdoc />
    public void Warn(string message, params object[] args) => warn(message, args);

    /// <inheritdoc />
    public void Error(string message, params object[] args) => error(message, args);

    /// <inheritdoc />
    public void Fatal(string message, params object[] args) => fatal(message, args);
}