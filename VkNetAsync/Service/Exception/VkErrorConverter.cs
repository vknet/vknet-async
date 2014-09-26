using System;
using System.Diagnostics.Contracts;
using Newtonsoft.Json.Linq;

namespace VkNetAsync.Service.Exception
{
	public static class VkErrorConverter
	{
		public static VkException Convert(JObject error)
		{
			Contract.Requires<ArgumentNullException>(error != null);

			var code = (int)error["error_code"];
			var message = (string)error["error_msg"];

			switch (code)
			{
				case 5:
					return new AuthorizationFailedException(message, code);

				case 14:
					var sid = (long)error["captcha_sid"];
					var img = (string)error["captcha_img"];
					return new CaptchaNeededException(message, new Captcha.Captcha(sid, new Uri(img)));

				case 4:	  // Incorrect signature.
				case 100: // One of the parameters specified was missing or invalid.
				case 113: // Invalid user id.
				case 120: // Invalid message.
				case 125: // Invalid group id.
					return new InvalidParameterException(message, code);

				case 6:   // Too many requests per second.
					return new TooManyRequestsException(message, code);

				case 7:   // Permission to perform this action is denied by user.
				case 15:  // Access denied: 1) groups list of this user are under privacy.	2) cannot blacklist yourself	
				case 148: // Access to the menu of the user denied
				case 170: // Access to user's friends list denied.
				case 201: // Access denied.
				case 203: // Access to the group is denied.
				case 220: // Access to status denied.
				case 221: // User disabled track name broadcast.
				case 260: // Access to the groups list is denied due to the user's privacy settings.
				case 500: // Permission denied. You must enable votes processing in application settings.
					return new AccessDeniedException(message, code);

				default:
					return new VkException(message, code);
			}
		}
	}
}