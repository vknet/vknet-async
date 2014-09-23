using NUnit.Framework;
using VkNetAsync.API.Authorisation;
using VkNetAsync.API.VkTypes.Enums.Filters;
using VkNetAsync.Service.Exception;
using VkNetAsync.Service.Network;

namespace VkNetAsync.Tests.API.Authorisation
{
	[TestFixture]
	public class ManualAuthoriserTests
	{
		[Test]
		public void Authorise_IncorrectApplicationId_ThrowsAuthorizationFailedException()
		{
			var authoriser = new ManualAuthoriser(new NetworkTransport());

			Assert.Throws<AuthorizationFailedException>(async () => await authoriser.Authorise(long.MaxValue, "login", "password", Settings.AllOffline));
		}

		[Test]
		public void Authorise_IncorrectCombinationOfApplicationIdAndScope_ThrowsAuthorizationFailedException()
		{
			var authoriser = new ManualAuthoriser(new NetworkTransport());

			Assert.Throws<AuthorizationFailedException>(async () => await authoriser.Authorise(1, "login", "password", Settings.AllOffline));
		}

		[Test]
		public void Authorise_IncorrectLoginOrPassword_ThrowsAuthorizationFailedException()
		{
			var authoriser = new ManualAuthoriser(new NetworkTransport());

			Assert.Throws<AuthorizationFailedException>(async () => await authoriser.Authorise(4527865, "login", "password", Settings.AllOffline));
		}
	}
}