using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using VkNetAsync.Annotations;
using VkNetAsync.API.VkTypes.Enums.Filters;
using VkNetAsync.Service.Exception;
using VkNetAsync.Service.Network;

namespace VkNetAsync.API.Authorisation
{
	public class ManualAuthoriser : Authoriser
	{
		public ManualAuthoriser([NotNull] INetworkTransport transport)
			: base(transport)
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

			await AuthoriseInternal(appId, login, password, settings, cancellationToken);
		}

		private async Task AuthoriseInternal(long appId, string login, string password, Settings settings,
											 CancellationToken cancellationToken)
		{
			var loginPageUri = new Uri(
				string.Format(@"https://oauth.vk.com/authorize?client_id={0}&scope={1}&redirect_uri=https://oauth.vk.com/blank.html&display=mobile&v=5.24&response_type=token",
							  appId, settings.Value));

			var loginPage = await Transport.Get(loginPageUri, cancellationToken);
			var loginParameters = GetLoginRequestParametersFromLoginPage(loginPage.ResponseData, login, password);

			var permissionsPage = await Transport.Post(loginParameters.Item1, loginParameters.Item2, cancellationToken);
			var grantAccessUri = GetGrantAccessUriFromPermissionsPage(permissionsPage.ResponseData);

			var accessUri = (await Transport.Post(grantAccessUri, null, cancellationToken)).Uri;
			
			AuthoriseFromUri(accessUri);
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

				API = FromToken(uid, token, Transport);
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

		private Uri GetGrantAccessUriFromPermissionsPage(string permissionsPage)
		{
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(permissionsPage);
			HtmlNode form = htmlDocument.DocumentNode.Descendants("form").SingleOrDefault();

			Contract.Assume(form != null);

			return new Uri(form.Attributes["action"].Value);
		}
	}
}