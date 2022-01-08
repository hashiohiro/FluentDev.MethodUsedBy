namespace FluentDev.MethodUsedBy.Core.Error
{
    /// <summary>
    /// 例外のハンドラ
    /// </summary>
    public class ErrorHandler
    {
        /// <summary>
        /// 例外に対して、適切なエラーメッセージを返す
        /// </summary>
        /// <param name="exception">例外</param>
        /// <returns>エラーメッセージ</returns>
        public string[] GetErrorMessages(Exception exception)
        {
            var errorMessages = new List<string>();
            this.GetErrorMessageRecursive(exception, errorMessages);
            return errorMessages.ToArray();
        }

        /// <summary>
        /// 複合的な例外を再帰的に調査し、エラーメッセージのリストを取得する
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="errorMessages"></param>
        public void GetErrorMessageRecursive(Exception exception, List<string> errorMessages)
        {
            // 複合的な例外は掘り下げ調査する
            if (exception is AggregateException)
            {
                var ag = (AggregateException)exception;
                foreach (var each in ag.InnerExceptions)
                    this.GetErrorMessageRecursive(each, errorMessages);
            }
            // 単一のハンドル対象例外は、例外が持つメッセージを取得する
            else if (this.HandleError(exception))
            {
                errorMessages.Add(exception.Message);
            }
            // 未ハンドルのエラーは即時に例外を起こす
            else
            {
                throw exception;
            }
        }

        /// <summary>
        /// 例外がハンドル対象かどうか判断する
        /// </summary>
        /// <param name="ex">例外</param>
        /// <returns>ハンドル対象の場合はtrueを返す</returns>
        protected bool HandleError(Exception ex)
        {
            return ex is RequiredParamException
                || ex is InvalidParamValueException;
        }
    }
}
