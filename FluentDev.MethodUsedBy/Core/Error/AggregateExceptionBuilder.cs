namespace FluentDev.MethodUsedBy.Core.Error
{
    // https://stackoverflow.com/questions/3025525/throwing-an-aggregateexception-in-my-own-code
    /// <summary>
    /// 複合的な例外ファクトリ
    /// </summary>
    public class AggregateExceptionBuilder
    {
        private List<Exception> exception = new List<Exception>();

        public bool HasException => this.exception.Any();

        /// <summary>
        /// 必須パラメータ未指定の例外を追加する
        /// </summary>
        /// <param name="name">パラメタ名</param>
        /// <returns>メソッドチェーンするための自インスタンス</returns>
        public AggregateExceptionBuilder AddCmdParamNull(string name)
        {
            this.exception.Add(new RequiredParamException(name, string.Format(MessageResource.MSG00002, name)));
            return this;
        }

        /// <summary>
        /// パラメータの指定値が不正の例外を追加する
        /// </summary>
        /// <param name="name">パラメタ名</param>
        /// <returns>メソッドチェーンするための自インスタンス</returns>
        public AggregateExceptionBuilder AddCmdParamInvalid(string name)
        {
            this.exception.Add(new InvalidParamValueException(string.Format(MessageResource.MSG00006), name));
            return this;
        }

        /// <summary>
        /// 複合的な例外を生成する
        /// </summary>
        /// <returns>例外</returns>
        public AggregateException Build()
        {
            return new AggregateException(this.exception);
        }
    }
}
