namespace FluentDev.MethodUsedBy.Core.Cache
{
    /// <summary>
    /// キャッシュ情報の抽出フィルタ
    /// </summary>
    public class CacheFilter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cache">フィルタ対象キャッシュ</param>
        private CacheFilter(MethodCache cache)
        {
            this.Cache = cache;
            this.Methods = cache.List();
        }

        /// <summary>
        /// 抽出フィルタのインスタンスを生成する
        /// </summary>
        /// <param name="cache">フィルタ対象キャッシュ</param>
        /// <returns>抽出フィルタ</returns>
        public static CacheFilter Create(MethodCache cache)
        {
            return new CacheFilter(cache);
        }

        /// <summary>
        /// メソッド解析情報キャッシュ
        /// </summary>
        protected MethodCache Cache;

        /// <summary>
        /// フィルタ対象の解析情報リスト
        /// </summary>
        protected IEnumerable<MethodInfo> Methods;

        /// <summary>
        /// 解析情報リストをメソッド単純名でフィルタする ※ 遅延評価
        /// </summary>
        /// <param name="methodSimpleNames">メソッド単純名</param>
        /// <returns>メソッドチェーンするためのフィルタインスタンス</returns>
        public CacheFilter FilterKeyword(string methodSimpleNames)
        {
            var k = methodSimpleNames.Split(",");
            this.Methods = this.Methods.Where(x => k.Contains(x.GetSimpleName()));
            return this;
        }

        /// <summary>
        /// 始祖メソッドを探索する ※ 即時評価
        /// </summary>
        /// <returns>探索対象のメソッドとその始祖メソッドの組リスト</returns>
        public (MethodInfo Target, MethodInfo[] Ancestors)[] TraverseAncestors()
        {
            // 探索対象のメソッドの始祖メソッドを再帰的に探索する
            void AncestorRecursive(MethodInfo target, MethodInfo current, HashSet<MethodInfo> ancestors)
            {
                current.AdditionalInfo[MethodInfo.AdditionalInfoKey.Traversed] = true;

                if (!current.UsedBy.Any())
                {
                    if (target != current && !ancestors.Contains(current))
                    {
                        ancestors.Add(current);
                    }
                    
                    return;
                }

                foreach (var each in current.UsedBy)
                {
                    AncestorRecursive(target, each, ancestors);
                }
            }

            // 探索対象メソッドごとに、始祖メソッドの探索処理を実行する
            return this.Methods.Select(x =>
                {
                    var a = new HashSet<MethodInfo>();
                    AncestorRecursive(x, x, a);
                    return (x, a.OrderBy(y => y.GetSimpleName(true, true, false, true)).ToArray());
                })
                .OrderBy(x => x.Item1.GetSimpleName(true, true, false, true))
                .ToArray();
            
        }

        public string[] ListSimpleName()
        {
            return this.Methods.Select(x => x.GetSimpleName()).ToArray();
        }

        public MethodInfo[] Execute()
        {
            return this.Methods.ToArray();
        }

    }
}
