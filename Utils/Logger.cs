using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

///A custom logger
public static class Logger
{

    ///A message that is logged
    public struct Message
    {
        private System.WeakReference<object> _contextRef;
        public object context
        {
            get
            {
                object reference = null;
                if (_contextRef != null) { _contextRef.TryGetTarget(out reference); }
                return reference;
            }
            set { _contextRef = new System.WeakReference<object>(value); }
        }

        public LogType type;
        public string text;
        public string tag;

        public bool IsValid() { return !string.IsNullOrEmpty(text); }
    }

    public delegate bool LogHandler(Message message);
    private static List<LogHandler> subscribers = new List<LogHandler>();

    ///----------------------------------------------------------------------------------------------

    ///Subscribe a listener to the logger
    public static void AddListener(LogHandler callback) { subscribers.Add(callback); }
    ///Remove a listener from the logger
    public static void RemoveListener(LogHandler callback) { subscribers.Remove(callback); }

    ///----------------------------------------------------------------------------------------------

    ///Log Info
    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void Log(object message, string tag = null, object context = null)
    {
        Internal_Log(LogType.Log, message, tag, context);
    }

    ///Log Warning
    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void LogWarning(object message, string tag = null, object context = null)
    {
        Internal_Log(LogType.Warning, message, tag, context);
    }

    ///Log Error
    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void LogError(object message, string tag = null, object context = null)
    {
        Internal_Log(LogType.Error, message, tag, context);
    }

    ///Log Exception
    public static void LogException(System.Exception exception, string tag = null, object context = null)
    {
        Internal_Log(LogType.Exception, exception, tag, context);
    }

    ///----------------------------------------------------------------------------------------------

    //...
    private static void Internal_Log(LogType type, object message, string tag, object context)
    {
        if (subscribers != null && subscribers.Count > 0)
        {
            var msg = new Message();
            msg.type = type;
            if (message is System.Exception)
            {
                var exc = (System.Exception)message;
                msg.text = exc.Message + "\n" + exc.StackTrace.Split('\n').FirstOrDefault();
            }
            else
            {
                msg.text = message != null ? message.ToString() : "NULL";
            }
            msg.tag = tag;
            msg.context = context;
            var handled = false;
            foreach (var call in subscribers)
            {
                if (call(msg))
                {
                    handled = true;
                    break;
                }
            }
            //if log is handled, don't forward to unity console unless its an exception
            if (handled && type != LogType.Exception)
            {
                return;
            }
        }

        if (!string.IsNullOrEmpty(tag))
        {
            tag = string.Format("<b>({0} {1})</b>", tag, type.ToString());
        }
        else { tag = string.Format("<b>({0})</b>", type.ToString()); }

#if UNITY_EDITOR
        if (!Threader.isMainThread)
        {
            UnityEditor.EditorApplication.delayCall += () => { ForwardToUnity(type, message, tag, context); };
            return;
        }
#endif

        ForwardToUnity(type, message, tag, context);
    }

    //forward the log to unity console
    private static void ForwardToUnity(LogType type, object message, string tag, object context)
    {
        if (message is System.Exception)
        {
            UnityEngine.Debug.unityLogger.LogException((System.Exception)message);
        }
        else
        {
            if (type == LogType.Error)
                UnityEngine.Debug.unityLogger.Log(LogType.Log, tag, $"Error: <color=red>{message}</color>", context as UnityEngine.Object);
            else
                UnityEngine.Debug.unityLogger.Log(type, tag, message, context as UnityEngine.Object);
        }
    }
}