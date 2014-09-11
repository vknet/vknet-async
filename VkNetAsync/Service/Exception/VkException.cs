namespace VkNetAsync.Service.Exception
{
	public class VkException : System.Exception
	{
		/// <summary>
		/// Код ошибки, полученный от сервера ВКонтакте.
		/// </summary>
		public int ErrorCode { get; private set; }

		public VkException()
		{
		}

		public VkException(string message, int code) : base(message)
		{
			ErrorCode = code;
		}

		public VkException(string message, int code, System.Exception innerException) : base(message, innerException)
		{
			ErrorCode = code;
		}
	}
}