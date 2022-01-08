using Mono.Cecil;
using System.Text.RegularExpressions;

namespace FluentDev.MethodUsedBy.Core
{
    /// <summary>
    /// メソッド解析情報
    /// </summary>
    public class MethodInfo
    {
        public MethodInfo(string name)
        {
            this.Name = name;
        }

        private static readonly Regex argumentReg = new Regex(@"\(.+");
        private static readonly Regex genericReg = new Regex(@"`\d+");

        public string Name { get; set; }

        public string GetSimpleName(bool withArgument = false, bool withPropertyPrefix = false, bool withReturnType = false, bool withCtorSuffix = false)
        {
            var t = this.Name.Split(" ");
            string name = t[1];

            if (!withArgument)
            {
                name = argumentReg.Replace(name, string.Empty);
            }

            name = genericReg.Replace(name, string.Empty);
            name = name.Replace("::", ".");

            if (!withCtorSuffix)
            {
                name = name.Replace("..ctor", string.Empty);
            }

            if (!withPropertyPrefix)
            {
                name = name.Replace("get_", string.Empty)
                           .Replace("set_", string.Empty);
            }

            if (withReturnType)
            {
                name = t[0] + " " + name;
            }

            return name;
        }

        [field: NonSerialized]
        public MethodDefinition? Definition { get; set; }

        // https://stackoverflow.com/questions/150750/hashset-vs-list-performance
        public HashSet<MethodInfo> Uses { get; set; } = new HashSet<MethodInfo>();

        public HashSet<MethodInfo> UsedBy { get; set; } = new HashSet<MethodInfo>();

        [field: NonSerialized]
        public Dictionary<string, object> AdditionalInfo { get; set; } = new Dictionary<string, object>();

        public static class AdditionalInfoKey
        {
            public static readonly string Traversed = "Traversed";
        }
    }
}
