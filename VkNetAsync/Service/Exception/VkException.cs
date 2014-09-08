using System.Runtime.Serialization;
using VkNetAsync.Annotations;

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
		}

		public VkException(string message, int code, System.Exception innerException) : base(message, innerException)
		{
		}

		protected VkException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}