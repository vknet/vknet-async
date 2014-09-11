namespace VkNetAsync.Service.Exception
{
	/// <summary>
    /// Исключение, которые выбрасывается при попытке выполнить запрос с частотой, превышающей максимально допустимую ВКонтакте.
    /// В настоящее время действует ограничение (один раз в три секунды) на количество однотипных запросов (вызовов методов 
    /// с одним и тем же именем). Если это ограничение превышается, то сервер ВКонтакте возвращает ошибку с кодом 6 - 
    /// Too many requests per second.
    /// </summary>
    public class TooManyRequestsException : VkException
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="TooManyRequestsException"/>.
        /// </summary>
        public TooManyRequestsException()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="TooManyRequestsException"/> с указанным описанием и кодом ошибки.
        /// </summary>
        /// <param name="message">Описание исключения.</param>
        /// <param name="code">Код ошибки, полученный от сервера ВКонтакте.</param>
        public TooManyRequestsException(string message, int code) : base(message, code)
        {
        }
    }
}