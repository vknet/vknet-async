using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkNetAsync.Annotations;
using VkNetAsync.API.VkTypes.Enums.Filters;
using VkNetAsync.Service.Captcha;
using VkNetAsync.Service.Exception;
using VkNetAsync.Service.Network;

namespace VkNetAsync.API.Authorisation
{
	public class ManualAuthoriser : Authoriser
	{
		public ManualAuthoriser([NotNull] INetworkTransport transport, [CanBeNull] ICaptchaResolver captchaResolver = null)
			: base(transport, captchaResolver)
		{
		}

		public async Task Authorise(long appId, [NotNull] string login, [NotNull] string password, [NotNull] Settings settings, CancellationToken cancellationToken = default(CancellationToken))
		{
			Contract.Requires<ArgumentOutOfRangeException>(appId > 0);
			Contract.Requires<ArgumentNullException>(login != null);
			Contract.Requires<ArgumentException>(login.Length > 0);
			Contract.Requires<ArgumentNullException>(password != null);
			Contract.Requires<ArgumentException>(password.Length > 0);
			Contract.Requires<ArgumentNullException>(settings != null);

			await AuthoriseInternal(appId, login, password, settings, cancellationToken).ConfigureAwait(false);
		}

		// Шаги авторизации
		private async Task AuthoriseInternal(long appId, string login, string password, Settings settings, CancellationToken cancellationToken)
		{
			var loginPageUri = new Uri(
				string.Format(@"https://oauth.vk.com/authorize?client_id={0}&scope={1}&redirect_uri=https://oauth.vk.com/blank.html&display=mobile&v=5.24&response_type=token",
							  appId, settings.Value));
			Response loginResponse = await Transport.Get(loginPageUri, cancellationToken).ConfigureAwait(false);

			ThrowIfLoginPageIncorrect(await Transport.Get(loginPageUri, cancellationToken).ConfigureAwait(false));
	
			Captcha captcha = null;
			do
			{
				var loginParameters = GetLoginRequestParametersFromLoginPage(loginResponse.ResponseData, login, password);
				
				if (captcha != null)
					loginParameters = ResolveCaptcha(captcha, loginParameters);
				
				loginResponse = await Transport.Post(loginParameters.Item1, loginParameters.Item2, cancellationToken).ConfigureAwait(false);
				ThrowIfLoginFailed(loginResponse);
				
				captcha = TryGetCaptchaFromPage(loginResponse);
			}
			while (captcha != null);


			var grantAccessUri = GetGrantAccessUriFromPermissionsPage(loginResponse.ResponseData);
			var accessUri = (await Transport.Post(grantAccessUri, null, cancellationToken).ConfigureAwait(false)).Uri;

			AuthoriseFromUri(accessUri);
		}

		private Captcha TryGetCaptchaFromPage(Response response)
		{
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(response.ResponseData);
			HtmlNode form = htmlDocument.DocumentNode.Descendants("form").Single().ParentNode;

			var sidNode = form.Descendants("input")
										   .SingleOrDefault(node => node.Attributes["name"] != null && node.Attributes["name"].Value == "captcha_sid");
			if (sidNode == null)
				return null;

			long sid = long.Parse(sidNode.Attributes["value"].Value);

			string imgString = form.Descendants("img")
								   .Single(node => node.Attributes["class"].Value == "captcha_img" && node.Attributes["id"].Value == "captcha")
								   .Attributes["src"].Value;
			
			Contract.Assume(imgString != null);

			return new Captcha(sid, new Uri(imgString));
		}

		private Tuple<Uri, byte[]> ResolveCaptcha(Captcha captcha, Tuple<Uri, byte[]> loginParameters)
		{
			if(CaptchaResolver == null)
				throw new CaptchaNeededException("Captcha not resolved because there is no resolver at ManualAuthoriser. " +
												 "Use ManualAuthoriser constructor with capthaResolver parameter to resolve captchas.", captcha);
			string captchaAnswer;
			try
			{
				captchaAnswer = CaptchaResolver.Resolve(captcha);
			}
			catch (CaptchaResolvingCancelledException ex)
			{
				throw new CaptchaNeededException("Captcha needed and was not resolved.", captcha, ex);
			}

			if (!string.IsNullOrEmpty(captchaAnswer))
			{
				byte[] originalRequestParameters = loginParameters.Item2;
				byte[] captchParameters = Encoding.UTF8.GetBytes(string.Format(@"&captcha_sid={0}&captcha_key={1}", captcha.Sid, Uri.EscapeUriString(captchaAnswer)));

				var combinedParameters = new byte[originalRequestParameters.Length + captchParameters.Length];
				originalRequestParameters.CopyTo(combinedParameters, 0);
				captchParameters.CopyTo(combinedParameters, originalRequestParameters.Length);

				loginParameters = new Tuple<Uri, byte[]>(loginParameters.Item1, combinedParameters);
			}
			return loginParameters;
		}

