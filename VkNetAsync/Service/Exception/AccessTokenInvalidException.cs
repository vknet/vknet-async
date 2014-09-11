namespace VkNetAsync.Service.Exception
{
	/// <summary>
    /// Исключение, которое выбрасывается, в случае, если предоставленный маркер доступа является недействительным.
    /// </summary>
    public class AccessTokenInvalidException : VkException
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AccessTokenInvalidException"/>.
        /// </summary>
        public AccessTokenInvalidException()
        {
        }

		public AccessTokenInvalidException(string message, int code) : base(message, code)
		{
		}

		public AccessTokenInvalidException(string message, int code, System.Exception innerException) : base(message, code, innerException)
		{
		}
    }
}