using System.Runtime.Serialization;

namespace FluentDev.MethodUsedBy.Core.Error
{
    /// <summary>
    /// パラメータの指定値が不正の場合、発生する例外
    /// </summary>
    public class InvalidParamValueException : ArgumentException
    {
        public InvalidParamValueException()
        {
        }

        public InvalidParamValueException(string? message) : base(message)
        {
        }

        public InvalidParamValueException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public InvalidParamValueException(string? message, string? paramName) : base(message, paramName)
        {
        }

        public InvalidParamValueException(string? message, string? paramName, Exception? innerException) : base(message, paramName, innerException)
        {
        }

        protected InvalidParamValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
