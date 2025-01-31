﻿namespace NAPS2.Logging;

/// <summary>
/// A base interface for logging APIs. Used by the Log class.
/// </summary>
public interface ILogger
{
    void Info(string message);
    void Error(string message);
    void ErrorException(string message, Exception exception);
    void FatalException(string message, Exception exception);
}