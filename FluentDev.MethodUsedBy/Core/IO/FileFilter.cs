namespace FluentDev.MethodUsedBy.Core.IO
{
    /// <summary>
    /// ファイルの抽出フィルタ
    /// </summary>
    public class FileFilter
    {
        public static FileFilter Create(DirectoryInfo di)
        {
            return new FileFilter(di);
        }

        private FileFilter(DirectoryInfo di)
        {
            this.Directory = di;
            this.Pattern = new List<string>();
        }

        protected DirectoryInfo Directory;
        protected List<string> Pattern;

        public FileFilter AddPattern(string searchPattern)
        {
            this.Pattern.Add(searchPattern);
            return this;
        }

        public FileFilter AddPatterns(string searchPattern)
        {
            this.Pattern.AddRange(searchPattern.Split("|"));
            return this;
        }

        public FileInfo[] Execute(SearchOption option)
        {
            var files = new List<FileInfo>();

            // 拡張子の複数指定に対応 ※ パイプ(|)で区切る
            foreach (var each in this.Pattern)
            {
                files.AddRange(this.Directory.GetFiles(each, option));
            }

            return files.ToArray();
        }

        public FileFilter EachOpenText(SearchOption option, Action<FileInfo, StreamReader> action)
        {
            foreach (var each in this.Execute(option))
            {
                using var reader = each.OpenText();
                action(each, reader);
            }
            return this;
        }

    }
}
