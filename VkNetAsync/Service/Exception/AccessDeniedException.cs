namespace VkNetAsync.Service.Exception
{
	/// <summary>
	/// Исключение, которое выбрасывается при отказе в доступе на выполнение операции сервером ВКонтакте.
	/// </summary>
	public class AccessDeniedException : VkException
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="AccessDeniedException"/>.
		/// </summary>
		public AccessDeniedException()
		{
		}

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="AccessDeniedException"/> с указанным описанием и кодом ошибки.
		/// </summary>
		/// <param name="message">Описание исключения.</param>
		/// <param name="code">Код ошибки, полученный от сервера ВКонтакте.</param>
		public AccessDeniedException(string message, int code) : base(message, code)
		{
		}

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="AccessDeniedException"/> с указанным описанием, кодом ошибки и внутренним исключением.
		/// </summary>
		/// <param name="message">Описание исключения.</param>
		/// <param name="code">Код ошибки, полученный от сервера ВКонтакте.</param>
		/// <param name="innerException">Внутреннее исключение.</param>
		public AccessDeniedException(string message, int code, System.Exception innerException)	: base(message, code, innerException)
		{
		}
	}
}