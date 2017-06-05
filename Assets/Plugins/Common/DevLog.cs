using UnityEngine;
using System.Collections;
using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;

namespace common
{

public class Debug
{
    //
    // Summary:
    //     ///
    //     Opens or closes developer console.
    //     ///
    public static bool developerConsoleVisible
    {
        get
        {
            return UnityEngine.Debug.developerConsoleVisible;
        }
        set
        {
            UnityEngine.Debug.developerConsoleVisible = value;
        }
    }

    //
    // Summary:
    //     ///
    //     In the Build Settings dialog there is a check box called "Development Build".
    //     ///
    public static bool isDebugBuild
    {
        get
        {
            return UnityEngine.Debug.isDebugBuild;
        }
    }

    //
    // Summary:
    //     ///
    //     Get default debug logger.
    //     ///
    public static ILogger logger
    {
        get
        {
            return UnityEngine.Debug.logger;
        }
    }

    //
    // Summary:
    //     ///
    //     Assert a condition and logs a formatted error message to the Unity console on
    //     failure.
    //     ///
    //
    // Parameters:
    //   condition:
    //     Condition you expect to be true.
    //
    //   context:
    //     Object to which the message applies.
    //
    //   message:
    //     String or object to be converted to string representation for display.
    [Conditional("USE_LOG_OUTPUT")]
    public static void Assert(bool condition)
    {
        UnityEngine.Debug.Assert(condition);
    }
    [Conditional("USE_LOG_OUTPUT")]
    public static void Assert(bool condition, string message)
    {
        UnityEngine.Debug.Assert(condition, message);
    }

    //
    // Summary:
    //     ///
    //     Assert a condition and logs a formatted error message to the Unity console on
    //     failure.
    //     ///
    //
    // Parameters:
    //   condition:
    //     Condition you expect to be true.
    //
    //   context:
    //     Object to which the message applies.
    //
    //   message:
    //     String or object to be converted to string representation for display.
    [Conditional("USE_LOG_OUTPUT")]
    public static void Assert(bool condition, object message)
    {
        UnityEngine.Debug.Assert(condition, message);
    }

    //
    // Summary:
    //     ///
    //     Assert a condition and logs a formatted error message to the Unity console on
    //     failure.
    //     ///
    //
    // Parameters:
    //   condition:
    //     Condition you expect to be true.
    //
    //   context:
    //     Object to which the message applies.
    //
    //   message:
    //     String or object to be converted to string representation for display.
    [Conditional("USE_LOG_OUTPUT")]
    public static void Assert(bool condition, UnityEngine.Object context)
    {
        UnityEngine.Debug.Assert(condition, context);
    }
    [Conditional("USE_LOG_OUTPUT")]
    public static void Assert(bool condition, string message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Assert(condition, message, context);
    }

    //
    // Summary:
    //     ///
    //     Assert a condition and logs a formatted error message to the Unity console on
    //     failure.
    //     ///
    //
    // Parameters:
    //   condition:
    //     Condition you expect to be true.
    //
    //   context:
    //     Object to which the message applies.
    //
    //   message:
    //     String or object to be converted to string representation for display.
    [Conditional("USE_LOG_OUTPUT")]
    public static void Assert(bool condition, object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Assert(condition, message, context);
    }

    //
    // Summary:
    //     ///
    //     Assert a condition and logs a formatted error message to the Unity console on
    //     failure.
    //     ///
    //
    // Parameters:
    //   condition:
    //     Condition you expect to be true.
    //
    //   format:
    //     A composite format string.
    //
    //   args:
    //     Format arguments.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void AssertFormat(bool condition, string format, params object[] args)
    {
        UnityEngine.Debug.AssertFormat(condition, format, args);
    }

    //
    // Summary:
    //     ///
    //     Assert a condition and logs a formatted error message to the Unity console on
    //     failure.
    //     ///
    //
    // Parameters:
    //   condition:
    //     Condition you expect to be true.
    //
    //   format:
    //     A composite format string.
    //
    //   args:
    //     Format arguments.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void AssertFormat(bool condition, UnityEngine.Object context, string format, params object[] args)
    {
        UnityEngine.Debug.AssertFormat(condition, context, format, args);
    }

	public static void AssertThrowException(bool condition, string message)
	{
		if (!condition)
			throw new Exception("[Assert] " + message);
	}

    //
    // Summary:
    //     ///
    //     Pauses the editor.
    //     ///
    public static void Break()
    {
        UnityEngine.Debug.Break();
    }

    //
    // Summary:
    //     ///
    //     Clears errors from the developer console.
    //     ///
    public static void ClearDeveloperConsole()
    {
        UnityEngine.Debug.ClearDeveloperConsole();
    }
    public static void DebugBreak()
    {
        UnityEngine.Debug.DebugBreak();
    }

