using FluentDev.MethodUsedBy.Core.Cache;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace FluentDev.MethodUsedBy.Core.Analyze
{
    /// <summary>
    /// メソッド参照解析コンポーネント
    /// </summary>
    public class MethodReferenceAnalyzer : Analyzer
    {
        /// <summary>
        /// IL Callオペコード
        /// </summary>
        private static readonly OpCode[] CallOpCodes = new OpCode[]
        {
            OpCodes.Call,
            OpCodes.Callvirt,
            OpCodes.Calli,
            OpCodes.Newobj,
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        public MethodReferenceAnalyzer(ILogger<MethodReferenceAnalyzer> logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// ロガー
        /// </summary>
        protected ILogger<MethodReferenceAnalyzer> Logger;

        /// <summary>
        /// 解析する
        /// </summary>
        /// <param name="cache">メソッド解析情報キャッシュ</param>
        /// <param name="methodInfo">解析対象メソッド</param>
        public override void Analyze(MethodCache cache, MethodInfo methodInfo)
        {
            foreach (var each in methodInfo.Definition.Body.Instructions)
            {
                // メソッド呼び出しの命令以外は解析をスキップする
                if (!CallOpCodes.Contains(each.OpCode)) continue;

                MethodReference useMethodRef = (MethodReference)each.Operand;

                // メソッド解析情報キャッシュより、参照先メソッドの定義を取得する
                MethodInfo useMethodInfo = cache.GetOrCreate(useMethodRef.FullName);

                // 解析対象メソッドが呼び出すメソッドに追加する
                if (!methodInfo.Uses.Contains(useMethodInfo))
                {
                    methodInfo.Uses.Add(useMethodInfo);
                }

                // 参照先メソッドの呼び出し元メソッドに追加する
                if (!useMethodInfo.UsedBy.Contains(methodInfo))
                {
                    useMethodInfo.UsedBy.Add(methodInfo);
                }
            }
        }
    }
}
