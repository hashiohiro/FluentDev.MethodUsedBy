using FluentDev.MethodUsedBy.Core.Cache;

namespace FluentDev.MethodUsedBy.Core.Analyze
{
    /// <summary>
    /// 解析処理を表すコンポーネント
    /// </summary>
    public abstract class Analyzer
    {
        /// <summary>
        /// 解析する
        /// </summary>
        /// <param name="cache">メソッド解析情報キャッシュ</param>
        /// <param name="methodInfo">解析対象メソッド</param>
        public abstract void Analyze(MethodCache cache, MethodInfo methodInfo);
    }
}
