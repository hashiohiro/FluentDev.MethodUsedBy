namespace FluentDev.MethodUsedBy.Core.IO
{
    /// <summary>
    /// ファイル操作のユーティリティ
    /// </summary>
    public static class FileUtil
    {
        public static DirectoryInfo GetOrCreateDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                return Directory.CreateDirectory(dir);
            }
            return new DirectoryInfo(dir);
        }

        public static FileInfo GetOrCreateFile(string file)
        {
            if (!File.Exists(file))
            {
                using var stream = File.Create(file);
            }
            return new FileInfo(file);
        }
    }
}
