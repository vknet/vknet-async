namespace VkNetAsync.Service.Exception
{
	/// <summary>
	/// Исключение, которое выбрасывается при отсутствии авторизации на выполнение запрошенной операции.
	/// </summary>
	public class UserAuthorizationFailException : VkException
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="UserAuthorizationFailException"/> с указанным описанием и кодом ошибки.
		/// </summary>
		/// <param name="message">Описание исключения.</param>
		/// <param name="code">Код ошибки, полученный от сервера ВКонтакте.</param>
		public UserAuthorizationFailException(string message, int code) : base(message, code)
		{
		}

	}
}