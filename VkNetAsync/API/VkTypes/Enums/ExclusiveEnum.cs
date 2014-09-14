using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using VkNetAsync.Annotations;

namespace VkNetAsync.API.VkTypes.Enums
{
	public class ExclusiveEnum<TExclusive> where TExclusive : ExclusiveEnum<TExclusive>, new()
	{
		private sealed class EnumMember
		{
			private readonly long _value;
			public long Value
			{
				get { return _value; }
			}

			private readonly string _name;
			public string Name
			{
				get { return _name; }
			}

			internal EnumMember(long value, string name)
			{
				_value = value;
				_name = name;
			}

			public override string ToString()
			{
				return string.Format("{0} ({1})", Name, Value);
			}
		}
		
		#region Static members

		// ReSharper disable once StaticFieldInGenericType
		private static readonly IList<TExclusive> _possibleValues = new List<TExclusive>();

		protected static IList<TExclusive> PossibleValues
		{
			get { return new ReadOnlyCollection<TExclusive>(_possibleValues); }
		}

		protected static TExclusive RegisterPossibleValue(long value, [NotNull] string name)
		{
			Contract.Requires<ArgumentNullException>(name != null);
			Contract.Requires<ArgumentException>(name.Length > 0);

			if (_possibleValues.Any(member => member.Value == value))
				throw new ArgumentException(string.Format("Element of type {0} with value {1} is already registered ({2}).",
														  typeof(TExclusive).FullName, value, FromValue(value)), 
											"value");

			if (_possibleValues.Any(member => member.Name == name))
				throw new ArgumentException(string.Format("Element of type {0} with name {1} is not registered ({2}).",
														  typeof(TExclusive).FullName, name, FromName(name)),
											"name");

			_possibleValues.Add(new TExclusive { _member = new EnumMember(value, name) });

			return FromValue(value);
		}

		protected static TExclusive FromValue(long value)
		{
			var enumMember = _possibleValues.SingleOrDefault(member => member.Value == value);
			if (enumMember == null)
				throw new ArgumentOutOfRangeException("value", string.Format("Element of type {0} with value {1} is not registered.", typeof (TExclusive).FullName, value));
			
			return enumMember;
		}

		protected static TExclusive FromName([NotNull] string name)
		{
			Contract.Requires<ArgumentNullException>(name != null);
			Contract.Requires<ArgumentException>(name.Length > 0);

			var enumMember = _possibleValues.SingleOrDefault(member => member.Name == name);
			if (enumMember == null)
				throw new ArgumentOutOfRangeException("name", string.Format("Element of type {0} with name {1} is not registered.", typeof(TExclusive).FullName, name));

			return enumMember;
		}

		#endregion

		private EnumMember _member;

		public long Value
		{
			get { return _member.Value; }
		}

		public string Name
		{
			get { return _member.Name; }
		}

		protected ExclusiveEnum()
		{
			_member = null;
		}

		public override string ToString()
		{
			return _member.ToString();
		}

		public static bool operator ==(ExclusiveEnum<TExclusive> left, ExclusiveEnum<TExclusive> right)
		{
			if (ReferenceEquals(right, left)) return true;
			if (ReferenceEquals(null, left)) return false;
			if (ReferenceEquals(null, right)) return false;

			return left._member.Value == right._member.Value;
		}

		public static bool operator !=(ExclusiveEnum<TExclusive> left, ExclusiveEnum<TExclusive> right)
		{
			return !(left == right);
		}

		protected bool Equals(ExclusiveEnum<TExclusive> other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ExclusiveEnum<TExclusive>)obj);
		}

		public override int GetHashCode()
		{
			return _member.Value.GetHashCode();
		}

	}
}