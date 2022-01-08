using FluentDev.MethodUsedBy.Core.Cache;
using Microsoft.Extensions.Options;

namespace FluentDev.MethodUsedBy.Core.IO
{
    /// <summary>
    /// データの保存場所を表すコンポーネント
    /// </summary>
    public abstract class DataStore
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="setting"></param>
        public DataStore(IOptions<AppSettings> setting)
        {
            this.Settings = setting.Value;
        }

        /// <summary>
        /// 構成ファイルモデル
        /// </summary>
        protected AppSettings Settings;

        public abstract void Save(MethodCache cache);
        public abstract void Load(MethodCache cache);
    }
}
