// Generated from DevLog_Template by DevDebugEditor.
//

namespace dlc
{
    public class Debug
    {
        public static readonly string TAG = "DLC";
        private static common.DevLog devLog_;
        private static common.DevLog devLog
        {
            get
            {
                if (devLog_ == null)
                    devLog_ = new common.DevLog(TAG,
                    #if DLC_LOG
                        true
                    #else
                        false
                    #endif
                        );
                return devLog_;
            }
        }

		[System.Diagnostics.Conditional("USE_LOG_OUTPUT")]
		public static void Log(string msg)
		{
            devLog.Log(msg);
        }

        [System.Diagnostics.Conditional("USE_LOG_OUTPUT")]
        public static void LogWarning(string msg)
        {
            devLog.LogWarning(msg);
        }

        [System.Diagnostics.Conditional("USE_LOG_OUTPUT")]
        public static void LogError(string msg)
        {
            devLog.LogError(msg);
        }

        [System.Diagnostics.Conditional("USE_LOG_OUTPUT")]
        public static void Assert(bool condition, string msg)
        {
            devLog.Assert(condition, msg);
        }
    }
}

namespace abc
{
    public class Debug
    {
        public static readonly string TAG = "ABC";
        private static common.DevLog devLog_;
        private static common.DevLog devLog
        {
            get
            {
                if (devLog_ == null)
                    devLog_ = new common.DevLog(TAG,
                    #if ABC_LOG
                        true
                    #else
                        false
                    #endif
                        );
                return devLog_;
            }
        }

		[System.Diagnostics.Conditional("USE_LOG_OUTPUT")]
		public static void Log(string msg)
		{
            devLog.Log(msg);
        }

        [System.Diagnostics.Conditional("USE_LOG_OUTPUT")]
        public static void LogWarning(string msg)
        {
            devLog.LogWarning(msg);
        }

        [System.Diagnostics.Conditional("USE_LOG_OUTPUT")]
        public static void LogError(string msg)
        {
            devLog.LogError(msg);
        }

        [System.Diagnostics.Conditional("USE_LOG_OUTPUT")]
        public static void Assert(bool condition, string msg)
        {
            devLog.Assert(condition, msg);
        }
    }
}

