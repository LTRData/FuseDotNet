using System;

namespace FuseDotNet.Logging;

/// <summary>
/// Handle log messages with callbacks
/// </summary>
public class Logger : ILogger
{
    private readonly Action<FormattableString> _debug;
    private readonly Action<FormattableString> _info;
    private readonly Action<FormattableString> _warn;
    private readonly Action<FormattableString> _error;
    private readonly Action<FormattableString> _fatal;

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger"/> class.
    /// </summary>
    /// <param name="debug">An <see cref="Action{FormattableString}"/> that are called when a debug log message are arriving.</param>
    /// <param name="info">An <see cref="Action{FormattableString}"/> that are called when a information log message are arriving</param>
    /// <param name="warn">An <see cref="Action{FormattableString}"/> that are called when a warning log message are arriving</param>
    /// <param name="error">An <see cref="Action{FormattableString}"/> that are called when a error log message are arriving</param>
    /// <param name="fatal">An <see cref="Action{FormattableString}"/> that are called when a fatal error log message are arriving</param>
    public Logger(
        Action<FormattableString> debug,
        Action<FormattableString> info,
        Action<FormattableString> warn,
        Action<FormattableString> error,
        Action<FormattableString> fatal)
    {
        _debug = debug;
        _info = info;
        _warn = warn;
        _error = error;
        _fatal = fatal;
    }

    /// <inheritdoc />
    public bool DebugEnabled => _debug != null;

    /// <inheritdoc />
    public void Debug(FormattableString message) => _debug(message);

    /// <inheritdoc />
    public void Info(FormattableString message) => _info(message);

    /// <inheritdoc />
    public void Warn(FormattableString message) => _warn(message);

    /// <inheritdoc />
    public void Error(FormattableString message) => _error(message);

    /// <inheritdoc />
    public void Fatal(FormattableString message) => _fatal(message);
}
