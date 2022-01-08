using FluentDev.MethodUsedBy.Core;
using FluentDev.MethodUsedBy.Core.Analyze;
using FluentDev.MethodUsedBy.Core.Cache;
using FluentDev.MethodUsedBy.Core.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentDev.MethodUsedBy.Services
{
    public class AnalyzeService
    {
        public AnalyzeService(
            IOptions<AppSettings> options,
            SettingEnsuringService settingEnsuring,
            CacheLoaderFactory cacheLoaderFactory,
            FileStoreFactory fileStoreFactory,
            ILogger<AnalyzeService> logger)
        {
            this.Settings = options.Value;
            this.SettingEnsuring = settingEnsuring;
            this.Logger = logger;
            this.CacheLoaderFactory = cacheLoaderFactory;
            this.FileStoreFactory = fileStoreFactory;
        }

        /// <summary>
        /// 構成ファイルモデル
        /// </summary>
        protected AppSettings Settings;
        protected SettingEnsuringService SettingEnsuring;
        protected ILogger<AnalyzeService> Logger;
        protected CacheLoaderFactory CacheLoaderFactory;
        protected FileStoreFactory FileStoreFactory;

        public void Analyze()
        {
            // パラメタおよび設定の検証中...
            this.Logger.LogInformation(MessageResource.MSG00003);
            this.SettingEnsuring.Ensure();

            MethodCache cache = new MethodCache();

            this.LoadCache(cache, out bool persisted);

            // キャッシュファイルを保存中...
            if (this.Settings.Cache.Persist && !persisted)
            {
                this.Logger.LogInformation(MessageResource.MSG00011);
                this.PersistCache(cache);
            }

            // 解析結果をファイルへ出力中...
            this.Logger.LogInformation(MessageResource.MSG00010);
            this.ExportFile(cache);
        }

        protected void LoadCache(MethodCache cache, out bool persisted)
        {
            ICacheLoader loader = this.CacheLoaderFactory.Create();

            if (loader.Persisted)
            {
                // キャッシュから解析結果を読み込み中...
                this.Logger.LogInformation(MessageResource.MSG00005);
            }
            else
            {
                // 呼び出し階層を解析中...
                this.Logger.LogInformation(MessageResource.MSG00004);
            }

            loader.Load(cache);
            persisted = loader.Persisted;
        }

        protected void PersistCache(MethodCache cache)
        {
            FileStore cacheStore = this.FileStoreFactory.Create(FileStoreKind.CacheFileStore);
            cacheStore.Save(cache);
        }

        protected void ExportFile(MethodCache cache)
        {
            FileStore reportStore = this.FileStoreFactory.Create(FileStoreKind.ReportTextStore);
            reportStore.Save(cache);
        }
    }
}
