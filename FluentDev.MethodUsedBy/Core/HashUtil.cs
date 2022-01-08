using System.Security.Cryptography;
using System.Text;

namespace FluentDev.MethodUsedBy.Core
{
    /// <summary>
    /// ハッシュ関数のユーティリティ
    /// </summary>
    public static class HashUtil
    {
        public static byte[] Sha1(string str)
        {
            using var sha256 = SHA1.Create();
            var bytes = Encoding.UTF8.GetBytes(str);
            return sha256.ComputeHash(bytes);
        }
    }
}