    //
    // Summary:
    //     ///
    //     Draws a line between specified start and end points.
    //     ///
    //
    // Parameters:
    //   start:
    //     Point in world space where the line should start.
    //
    //   end:
    //     Point in world space where the line should end.
    //
    //   color:
    //     Color of the line.
    //
    //   duration:
    //     How long the line should be visible for.
    //
    //   depthTest:
    //     Should the line be obscured by objects closer to the camera?
    public static void DrawLine(Vector3 start, Vector3 end)
    {
        UnityEngine.Debug.DrawLine(start, end);
    }

    //
    // Summary:
    //     ///
    //     Draws a line between specified start and end points.
    //     ///
    //
    // Parameters:
    //   start:
    //     Point in world space where the line should start.
    //
    //   end:
    //     Point in world space where the line should end.
    //
    //   color:
    //     Color of the line.
    //
    //   duration:
    //     How long the line should be visible for.
    //
    //   depthTest:
    //     Should the line be obscured by objects closer to the camera?
    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        UnityEngine.Debug.DrawLine(start, end, color);
    }

    //
    // Summary:
    //     ///
    //     Draws a line between specified start and end points.
    //     ///
    //
    // Parameters:
    //   start:
    //     Point in world space where the line should start.
    //
    //   end:
    //     Point in world space where the line should end.
    //

