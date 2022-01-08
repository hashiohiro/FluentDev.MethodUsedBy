using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDev.MethodUsedBy.Core.Logging
{
    [ProviderAlias("FileLog")]
    public class FileLoggerProvider : ILoggerProvider
    {
        public FileLoggerProvider(IOptions<AppSettings> settings)
        {
            this.Settings = settings.Value;
            string logDir = this.Settings.Logging.FileLog.LogDir;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
        }

        protected AppSettings Settings;

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(this.Settings);
        }

        public void Dispose()
        {
        }
    }
}
