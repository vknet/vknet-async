using Newtonsoft.Json.Linq;

namespace VkNetAsync.Service
{
	public class VkResponse
	{
		private readonly JToken _token;

		public VkResponse(JToken response)
		{
			_token = response;
		}

		public T ToObject <T>()
		{
			return _token.ToObject<T>();
		}
	}
}