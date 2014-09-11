namespace VkNetAsync.Service.Exception
{
	/// <summary>
    /// Исключение, которое выбрасывается в случае, если параметр принимал недействительное значение.
    /// </summary>
    public class InvalidParameterException : VkException
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="InvalidParameterException"/>.
        /// </summary>
        public InvalidParameterException()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="InvalidParameterException"/> с указанным описанием и кодом ошибки.
        /// </summary>
        /// <param name="message">Описание исключения.</param>
        /// <param name="code">Код ошибки, полученный от сервера ВКонтакте.</param>
        public InvalidParameterException(string message, int code) : base(message, code)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="InvalidParameterException"/> с указанным описанием, кодом ошибки и внутренним исключением.
        /// </summary>
        /// <param name="message">Описание исключения.</param>
        /// <param name="code">Код ошибки, полученный от сервера ВКонтакте.</param>
        /// <param name="innerException">Внутреннее исключение.</param>
        public InvalidParameterException(string message, int code, System.Exception innerException) : base(message, code, innerException)
        {
        }
    }
}