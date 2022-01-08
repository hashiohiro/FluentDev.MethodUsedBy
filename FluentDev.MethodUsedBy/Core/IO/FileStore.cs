using Microsoft.Extensions.Options;

namespace FluentDev.MethodUsedBy.Core.IO
{
    /// <summary>
    /// ファイル形式のデータストア
    /// </summary>
    public abstract class FileStore : DataStore
    {
        protected FileStore(IOptions<AppSettings> setting) : base(setting)
        {
        }

        /// <summary>
        /// ストアファイルが存在するかどうか
        /// </summary>
        /// <returns>存在する場合はtrueを返す</returns>
        public virtual bool Exists()
        {
            return File.Exists($"{this.GetDirectoryName()}\\{this.GetFileName()}");
        }

        /// <summary>
        /// ストアファイルのディレクトリのパスを取得する
        /// </summary>
        /// <returns>ストアファイルのディレクトリのパス</returns>
        public abstract string GetDirectoryName();

        /// <summary>
        /// ストアファイルのパスを取得する
        /// </summary>
        /// <returns>ストアファイルのパス</returns>
        public abstract string GetFileName();

        /// <summary>
        /// ストアファイルのディレクトリを取得または生成する
        /// </summary>
        /// <returns>ストアファイルのディレクトリ情報</returns>
        protected DirectoryInfo GetOrCreateDirectory()
        {
            var dir = this.GetDirectoryName();
            return FileUtil.GetOrCreateDirectory(dir);
        }

        /// <summary>
        /// ストアファイルを取得または生成する
        /// </summary>
        /// <returns>ストアファイル情報</returns>
        protected FileInfo GetOrCreateFile()
        {
            var file = $"{this.GetDirectoryName()}\\{this.GetFileName()}";
            return FileUtil.GetOrCreateFile(file);
        }

        /// <summary>
        /// 名称が現在日時のタイムスタンプのサブディレクトリを生成する
        /// </summary>
        /// <returns>ディレクトリ情報</returns>
        protected DirectoryInfo CreateTimestampSubDirectory()
        {
            var di = this.GetOrCreateDirectory();
            return di.CreateSubdirectory(DateTime.Now.ToString("yyyy-MM-dd HHmmss fff"));
        }

        /// <summary>
        /// 直近のタイムスタンプのサブディレクトリを取得する
        /// </summary>
        /// <returns>ディレクトリ情報</returns>
        protected DirectoryInfo LatestTimestampSubDirectory()
        {
            var di = this.GetOrCreateDirectory();
            return di.GetDirectories().OrderByDescending(x => x.Name).FirstOrDefault();
        }
    }
}
