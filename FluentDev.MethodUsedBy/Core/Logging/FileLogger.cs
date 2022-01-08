using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDev.MethodUsedBy.Core.Logging
{
    public class FileLogger : ILogger
    {
        public FileLogger(AppSettings settings)
        {
            this.Settings = settings;
        }

        protected AppSettings Settings;
        protected FileInfo CurrentFile;

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            if (this.CurrentFile == null)
            {
                var logDir = new DirectoryInfo(this.Settings.Logging.FileLog.LogDir);
                var log = logDir.GetFiles("*.log", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.Name).FirstOrDefault();

                if (log == null)
                {
                    this.CurrentFile = this.CreateLogFile();
                }
                else
                {
                    this.CurrentFile = log;
                }
            }
            else
            {
                if (this.CurrentFile.Length > this.Settings.Logging.FileLog.MaxFileSize)
                {
                    this.CurrentFile = this.CreateLogFile();
                }
                else
                {
                    // nop
                }
            }


            var logRecord = string.Format("{0} [{1}] {2} {3}", "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "]", logLevel.ToString(), formatter(state, exception), exception != null ? exception.StackTrace : "");

            using (var writer = new StreamWriter(this.CurrentFile.FullName, true))
            {
                writer.WriteLine(logRecord);
            }
        }

        protected FileInfo CreateLogFile()
        {
            var fullFilePath = this.Settings.Logging.FileLog.LogDir + "/" + this.Settings.Logging.FileLog.LogName.Replace("{timestamp}", DateTimeOffset.UtcNow.ToString("yyyyMMdd_HHmmss"));
            return new FileInfo(fullFilePath);
        }
    }
}
