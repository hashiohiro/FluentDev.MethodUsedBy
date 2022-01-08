using System.Runtime.Serialization;

namespace FluentDev.MethodUsedBy.Core.Error
{
    /// <summary>
    /// 必須パラメータが未指定のときに発生する例外
    /// </summary>
    public class RequiredParamException : ArgumentNullException
    {
        public RequiredParamException()
        {
        }

        public RequiredParamException(string? paramName) : base(paramName)
        {
        }

        public RequiredParamException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public RequiredParamException(string? paramName, string? message) : base(paramName, message)
        {
        }

        protected RequiredParamException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
