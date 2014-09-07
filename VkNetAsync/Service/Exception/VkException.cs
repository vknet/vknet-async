using System.Runtime.Serialization;
using VkNetAsync.Annotations;

namespace VkNetAsync.Service.Exception
{
	public class VkException : System.Exception
	{
		public VkException()
		{
		}

		public VkException(string message) : base(message)
		{
		}

		public VkException(string message, System.Exception innerException) : base(message, innerException)
		{
		}

		protected VkException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}