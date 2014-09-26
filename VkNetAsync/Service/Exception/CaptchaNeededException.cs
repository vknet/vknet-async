using System;
using System.Diagnostics.Contracts;
using VkNetAsync.Annotations;
using VkNetAsync.Service.Captcha;

namespace VkNetAsync.Service.Exception
{
	/// <summary>
    /// Исключение, выбрасываемое при необходимости ввода капчи для вызова метода
    /// </summary>
    public class CaptchaNeededException : VkException
    {
        /// <summary>
        /// Капча
        /// </summary>
		public Captcha.Captcha Captcha { get; private set; }


		/// <summary>
		/// Создания экземпляра <see cref="CaptchaNeededException"/>
		/// </summary>
		/// <param name="message">Описание исключения</param>
		/// <param name="captcha">Описание капчи</param>
		/// <param name="innerException">Исключение, возникновение которого породило данное исключение</param>
		public CaptchaNeededException(string message, [NotNull] Captcha.Captcha captcha, System.Exception innerException = null) 
			: base(message, 14, innerException)  // TODO: introduce code 14 and other vk error codes to an enum
		{
			Contract.Requires<ArgumentNullException>(captcha != null);

			Captcha = captcha;

		}
    }
}