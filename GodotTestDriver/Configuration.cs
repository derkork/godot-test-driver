using System;
using GodotTestDriver.Util;
using JetBrains.Annotations;

namespace GodotTestDriver
{
    /// <summary>
    /// Godot test driver configuration class.
    /// </summary>
    [PublicAPI]
    public static class Configuration
    {
        public static LoggingConfiguration Logging { get; } = new LoggingConfiguration();

        public partial class LoggingConfiguration
        {
            public Action<string, object[]> DebugLogger
            {
                get => Log.DebugLogger;
                set => Log.DebugLogger = value;
            }
            
            public Action<string, object[]> InfoLogger
            {
                get => Log.InfoLogger;
                set => Log.InfoLogger = value;
            }
            
            public Action<string, object[]> ErrorLogger
            {
                get => Log.ErrorLogger;
                set => Log.ErrorLogger = value;
            }
        }
    }
}