		private static void ThrowIfLoginPageIncorrect(Response response)
		{
			try
			{
				JToken token = JToken.Parse(response.ResponseData);
				if (token["error"] != null && token["error_description"] != null)
					throw new AuthorizationFailedException(string.Format("Authorisation failed: {0}{1}", token["error"], token["error_description"]), 0);
				throw new AuthorizationFailedException(string.Format("Response: {0}", token.ToString(Formatting.None)), 0);
			}
			catch (JsonReaderException) { } // supress if response is not a json
		}

		private Tuple<Uri, byte[]> GetLoginRequestParametersFromLoginPage(string loginPage, string login, string password)
		{
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(loginPage);
			HtmlNode form = htmlDocument.DocumentNode.Descendants("form").Single().ParentNode;

			var uri = new Uri(form.Descendants("form").Single().Attributes["action"].Value);

			var parameters = new Dictionary<string, string>();

			foreach (var pair in form.Descendants("input").Where(node => node.Attributes["name"] != null)
									 .Select(node => new
													 {
														 Name = node.Attributes["name"].Value,
														 Value = (node.Attributes["value"] == null ? null : node.Attributes["value"].Value)
													 }))
			{
				var value = pair.Value;

				if (pair.Name == "email")
					value = login;
				if (pair.Name == "pass")
					value = password;

				parameters.Add(pair.Name, value);
			}
			Contract.Assume(parameters.ContainsKey("email"));
			Contract.Assume(parameters.ContainsKey("pass"));

			return new Tuple<Uri, byte[]>(
				uri,
				Encoding.UTF8.GetBytes(
									   string.Join("&",
												   parameters.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value))
										   )));
		}

		private static void ThrowIfLoginFailed(Response response)
		{
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(response.ResponseData);
			HtmlNode form = htmlDocument.DocumentNode.Descendants("form").Single().ParentNode;
			var serviceNode = form.Descendants("div").SingleOrDefault(node => node.Attributes["class"].Value == "service_msg service_msg_warning");
			if (serviceNode != null)
				throw new AuthorizationFailedException(string.Format("Authorisation failed: {0}", serviceNode.InnerText), 0);
		}


		private Uri GetGrantAccessUriFromPermissionsPage(string permissionsPage)
		{
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(permissionsPage);
			HtmlNode form = htmlDocument.DocumentNode.Descendants("form").SingleOrDefault();

			Contract.Assume(form != null);

			return new Uri(form.Attributes["action"].Value);
		}

		private void AuthoriseFromUri(Uri accessUri)
		{
			var path = Regex.Escape("https://oauth.vk.com/blank.html");
			var numberSign = Regex.Escape("#");
			var questionMark = Regex.Escape("?");
			var separator = Regex.Escape("&");
			var uri = accessUri.ToString();

			var successfulPattern =
				string.Format(@"^{0}{1}access_token=(?<token>[0-9a-f]+){2}expires_in=(?<expires>[0-9]+){2}user_id=(?<uid>[0-9]+)$",
							  path, numberSign, separator);

			if (Regex.IsMatch(uri, successfulPattern))
			{
				var match = Regex.Match(uri, successfulPattern);
				string token = match.Groups["token"].ToString();
				long uid = long.Parse(match.Groups["uid"].ToString());

				API = FromToken(uid, token, Transport, CaptchaResolver);
				return;
			}

			var failPattern = string.Format(@"^{0}{1}error=(?<error>[^{2}]+){2}error_description=(?<desc>.+)$", path, questionMark, separator);
			if (Regex.IsMatch(uri, failPattern))
			{
				var match = Regex.Match(uri, successfulPattern);
				string error = match.Groups["error"].ToString();
				string desc = match.Groups["desc"].ToString();

				throw new AuthorizationFailedException(error + ": " + desc, 0);
			}
			Contract.Assume(false);
		}
	}
}