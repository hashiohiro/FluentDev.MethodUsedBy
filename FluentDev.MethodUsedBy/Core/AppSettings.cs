namespace FluentDev.MethodUsedBy.Core
{
    /// <summary>
    /// 構成ファイルのモデル
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// [パラメタ] アセンブリが格納されているディレクトリを指定する
        /// </summary>
        public string AssemblyDir { get; set; }

        /// <summary>
        /// [パラメタ] 解析対象のメソッドを指定するキーワード
        /// </summary>
        public string Keyword { get; set; }

        public LoggingSection Logging { get; set; }
        public AnalyzeSection Analyze { get; set; }
        public CacheSection Cache { get; set; }


        public class CacheSection
        {
            /// <summary>
            /// [設定ファイル] キャッシュ保存先のディレクトリを指定する
            /// </summary>
            public string Dir { get; set; }

            /// <summary>
            /// [設定ファイル] キャッシュを永続化するかどうか
            /// </summary>
            public bool Persist { get; set; }

            /// <summary>
            /// [設定ファイル] キャッシュフォルダのすべてのキャッシュを利用するかどうか
            /// </summary>
            /// <remarks>
            /// ※ パフォーマンスの観点で大量のアセンブリを一度に解析することを避ける時など。
            /// あらかじめ準備しておいたキャッシュをキャッシュフォルダに配置することで利用できるようになる。
            /// ※ デフォルトの設定は無効。
            /// </remarks>
            public bool UseCacheAll { get; set; }
        }

        public class AnalyzeSection
        {
            /// <summary>
            /// [設定ファイル] 解析対象から除外するアセンブリの指定
            /// 以下のルールで完全一致、前方一致、後方一致、部分一致のいずれかの検索方式を指定する。
            /// </summary>
            /// <remarks>
            /// [検索方式の指定]
            /// 完全一致 : [アセンブリ名]
            /// 前方一致 : [アセンブリ名]*
            /// 後方一致 : *[アセンブリ名]
            /// 部分一致 : *[アセンブリ名]*
            /// </remarks>
            public string[] AssemblyIgnore { get; set; }

            /// <summary>
            /// [設定ファイル] 解析結果の保存先のディレクトリを指定する
            /// </summary>
            public string ExportDir { get; set; }

            /// <summary>
            /// 呼び出し階層の最大の深さを指定する
            /// ※ 循環参照の打ち切り用
            /// </summary>
            public int MaxCalltreeDepth { get; set; }

            public SuppressWarningSection SuppressWarning { get; set; }
        }

        public class LoggingSection
        {
            public FileLogSection FileLog { get; set; }
            public class FileLogSection 
            { 
                public string LogDir { get; set; }
                public string LogName { get; set; }
                public int MaxFileSize { get; set; }
            }
        }

        public class SuppressWarningSection
        {
            public bool MethodNoBody { get; set; }
            public bool MethodNoBodyInterfaceOnly { get; set; }
            public bool MethodNoBodyAbstractClassOnly { get; set; }
        }
    }

}
