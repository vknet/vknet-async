namespace VkNetAsync.Service.Exception
{
	/// <summary>
    /// Исключение, которое выбрасывается при неудачной попытке авторизации.
    /// </summary>
    
    public class AuthorizationFailedException : VkException
    {
       /// <summary>
		/// Инициализирует новый экземпляр класса <see cref="AuthorizationFailedException"/>.
		/// </summary>
		/// <param name="message">Описание исключения.</param>
		/// <param name="code">Код ошибки, полученный от сервера ВКонтакте.</param>
		public AuthorizationFailedException(string message, int code) : base(message, code)
        {
        }
    }
}