using FluentDev.MethodUsedBy.Core.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentDev.MethodUsedBy.Core.Cache
{
    /// <summary>
    /// ファイルとして永続化されたキャッシュをデータストアとして扱うコンポーネント
    /// </summary>
    public class CacheFileStore : FileStore, ICacheLoader
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="setting">構成ファイルモデル</param>
        /// <param name="logger">ロガー</param>
        public CacheFileStore(IOptions<AppSettings> setting, ILogger<CacheFileStore> logger) : base(setting)
        {
            this.Settings = setting.Value;
            this.Logger = logger;
        }

        /// <summary>
        /// ロガー
        /// </summary>
        protected ILogger<CacheFileStore> Logger;

        /// <summary>
        /// キャッシュが永続化済みかどうか
        /// </summary>
        public bool Persisted => this.Exists();

        /// <summary>
        /// ストアファイルのディレクトリのパスを取得する
        /// </summary>
        /// <returns>ストアファイルのディレクトリのパス</returns>
        public override string GetDirectoryName()
        {
            return this.Settings.Cache.Dir;
        }

        /// <summary>
        /// ストアファイルのパスを取得する
        /// </summary>
        /// <returns>ストアファイルのパス</returns>
        public override string GetFileName()
        {
            return BitConverter.ToString(HashUtil.Sha1(this.Settings.AssemblyDir)).Replace("-", string.Empty) + ".cache";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        public override void Save(MethodCache cache)
        {
            this.GetOrCreateDirectory();

            using var writer = this.GetOrCreateFile().CreateText();

            foreach (var method in cache.List().ToArray())
            {
                writer.WriteLine($"{method.Name}");
                foreach (var usedBy in method.UsedBy)
                {
                    writer.WriteLine($"\t{usedBy.Name}");
                }
                writer.WriteLine("\t");
                foreach (var uses in method.Uses)
                {
                    writer.WriteLine($"\t{uses.Name}");
                }
            }
        }


        public override void Load(MethodCache cache)
        {
            FileFilter.Create(this.GetOrCreateDirectory())
                .AddPattern("*.cache")
                .EachOpenText(SearchOption.TopDirectoryOnly, (each, reader) =>
                {
                    // 調査対象アセンブリのキャッシュのみ利用する場合
                    if (!this.Settings.Cache.UseCacheAll)
                    {
                        if (each.Name != this.GetFileName()) return;
                    }
                    // キャッシュフォルダ内のすべてのキャッシュを利用する場合
                    else
                    {
                        // nop
                    }

                    string line = null;
                    MethodInfo currentMi = null;
                    bool uses = false;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // メソッド名の行
                        if (!line.StartsWith("\t"))
                        {
                            uses = false;
                            var token = line.Split("\t");
                            currentMi = cache.GetOrCreate(token[0]);
                            continue;
                        }
                        else
                        {
                            // UsedBy/Usesを区切る、タブ文字のみの行
                            if (line == "\t")
                            {
                                uses = true;
                                continue;
                            }
                            else
                            {
                                var nm = line[1..line.Length];
                                var m = cache.GetOrCreate(nm);
                                // Usesの行
                                if (uses)
                                {
                                    currentMi.Uses.Add(m);
                                }
                                // UsedByの行
                                else
                                {
                                    currentMi.UsedBy.Add(m);
                                }

                                continue;
                            }
                        }
                    }
                });
        }
    }
}
