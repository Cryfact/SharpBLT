using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpBLT
{
    public enum LogType
    {
        Debug,
        Trace,
        Log,
        Lua,
        Warn,
        Error,
    }

    public class Logger : Singleton<Logger>
    {
        private readonly Stream m_logFileStream;
        private readonly StreamWriter m_writer;
        private readonly object m_lock;
        private Console? m_console;

        public LogType CurrentLogLevel { get; set; }

        public Logger()
        {
            CurrentLogLevel = LogType.Debug;

            string path = "mods/logs/";
            path += DateTime.Now;
            path += "_log.txt";

            m_logFileStream = new FileStream(path, FileMode.Append, FileAccess.ReadWrite);
            m_writer = new StreamWriter(m_logFileStream);
            m_lock = new object();
        }

        public virtual void Log(LogType logType, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
        {
            if (logType < CurrentLogLevel)
                return;

            var time = DateTime.Now;
            var strlogType = GetLogTypeString(logType);
            var msg = string.Format("{0} {1}: ({2}:{3}) {4}", time, strlogType, file, line, message);

            lock (m_lock)
            {
                if (m_logFileStream != null)
                    m_writer.WriteLine(msg);

                if (m_console != null)
                {
                    var hStdout = Kernel32.GetStdHandle(Kernel32.STD_OUTPUT_HANDLE);

                    switch (logType)
                    {
                        case LogType.Log:
                            Kernel32.SetConsoleTextAttribute(hStdout, Kernel32.FOREGROUND_BLUE | Kernel32.FOREGROUND_GREEN | Kernel32.FOREGROUND_INTENSITY);
                            break;
                        case LogType.Lua:
                            Kernel32.SetConsoleTextAttribute(hStdout, Kernel32.FOREGROUND_RED | Kernel32.FOREGROUND_BLUE | Kernel32.FOREGROUND_GREEN | Kernel32.FOREGROUND_INTENSITY);
                            break;
                        case LogType.Warn:
                            Kernel32.SetConsoleTextAttribute(hStdout, Kernel32.FOREGROUND_RED | Kernel32.FOREGROUND_GREEN | Kernel32.FOREGROUND_INTENSITY);
                            break;
                        case LogType.Error:
                            Kernel32.SetConsoleTextAttribute(hStdout, Kernel32.FOREGROUND_RED | Kernel32.FOREGROUND_INTENSITY);
                            break;
                        default:
                            Kernel32.SetConsoleTextAttribute(hStdout, Kernel32.FOREGROUND_RED | Kernel32.FOREGROUND_BLUE | Kernel32.FOREGROUND_GREEN | Kernel32.FOREGROUND_INTENSITY);
                            break;
                    }

                    System.Console.WriteLine(msg);
                }
            }
        }

        public void OpenConsole()
        { 
            lock (m_lock)
            {
                m_console = new Console();
            }
        }

        public void DestroyConsole()
        {
            lock (m_lock)
            {
                m_console = null;
            }
        }


        private static string GetLogTypeString(LogType logType)
        {
            switch (logType)
            {
                case LogType.Debug:
                    return "DEBUG";
                case LogType.Trace:
                    return "TRACE";
                case LogType.Log:
                    return "Log";
                case LogType.Lua:
                    return "Lua";
                case LogType.Warn:
                    return "WARNING";
                case LogType.Error:
                    return "FATAL ERROR";
                default:
                    return "Message";
            }
        }
    }
}
