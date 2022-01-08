using System.Collections;

namespace FluentDev.MethodUsedBy.Core.Cache
{

    /// <summary>
    /// メソッド解析情報を保持するキャッシュ
    /// </summary>
    public class MethodCache
    {
        /// <summary>
        /// 内部キャッシュ
        /// </summary>
        private Hashtable innerCache = new Hashtable();

        /// <summary>
        /// キャッシュが存在する場合は取得、存在しない場合は新しく生成する
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>メソッド解析情報</returns>
        public MethodInfo GetOrCreate(string key)
        {
            if (this.innerCache.ContainsKey(key))
            {
                return (MethodInfo)this.innerCache[key];
            }
            else
            {
                var mi = new MethodInfo(key);
                this.innerCache[key] = mi;
                return mi;
            }
        }

        /// <summary>
        /// キャッシュを全件列挙する
        /// </summary>
        /// <returns>メソッド解析情報</returns>
        public IEnumerable<MethodInfo> List()
        {
            foreach (DictionaryEntry each in this.innerCache)
            {
                yield return (MethodInfo)each.Value;
            }
        }
    }
}
