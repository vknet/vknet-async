using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using VkNetAsync.Service;

namespace VkNetAsync.Tests.Service
{
	[TestFixture]
	public class VkParametersTests
	{
		[Test]
		public void Add_NullParameter_NotAddedToParameters()
		{
			var parameters = new VkParameters { { "paramName", (object) null } };
			Assert.That(parameters.ToString(), Is.Null.Or.Empty);
		}

		[Test]
		public void Add_NullableParameter_Null_NotAddedToParameters()
		{
			var parameters = new VkParameters { { "paramName", (int?) null } };
			Assert.That(parameters.ToString(), Is.Null.Or.Empty);
		}

		[Test]
		public void Add_NullableParameter_NotNull_AddedToParameters()
		{
			var parameters = new VkParameters { { "paramName", (int?) 10 } };
			Assert.That(parameters.ToString(), Is.EqualTo("paramName=10"));
		}

		[Test]
		public void Add_BoolParameter_False_InterpretedAsZero()
		{
			var parameters = new VkParameters { { "paramName", false } };
			Assert.That(parameters.ToString(), Is.EqualTo("paramName=0"));
		}

		[Test]
		public void Add_BoolParameter_True_InterpretedAsOne()
		{
			var parameters = new VkParameters { { "paramName", true } };
			Assert.That(parameters.ToString(), Is.EqualTo("paramName=1"));
		}

		[Test]
		public void Add_DateTimeParameter_InterpretedAsUnixtime()
		{
			var dateTime = DateTime.UtcNow;
			
			var parameters = new VkParameters { { "paramName", dateTime } };
			Assert.That(parameters.ToString(), Is.EqualTo(string.Format("paramName={0}", dateTime.ToUnixTime())));
		}

		[Test]
		public void Add_ReferenceTypeParameter_NotNull_AddedToParameters()
		{
			var parameters = new VkParameters { { "paramName", "stringValue" } };
			Assert.That(parameters.ToString(), Is.EqualTo("paramName=stringValue"));
		}

		[Test]
		public void Add_EnumParameter_AddedToParameters()
		{
			var enumValue = DateTimeKind.Local;
			var parameters = new VkParameters { { "paramName", enumValue } };
			Assert.That(parameters.ToString(), Is.EqualTo(string.Format("paramName={0}", enumValue.ToString("d"))));
		}

		[Test]
		public void ToString_CombineMultiple_Successful()
		{
			var parameters = new VkParameters
							 {
								 { "param1", 10 },
								 { "param2", true },
								 { "param3", "value" }
							 };

			Assert.That(parameters.ToString(), Is.EqualTo("param1=10&param2=1&param3=value"));
		}



		[Test]
		public void Add_MultipleParameters_WithSameNames_ThrowsException()
		{
			const string paramName = "paramName";
			
			var parameters = new VkParameters();
			parameters.Add(paramName, false);
			
			Assert.Throws<ArgumentException>(() => parameters.Add(paramName, false));
		}

		[Test]
		public void Count_HasCorrectValue()
		{
			var parameters = new VkParameters
							 {
								 { "param1", 1 },
								 { "param2", true },
								 { "param3", "value" }
							 };

			Assert.That(parameters.Count, Is.EqualTo(3));
		}

		[Test]
		public void ContainsKey_ReturnsCorrectValue()
		{
			var parameters = new VkParameters
							 {
								 { "param1", 10 },
								 { "param2", true },
								 { "param3", "value" }
							 };

			Assert.That(parameters.ContainsKey("param2"), Is.True);
			
			Assert.That(parameters.ContainsKey("param4"), Is.False);
		}

		[Test]
		public void Indexer_ReturnsCorrectValue()
		{
			var parameters = new VkParameters
							 {
								 { "param1", 10 },
								 { "param2", true },
								 { "param3", "value" }
							 };

			Assert.That(parameters["param1"], Is.EqualTo("10"));
			Assert.That(parameters["param2"], Is.EqualTo("1"));
			Assert.That(parameters["param3"], Is.EqualTo("value"));
		}

		[Test]
		public void Indexer_KeyNotExists_ThrowsKeyNotFound()
		{
			var parameters = new VkParameters { { "param1", 10 } };

			Assert.Throws<KeyNotFoundException>(() => parameters["param4"].ToString());
		}


		[Test]
		public void Remove_RemovesExistedParameter()
		{
			var parameters = new VkParameters
							 {
								 { "param1", 10 },
								 { "param2", true },
								 { "param3", "value" }
							 };

			parameters.Remove("param2");
			Assert.That(parameters.Count, Is.EqualTo(2));
			Assert.That(!parameters.ContainsKey("param2"));
		}

		[Test]
		public void Remove_ParameterNotExists_NotThrows()
		{
			var parameters = new VkParameters
							 {
								 { "param1", 10 },
								 { "param2", true },
								 { "param3", "value" }
							 };

			Assert.DoesNotThrow(() => parameters.Remove("param5"));
		}
	}
}