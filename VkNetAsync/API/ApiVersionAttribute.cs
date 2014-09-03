using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace VkNetAsync.API
{
	/// <summary>
	/// Версия VK API, для которой реализован метод.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	internal class ApiVersionAttribute : Attribute
	{
		/// <summary>
		/// Версия VK API
		/// </summary>
		public string Version { get; private set; }

		/// <summary>
		/// Создает экземпляр атрибута <see cref="ApiVersionAttribute"/> с заданой версией API
		/// </summary>
		/// <param name="version">Версия API</param>
		public ApiVersionAttribute(string version)
		{
			Contract.Requires<FormatException>(Regex.IsMatch(version, @"[0-9]*.[0-9]*"));
			Version = version;
		}

	}
}