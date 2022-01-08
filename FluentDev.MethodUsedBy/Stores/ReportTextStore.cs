using FluentDev.MethodUsedBy.Core.Cache;
using FluentDev.MethodUsedBy.Core.IO;
using Microsoft.Extensions.Options;

namespace FluentDev.MethodUsedBy.Core.Report
{
    /// <summary>
    /// 解析結果をテキストファイルに保存するためのデータストア
    /// </summary>
    public class ReportTextStore : FileStore
    {
        protected const string CallContext_CallHistory = "CallHistory";
        public ReportTextStore(IOptions<AppSettings> setting) : base(setting) { }

        public override bool Exists()
        {
            // 直近の解析結果フォルダを探す
            DirectoryInfo latest = this.LatestTimestampSubDirectory();

            if (latest == null) return false;

            return File.Exists($"{latest.FullName}\\{this.GetFileName()}");
        }

        public override string GetDirectoryName()
        {
            return this._timestampDirectory ?? this.Settings.Analyze.ExportDir;
        }

        private string _timestampDirectory;

        public override string GetFileName()
        {
            return this._currentFile;
        }
        private string _currentFile;

        public override void Save(MethodCache cache)
        {
            this.SetupDirectory();
            var targets = CacheFilter.Create(cache)
                                     .FilterKeyword(this.Settings.Keyword)
                                     .TraverseAncestors();

            this.SaveList(targets);
            this.SaveDetail(targets);
        }

        protected void SaveList((MethodInfo Target, MethodInfo[] Ancestors)[] targets)
        {
            foreach (var perReportFile in targets.GroupBy(x => "一覧_" + x.Target.GetSimpleName() + ".txt"))
            {
                // https://stackoverflow.com/questions/6350224/does-filestream-dispose-close-the-file-immediately#:~:text=1%20Answer&text=As%20%22immediately%22%20as%20possible.
                using var stream = this.SetupFile(perReportFile.Key);
                using var writer = new StreamWriter(stream);

                var ancestors = perReportFile
                    .SelectMany(x => x.Ancestors)
                    .Select(y => y.GetSimpleName(true, true, false, true))
                    .Distinct()
                    .ToArray();

                foreach (var each in ancestors)
                {
                    writer.WriteLine(each);
                }
            }
        }

        protected void SaveDetail((MethodInfo Target, MethodInfo[] Ancestors)[] targets)
        {
            foreach (var perReportFile in targets.GroupBy(x => "詳細_" + x.Target.GetSimpleName() + ".txt"))
            {
                // https://stackoverflow.com/questions/6350224/does-filestream-dispose-close-the-file-immediately#:~:text=1%20Answer&text=As%20%22immediately%22%20as%20possible.
                using var stream = this.SetupFile(perReportFile.Key);
                using var writer = new StreamWriter(stream);
                foreach (var each in perReportFile)
                {
                    foreach (var a in each.Ancestors)
                    {
                        this.WriteDetailRecursive(a, writer, 0);
                        writer.Write(Environment.NewLine);
                    }
                }
            }
        }

        protected void WriteDetailRecursive(MethodInfo methodInfo, StreamWriter writer, int depth)
        {
            if (!methodInfo.AdditionalInfo.ContainsKey(MethodInfo.AdditionalInfoKey.Traversed)) return;

            var indent = new string('\t', depth);
            writer.Write(indent);
            if (depth > this.Settings.Analyze.MaxCalltreeDepth)
            {
                writer.WriteLine(String.Format(MessageResource.MSG00014, methodInfo.GetSimpleName(true, true, false, true), indent));
                return;
            }
            writer.WriteLine(methodInfo.GetSimpleName(true, true, false, true));

            foreach (var each in methodInfo.Uses)
            {
                this.WriteDetailRecursive(each, writer, depth + 1);
            }
        }

        /// <summary>
        /// タイムスタンプディレクトリを作成して、解析結果の出力先をそこに設定する
        /// </summary>
        protected void SetupDirectory()
        {
            // 出力用のタイムスタップディレクトリを初期化する
            this._timestampDirectory = null;

            // 出力ディレクトリを作成する
            this.GetOrCreateDirectory();

            this._timestampDirectory = this.CreateTimestampSubDirectory().FullName;
        }

        /// <summary>
        /// 調査対象ごとにファイルを作成して、解析結果の出力をそこに設定する
        /// </summary>
        protected FileStream SetupFile(string file)
        {
            this._currentFile = file;

            // 出力ファイルを作成する
            FileInfo textFile = this.GetOrCreateFile();
            return textFile.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
        }

        public override void Load(MethodCache cache)
        {
            throw new NotSupportedException();
        }
    }
}
