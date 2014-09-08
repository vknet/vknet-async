using System;
using System.Runtime.Serialization;

namespace VkNetAsync.Service.Exception
{
	/// <summary>
    /// Исключение, которое выбрасывается, в случае, если предоставленный маркер доступа является недействительным.
    /// </summary>
    [Serializable]
    public class AccessTokenInvalidException : VkException
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AccessTokenInvalidException"/>.
        /// </summary>
        public AccessTokenInvalidException()
        {
        }

		public AccessTokenInvalidException(string message, int code) : base(message, code)
		{
		}

		public AccessTokenInvalidException(string message, int code, System.Exception innerException) : base(message, code, innerException)
		{
		}

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="AccessTokenInvalidException"/> на основе ранее сериализованных данных.
		/// </summary>
		/// <param name="info">Содержит все данные, необходимые для десериализации.</param>
		/// <param name="context">Описывает источник и назначение данного сериализованного потока и предоставляет дополнительный, 
		/// определяемый вызывающим, контекст.</param>
		protected AccessTokenInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
    }
}