using FluentDev.MethodUsedBy.Core;
using FluentDev.MethodUsedBy.Core.Error;
using Microsoft.Extensions.Options;

namespace FluentDev.MethodUsedBy.Services
{
    /// <summary>
    /// 設定に異常値がないことを保障する検証サービス
    /// </summary>
    public sealed partial class SettingEnsuringService
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings">アプリ設定</param>
        public SettingEnsuringService(IOptions<AppSettings> settings)
        {
            this.settings = settings.Value;
        }

        /// <summary>
        /// 構成ファイルモデル
        /// </summary>
        private AppSettings settings;

        /// <summary>
        /// 設定を検証する
        /// </summary>
        public void Ensure()
        {
            var aggregateEx = new AggregateExceptionBuilder();
            this.CheckCmdParam(aggregateEx);

            if (aggregateEx.HasException)
            {
                throw aggregateEx.Build();
            }
        }

        /// <summary>
        /// コマンドパラメタのチェック
        /// </summary>
        /// <exception cref="ArgumentNullException">必須パラメータが未指定のとき、発生する</exception>
        private void CheckCmdParam(AggregateExceptionBuilder builder)
        {
            #region 必須チェック

            if (string.IsNullOrWhiteSpace(this.settings.AssemblyDir))
                builder.AddCmdParamNull(MessageResource.AssemblyDir);

            if (string.IsNullOrWhiteSpace(this.settings.Keyword))
                builder.AddCmdParamNull(MessageResource.Keyword);

            // エラーが発生している場合、処理を中断する
            if (builder.HasException) return;

            #endregion

            #region 値チェック
            if (this.IsInvalidDirectoryName(this.settings.AssemblyDir))
                builder.AddCmdParamInvalid(MessageResource.AssemblyDir);

            if (this.IsInvalidKeyword(this.settings.Keyword))
                builder.AddCmdParamInvalid(MessageResource.Keyword);

            #endregion
        }

        private bool IsInvalidDirectoryName(string path)
        {
            return !Directory.Exists(path);
        }

        private bool IsInvalidKeyword(string keyword)
        {
            // キーワードの値チェックは行わない
            return false;
        }
    }
}
