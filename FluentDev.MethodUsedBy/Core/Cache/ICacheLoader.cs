namespace FluentDev.MethodUsedBy.Core.Cache
{
    /// <summary>
    /// キャッシュの読み込みコンポーネント
    /// </summary>
    public interface ICacheLoader
    {
        /// <summary>
        /// キャッシュを読み込む
        /// </summary>
        /// <param name="cache">読み込み対象キャッシュ</param>
        public void Load(MethodCache cache);

        /// <summary>
        /// キャッシュが永続化済みかどうか
        /// </summary>
        public bool Persisted { get; }
    }
}
