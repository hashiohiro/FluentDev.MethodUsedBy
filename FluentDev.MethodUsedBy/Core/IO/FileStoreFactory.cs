using FluentDev.MethodUsedBy.Core.IO;
using FluentDev.MethodUsedBy.Core.Report;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDev.MethodUsedBy.Core.Cache
{
    /// <summary>
    /// ファイル形式のデータストアのファクトリ
    /// </summary>
    public class FileStoreFactory
    {
        public FileStoreFactory(IServiceProvider provider)
        {
            this.Provider = provider;
        }

        /// <summary>
        /// IServiceProvider
        /// </summary>
        protected IServiceProvider Provider;

        public FileStore Create(FileStoreKind kind)
        {
            if (kind == FileStoreKind.CacheFileStore)
            {
                return this.Provider.GetService<CacheFileStore>();
            }
            if (kind == FileStoreKind.ReportTextStore)
            {
                return this.Provider.GetService<ReportTextStore>();
            }
            return null;
        }
    }

    public enum FileStoreKind
    {
        CacheFileStore,
        ReportTextStore,
    }
}
