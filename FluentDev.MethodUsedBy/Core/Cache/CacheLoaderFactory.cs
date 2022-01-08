using FluentDev.MethodUsedBy.Core.IO;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDev.MethodUsedBy.Core.Cache
{
    /// <summary>
    /// キャッシュ読み込みコンポーネントの生成を担う
    /// </summary>
    public class CacheLoaderFactory
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="provider">IServiceProvider</param>
        /// <param name="fileStoreFactory">ファイル形式のデータストアのファクトリ</param>
        public CacheLoaderFactory(IServiceProvider provider, FileStoreFactory fileStoreFactory)
        {
            this.Provider = provider;
            this.FileStoreFactory = fileStoreFactory;
        }

        /// <summary>
        /// IServiceProvider
        /// </summary>
        protected IServiceProvider Provider;

        /// <summary>
        /// ファイル形式のデータストアのファクトリ
        /// </summary>
        protected FileStoreFactory FileStoreFactory;

        public ICacheLoader Create()
        {
            // キャッシュファイルが存在する場合
            FileStore store = this.FileStoreFactory.Create(FileStoreKind.CacheFileStore);
            if (store.Exists())
            {
                return this.Provider.GetService<CacheFileStore>();
            }
            // 存在しない場合
            else
            {
                return this.Provider.GetService<CacheAssemblyLoader>();
            }
        }
    }
}
