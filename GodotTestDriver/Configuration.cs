namespace GodotTestDriver;

using System;
using GodotTestDriver.Util;
using JetBrains.Annotations;

/// <summary>
/// Godot test driver configuration class.
/// </summary>
[PublicAPI]
public static class Configuration
{
    public static LoggingConfiguration Logging { get; } = new LoggingConfiguration();

    public class LoggingConfiguration
    {
        public static Action<string, object[]> DebugLogger
        {
            get => Log.DebugLogger;
            set => Log.DebugLogger = value;
        }

        public static Action<string, object[]> InfoLogger
        {
            get => Log.InfoLogger;
            set => Log.InfoLogger = value;
        }

        public static Action<string, object[]> ErrorLogger
        {
            get => Log.ErrorLogger;
            set => Log.ErrorLogger = value;
        }
    }
}