    //   color:
    //     Color of the line.
    //
    //   duration:
    //     How long the line should be visible for.
    //
    //   depthTest:
    //     Should the line be obscured by objects closer to the camera?
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration);
    }

    //
    // Summary:
    //     ///
    //     Draws a line between specified start and end points.
    //     ///
    //
    // Parameters:
    //   start:
    //     Point in world space where the line should start.
    //
    //   end:
    //     Point in world space where the line should end.
    //
    //   color:
    //     Color of the line.
    //
    //   duration:
    //     How long the line should be visible for.
    //
    //   depthTest:
    //     Should the line be obscured by objects closer to the camera?
    public static void DrawLine(Vector3 start, Vector3 end, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
    }

    //
    // Summary:
    //     ///
    //     Draws a line from start to start + dir in world coordinates.
    //     ///
    //
    // Parameters:
    //   start:
    //     Point in world space where the ray should start.
    //
    //   dir:
    //     Direction and length of the ray.
    //
    //   color:
    //     Color of the drawn line.
    //
    //   duration:
    //     How long the line will be visible for (in seconds).
    //
    //   depthTest:
    //     Should the line be obscured by other objects closer to the camera?
    public static void DrawRay(Vector3 start, Vector3 dir)
    {
        UnityEngine.Debug.DrawRay(start, dir);
    }

    //
    // Summary:
    //     ///
    //     Draws a line from start to start + dir in world coordinates.
    //     ///
    //
    // Parameters:
    //   start:
    //     Point in world space where the ray should start.
    //
    //   dir:
    //     Direction and length of the ray.
    //
    //   color:
    //     Color of the drawn line.
    //
    //   duration:
    //     How long the line will be visible for (in seconds).
    //
    //   depthTest:
    //     Should the line be obscured by other objects closer to the camera?
    public static void DrawRay(Vector3 start, Vector3 dir, Color color)
    {
        UnityEngine.Debug.DrawRay(start, dir, color);
    }

    //
    // Summary:
    //     ///
    //     Draws a line from start to start + dir in world coordinates.
    //     ///
    //
    // Parameters:
    //   start:
    //     Point in world space where the ray should start.
    //
    //   dir:
    //     Direction and length of the ray.
    //
    //   color:
    //     Color of the drawn line.
    //
    //   duration:
    //     How long the line will be visible for (in seconds).
    //
    //   depthTest:
    //     Should the line be obscured by other objects closer to the camera?
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
    {
        UnityEngine.Debug.DrawRay(start, dir, color, duration);
    }

    //
    // Summary:
    //     ///
    //     Draws a line from start to start + dir in world coordinates.
    //     ///
    //
    // Parameters:
    //   start:
    //     Point in world space where the ray should start.
    //
    //   dir:
    //     Direction and length of the ray.
    //
    //   color:
    //     Color of the drawn line.
    //
    //   duration:
    //     How long the line will be visible for (in seconds).
    //
    //   depthTest:
    //     Should the line be obscured by other objects closer to the camera?
    public static void DrawRay(Vector3 start, Vector3 dir, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
    {
        UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
    }

    //
    // Summary:
    //     ///
    //     Logs message to the Unity Console.
    //     ///
    //
    // Parameters:
    //   message:
    //     String or object to be converted to string representation for display.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    //
    // Summary:
    //     ///
    //     Logs message to the Unity Console.
    //     ///
    //
    // Parameters:
    //   message:
    //     String or object to be converted to string representation for display.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void Log(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }

    //
    // Summary:
    //     ///
    //     A variant of Debug.Log that logs an assertion message to the console.
    //     ///
    //
    // Parameters:
    //   message:
    //     String or object to be converted to string representation for display.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogAssertion(object message)
    {
        UnityEngine.Debug.LogAssertion(message);
    }

    //
    // Summary:
    //     ///
    //     A variant of Debug.Log that logs an assertion message to the console.
    //     ///
    //
    // Parameters:
    //   message:
    //     String or object to be converted to string representation for display.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogAssertion(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogAssertion(message, context);
    }

    //
    // Summary:
    //     ///
    //     Logs a formatted assertion message to the Unity console.
    //     ///
    //
    // Parameters:
    //   format:
    //     A composite format string.
    //
    //   args:
    //     Format arguments.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogAssertionFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogAssertionFormat(format, args);
    }

    //
    // Summary:
    //     ///
    //     Logs a formatted assertion message to the Unity console.
    //     ///
    //
    // Parameters:
    //   format:
    //     A composite format string.
    //
    //   args:
    //     Format arguments.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogAssertionFormat(UnityEngine.Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogAssertionFormat(context, format, args);
    }

    //
    // Summary:
    //     ///
    //     A variant of Debug.Log that logs an error message to the console.
    //     ///
    //
    // Parameters:
    //   message:
    //     String or object to be converted to string representation for display.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    //
    // Summary:
    //     ///
    //     A variant of Debug.Log that logs an error message to the console.
    //     ///
    //
    // Parameters:
    //   message:
    //     String or object to be converted to string representation for display.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogError(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }

    //
    // Summary:
    //     ///
    //     Logs a formatted error message to the Unity console.
    //     ///
    //
    // Parameters:
    //   format:
    //     A composite format string.
    //
    //   args:
    //     Format arguments.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogErrorFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(format, args);
    }

    //
    // Summary:
    //     ///
    //     Logs a formatted error message to the Unity console.
    //     ///
    //
    // Parameters:
    //   format:
    //     A composite format string.
    //
    //   args:
    //     Format arguments.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(context, format, args);
    }

    //
    // Summary:
    //     ///
    //     A variant of Debug.Log that logs an error message to the console.
    //     ///
    //
    // Parameters:
    //   context:
    //     Object to which the message applies.
    //
    //   exception:
    //     Runtime Exception.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogException(Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }

    //
    // Summary:
    //     ///
    //     A variant of Debug.Log that logs an error message to the console.
    //     ///
    //
    // Parameters:
    //   context:
    //     Object to which the message applies.
    //
    //   exception:
    //     Runtime Exception.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogException(Exception exception, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogException(exception, context);
    }

    //
    // Summary:
    //     ///
    //     Logs a formatted message to the Unity Console.
    //     ///
    //
    // Parameters:
    //   format:
    //     A composite format string.
    //
    //   args:
    //     Format arguments.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(format, args);
    }

    //
    // Summary:
    //     ///
    //     Logs a formatted message to the Unity Console.
    //     ///
    //
    // Parameters:
    //   format:
    //     A composite format string.
    //
    //   args:
    //     Format arguments.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(context, format, args);
    }

    //
    // Summary:
    //     ///
    //     A variant of Debug.Log that logs a warning message to the console.
    //     ///
    //
    // Parameters:
    //   message:
    //     String or object to be converted to string representation for display.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    //
    // Summary:
    //     ///
    //     A variant of Debug.Log that logs a warning message to the console.
    //     ///
    //
    // Parameters:
    //   message:
    //     String or object to be converted to string representation for display.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogWarning(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogWarning(message, context);
    }

    //
    // Summary:
    //     ///
    //     Logs a formatted warning message to the Unity Console.
    //     ///
    //
    // Parameters:
    //   format:
    //     A composite format string.
    //
    //   args:
    //     Format arguments.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogWarningFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(format, args);
    }

    //
    // Summary:
    //     ///
    //     Logs a formatted warning message to the Unity Console.
    //     ///
    //
    // Parameters:
    //   format:
    //     A composite format string.
    //
    //   args:
    //     Format arguments.
    //
    //   context:
    //     Object to which the message applies.
    [Conditional("USE_LOG_OUTPUT")]
    public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(context, format, args);
    }


    // Always output log, even in release version.
    public static void LogAlways(string msg)
    {
        UnityEngine.Debug.Log(msg);
    }

    public static void LogWarningAlways(string msg)
    {
        UnityEngine.Debug.LogWarning(msg);
    }

    public static void LogErrorAlways(string msg)
    {
        UnityEngine.Debug.LogError(msg);
    }
}

}