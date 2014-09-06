using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace VkNetAsync.Service
{
	public class VkParameters : IEnumerable<KeyValuePair<string, string>>
	{
		private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();

		public void Add<T>(string name, T value)
		{
			Contract.Requires<ArgumentNullException>(name != null);
			Contract.Requires<ArgumentException>(name.Length > 0);

			if (value == null)
				return;

			if (value is bool)
			{
				_parameters.Add(name, (bool)(object)value ? "1" : "0");
				return;
			}

			if (value is DateTime)
			{
				Add(name, ((DateTime)(object)value).ToUnixTime());
				return;
			}

			if (value is Enum)
			{
				_parameters.Add(name, (value as Enum).ToString("d"));
				return;
			}

			var stringValue = value.ToString();
			if (!string.IsNullOrEmpty(stringValue))
				_parameters.Add(name, stringValue);
		}

		public override string ToString()
		{
			return string.Join("&", _parameters.Select(pair => pair.Key + "=" + pair.Value));
		}


		#region Delegated from dictionary methods

		public int Count
		{
			get { return _parameters.Count; }
		}

		public bool ContainsKey(string key)
		{
			return _parameters.ContainsKey(key);
		}

		public string this[string key]
		{
			get { return _parameters[key]; }
			set { _parameters[key] = value; }
		}

		public void Remove(string key)
		{
			Contract.Requires<ArgumentNullException>(key != null);
			_parameters.Remove(key);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}

		#endregion
	}
}