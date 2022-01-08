using FluentDev.MethodUsedBy.Core.Analyze;
using FluentDev.MethodUsedBy.Core.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mono.Cecil;

namespace FluentDev.MethodUsedBy.Core.Cache
{
    /// <summary>
    /// アセンブリよりキャッシュを読み込むキャッシュローダ
    /// </summary>
    public class CacheAssemblyLoader : ICacheLoader
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="setting">構成ファイルモデル</param>
        /// <param name="methodReference">解析コンポーネントのファクトリ</param>
        /// <param name="logger">ロガー</param>
        public CacheAssemblyLoader(IOptions<AppSettings> setting, MethodReferenceAnalyzer methodReference, ILogger<CacheAssemblyLoader> logger)
        {
            this.MethodReference = methodReference;
            this.Settings = setting.Value;
            this.Logger = logger;
        }

        /// <summary>
        /// 構成ファイルモデル
        /// </summary>
        protected AppSettings Settings;

        /// <summary>
        /// 解析コンポーネントのファクトリ
        /// </summary>
        protected MethodReferenceAnalyzer MethodReference;

        /// <summary>
        /// ロガー
        /// </summary>
        protected ILogger<CacheAssemblyLoader> Logger;

        /// <summary>
        /// キャッシュが永続化済みかどうか
        /// </summary>
        public bool Persisted => false;

        /// <summary>
        /// キャッシュを読み込む
        /// </summary>
        /// <param name="cache">読み込み対象キャッシュ</param>
        public void Load(MethodCache cache)
        {
            var files = FileFilter.Create(FileUtil.GetOrCreateDirectory(this.Settings.AssemblyDir))
                .AddPatterns("*.dll|*.exe")
                .Execute(SearchOption.AllDirectories);
            foreach (var file in files)
            {
                // モジュール
                foreach (var mo in AssemblyDefinition.ReadAssembly(file.FullName).Modules)
                {
                    // 無視リストに含まれるアセンブリはキャッシュしない
                    if (this.ContainsAssemblyIgnore(mo.Assembly.Name.Name)) continue;

                    // 型
                    foreach (var t in mo.Types)
                    {
                        // メソッド
                        foreach (var mt in t.Methods)
                        {
                            var mi = cache.GetOrCreate(mt.FullName);
                            mi.Definition = mt;
                        }
                    }
                }
            }

            // 呼び出し階層ツリーの構築
            this.CreateReferenceInfo(cache);
        }

        /// <summary>
        /// アセンブリ無視リストチェック
        /// </summary>
        /// <param name="assemblyFullName">チェック対象アセンブリ名</param>
        /// <returns>無視対象の場合はtrue</returns>
        private bool ContainsAssemblyIgnore(string assemblyFullName)
        {
            return this.Settings.Analyze.AssemblyIgnore.Any(x =>
            {
                // 部分一致
                if (x.EndsWith("*") && x.StartsWith("*"))
                {
                    var removeAster = x[1..(x.Length - 1)];
                    return assemblyFullName.Contains(removeAster);
                }

                // 前方一致
                if (x.EndsWith("*"))
                {
                    var removeAster = x[0..(x.Length - 1)];
                    return assemblyFullName.StartsWith(removeAster);
                }

                // 後方一致
                if (x.StartsWith("*"))
                {
                    var removeAster = x[1..(x.Length)];
                    return assemblyFullName.EndsWith(removeAster);
                }

                // 完全一致
                return assemblyFullName == x;
            });
        }

        /// <summary>
        /// メソッド参照情報を構築する
        /// </summary>
        /// <param name="cache"></param>
        protected void CreateReferenceInfo(MethodCache cache)
        {
            foreach (var each in cache.List().ToArray())
            {
                // メソッド定義が見つからない場合、警告ログを記録して解析をスキップする
                if (each.Definition == null)
                {
                    this.Logger.LogWarning(MessageResource.MSG00007, each.Name);
                    continue;
                }

                // メソッドが実装を持たない場合、警告ログを記録して解析をスキップする
                if (!each.Definition.HasBody)
                {
                    if (this.Settings.Analyze.SuppressWarning.MethodNoBody) continue;

                    if (this.Settings.Analyze.SuppressWarning.MethodNoBodyInterfaceOnly)
                    {
                        if (each.Definition.DeclaringType.IsInterface) continue;
                    }

                    if (this.Settings.Analyze.SuppressWarning.MethodNoBodyAbstractClassOnly)
                    {
                        if (each.Definition.DeclaringType.IsAbstract) continue;
                    }

                    this.Logger.LogWarning(MessageResource.MSG00008, each.Name);
                    continue;
                }
                this.MethodReference.Analyze(cache, each);
            }
        }

    }
}
