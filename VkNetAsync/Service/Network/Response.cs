using System;
using System.Diagnostics.Contracts;

namespace VkNetAsync.Service.Network
{
	public class Response
	{
		private readonly Uri _uri;
		private readonly string _responseData;

		public Uri Uri
		{
			get { return _uri; }
		}

		public string ResponseData
		{
			get { return _responseData; }
		}

		public Response(Uri uri, string responseData)
		{
			Contract.Requires<ArgumentNullException>(uri != null);
			Contract.Requires<ArgumentNullException>(responseData != null);
			
			_uri = uri;
			_responseData = responseData;
		}
	}
}