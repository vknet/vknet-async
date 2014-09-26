using System.Diagnostics.Contracts;

namespace VkNetAsync.Service.Captcha
{
	[ContractClass(typeof (ICaptchaResolverContract))]
	public interface ICaptchaResolver
	{
		string Resolve(Captcha captcha);
	}
}