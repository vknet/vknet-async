using System;
using System.Diagnostics.Contracts;
using VkNetAsync.Annotations;

namespace VkNetAsync.Service.Exception
{
	/// <summary>
    /// Исключение, выбрасываемое при необходимости ввода капчи для вызова метода
    /// </summary>
    public class CaptchaNeededException : VkException
    {
        /// <summary>
        /// Идентификатор капчи
        /// </summary>
        public long Sid { get; private set; }

        /// <summary>
        /// Url-адрес изображения с капчей
        /// </summary>
        public Uri Img { get; private set; }


		/// <summary>
		/// Создания экземпляра <see cref="CaptchaNeededException"/>
		/// </summary>
		/// <param name="message">Описание исключения.</param>
		/// <param name="code">Код ошибки, полученный от сервера ВКонтакте.</param>
		/// <param name="sid">Сид</param>
		/// <param name="img">Url-адрес изображения с капчей</param>
		public CaptchaNeededException(string message, int code, long sid, [NotNull] Uri img) : base(message, code)
		{
			Contract.Requires<ArgumentNullException>(img != null);	
			
			Sid = sid;
            Img = img;
		}
    }
}