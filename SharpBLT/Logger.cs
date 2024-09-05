namespace SharpBLT;

using System.Runtime.CompilerServices;

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
    private ConsoleEx? m_console;

    public LogType CurrentLogLevel { get; set; }

    public Logger()
    {
        CurrentLogLevel = LogType.Debug;

        string path = "mods/logs/";
        path += DateTime.Now.ToString("yyyy-MM-dd");
        path += "_log.txt";

        m_logFileStream = new FileStream(path, FileMode.Append, FileAccess.Write);
        m_writer = new StreamWriter(m_logFileStream);
        m_lock = new object();
    }

    static string? thisPath = null;

    private static string GetThisPath([CallerFilePath] string? path = null)
    {
        thisPath ??= Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
        return thisPath!;
    }

    public virtual void Log(LogType logType, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        if (logType < CurrentLogLevel)
            return;

        DateTime time = DateTime.Now;
        string strlogType = GetLogTypeString(logType);

        string strippedFile = file!.Replace(GetThisPath(), string.Empty);
        string msg = string.Format("{0} {1}: ({2}:{3}) {4}", time, strlogType, strippedFile, line, message);

        lock (m_lock)
        {
            if (m_logFileStream != null)
            {
                m_writer.WriteLine(msg);
                m_writer.Flush();
            }

            if (m_console != null)
            {
                IntPtr hStdout = Kernel32.GetStdHandle(Kernel32.STD_OUTPUT_HANDLE);

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

                Console.WriteLine(msg);
            }
        }
    }

    public void OpenConsole()
    {
        lock (m_lock)
        {
            m_console = new ConsoleEx();
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
        return logType switch
        {
            LogType.Debug => "DEBUG",
            LogType.Trace => "TRACE",
            LogType.Log => "Log",
            LogType.Lua => "Lua",
            LogType.Warn => "WARNING",
            LogType.Error => "FATAL ERROR",
            _ => "Message",
        };
    }
}
