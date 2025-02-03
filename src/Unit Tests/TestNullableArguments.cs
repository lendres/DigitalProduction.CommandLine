using DigitalProduction.CommandLine;

namespace UnitTests;

public class TestNullableArguments
{
	readonly QuotationInfo _quotationInfo = new('\"');

	public TestNullableArguments()
	{
		_quotationInfo.AddEscapeCode('\"', '\"');
	}

	private static T GetArgumentsInstance<T>(string commandLineString) where T : class, new()
	{
		T nullableArguments	= new();
		try
		{
			CommandLineParser parser = new(nullableArguments);
			Assert.NotNull(parser);
			parser.Parse(commandLineString, false);
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine(exception.ToString());
		}
		return nullableArguments;
	}

	private static CommandLineParser GetParser<T>(string commandLineString) where T : class, new()
	{
		T nullableArguments	= new();
		try
		{
			CommandLineParser parser = new(nullableArguments);
			Assert.NotNull(parser);
			parser.Parse(commandLineString, false);
			return parser;
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine(exception.ToString());
			throw;
		}
	}

	/// <summary>
	/// String.
	/// </summary>
	[Fact]
	public void StringTest()
	{
		StringManager arguments = GetArgumentsInstance<StringManager>("-filename \"Test File.txt\"");
		Assert.Equal("Test File.txt", arguments.FileName);
	}

	[Fact]
	public void NullableStringTest()
	{
		NullableStringManager arguments = GetArgumentsInstance<NullableStringManager>("-filename \"Test File.txt\"");
		Assert.Equal("Test File.txt", arguments.FileName);

		arguments = GetArgumentsInstance<NullableStringManager>("");
		Assert.Null(arguments.FileName);
	}

	/// <summary>
	/// Boolean.
	/// </summary>
	[Fact]
	public void BoolTest()
	{
		BoolManager arguments = GetArgumentsInstance<BoolManager>("-run");
		Assert.True(arguments.Run);
	}

	[Fact]
	public void NullableBoolTest()
	{
		NullableBoolManager arguments = GetArgumentsInstance<NullableBoolManager>("-run");
		Assert.True(arguments.Run);

		arguments = GetArgumentsInstance<NullableBoolManager>("");
		Assert.Null(arguments.Run);
	}

	/// <summary>
	/// Byte.
	/// </summary>
	[Fact]
	public void ByteTest()
	{
		ByteManager arguments = GetArgumentsInstance<ByteManager>("-value 77");
		Assert.Equal(77, arguments.Value);
	}

	[Fact]
	public void NullableByteTest()
	{
		NullableByteManager arguments = GetArgumentsInstance<NullableByteManager>("-value 77");
		Assert.Equal((byte)77, arguments.Value);

		arguments = GetArgumentsInstance<NullableByteManager>("");
		Assert.Null(arguments.Value);
	}

	/// <summary>
	/// Signed byte.
	/// </summary>
	[Fact]
	public void ShortByteTest()
	{
		ShortByteManager arguments = GetArgumentsInstance<ShortByteManager>("-value \"-77\"");
		Assert.Equal((sbyte)(-77), arguments.Value);
	}

	[Fact]
	public void NullableShortByteTest()
	{
		NullableShortByteManager arguments = GetArgumentsInstance<NullableShortByteManager>("-value \"-77\"");
		Assert.Equal((sbyte)(-77), arguments.Value);

		arguments = GetArgumentsInstance<NullableShortByteManager>("");
		Assert.Null(arguments.Value);
	}

	/// <summary>
	/// Char.
	/// </summary>
	[Fact]
	public void CharTest()
	{
		CharManager arguments = GetArgumentsInstance<CharManager>("-value x");
		Assert.Equal('x', arguments.Value);

		CommandLineParser parser = GetParser<CharManager>("-value xx");
		Assert.Equal("The value \"xx\" is not valid for option \"value\"; Character options must be a single character.", parser.Errors.First().Message);
	}

	[Fact]
	public void NullableCharTest()
	{
		NullableCharManager arguments = GetArgumentsInstance<NullableCharManager>("-value x");
		Assert.Equal('x', arguments.Value);

		arguments = GetArgumentsInstance<NullableCharManager>("");
		Assert.Null(arguments.Value);
	}

	/// <summary>
	/// Intger.
	/// </summary>
	[Fact]
	public void IntTest()
	{
		IntManager arguments = GetArgumentsInstance<IntManager>("-value \"-77\"");
		Assert.Equal(-77, arguments.Value);
	}

	[Fact]
	public void NullableIntTest()
	{
		NullableIntManager arguments = GetArgumentsInstance<NullableIntManager>("-value \"-77\"");
		Assert.Equal(-77, arguments.Value);

		arguments = GetArgumentsInstance<NullableIntManager>("");
		Assert.Null(arguments.Value);
	}

	/// <summary>
	/// Unsigned integer.
	/// </summary>
	[Fact]
	public void UIntTest()
	{
		UIntManager arguments = GetArgumentsInstance<UIntManager>("-value 77");
		Assert.Equal(77u, arguments.Value);
	}

	[Fact]
	public void NullableUIntTest()
	{
		NullableUIntManager arguments = GetArgumentsInstance<NullableUIntManager>("-value 77");
		Assert.Equal(77u, arguments.Value);

		arguments = GetArgumentsInstance<NullableUIntManager>("");
		Assert.Null(arguments.Value);
	}

	/// <summary>
	/// Short integer.
	/// </summary>
	[Fact]
	public void ShortTest()
	{
		ShortManager arguments = GetArgumentsInstance<ShortManager>("-value \"-77\"");
		Assert.Equal(-77, arguments.Value);
	}

	[Fact]
	public void NullableShortTest()
	{
		NullableShortManager arguments = GetArgumentsInstance<NullableShortManager>("-value \"-77\"");
		Assert.Equal((short)-77, arguments.Value);

		arguments = GetArgumentsInstance<NullableShortManager>("");
		Assert.Null(arguments.Value);
	}

	/// <summary>
	/// Unsigned short integer.
	/// </summary>
	[Fact]
	public void UShortTest()
	{
		UShortManager arguments = GetArgumentsInstance<UShortManager>("-value 77");
		Assert.Equal(77, arguments.Value);
	}

	[Fact]
	public void NullableUShortTest()
	{
		NullableUShortManager arguments = GetArgumentsInstance<NullableUShortManager>("-value 77");
		Assert.Equal((ushort)77, arguments.Value);

		arguments = GetArgumentsInstance<NullableUShortManager>("");
		Assert.Null(arguments.Value);
	}

	/// <summary>
	/// Long integer.
	/// </summary>
	[Fact]
	public void LongTest()
	{
		LongManager arguments = GetArgumentsInstance<LongManager>("-value \"-77\"");
		Assert.Equal(-77L, arguments.Value);
	}

	[Fact]
	public void NullableLongTest()
	{
		NullableLongManager arguments = GetArgumentsInstance<NullableLongManager>("-value \"-77\"");
		Assert.Equal(-77L, arguments.Value);

		arguments = GetArgumentsInstance<NullableLongManager>("");
		Assert.Null(arguments.Value);
	}

	/// <summary>
	/// Unsigned long integer.
	/// </summary>
	[Fact]
	public void ULongTest()
	{
		ULongManager arguments = GetArgumentsInstance<ULongManager>("-value 77");
		Assert.Equal(77UL, arguments.Value);
	}

	[Fact]
	public void NullableULongTest()
	{
		NullableULongManager arguments = GetArgumentsInstance<NullableULongManager>("-value 77");
		Assert.Equal(77UL, arguments.Value);

		arguments = GetArgumentsInstance<NullableULongManager>("");
		Assert.Null(arguments.Value);
	}

	/// <summary>
	/// Double.
	/// </summary>
	[Fact]
	public void DoubleTest()
	{
		DoubleManager arguments = GetArgumentsInstance<DoubleManager>("-value 77.5");
		Assert.Equal(77.5, arguments.Value);
	}

	[Fact]
	public void NullableDoubleTest()
	{
		NullableDoubleManager arguments = GetArgumentsInstance<NullableDoubleManager>("-value 77.5");
		Assert.Equal(77.5, arguments.Value);

		arguments = GetArgumentsInstance<NullableDoubleManager>("");
		Assert.Null(arguments.Value);
	}

	/// <summary>
	/// Float.
	/// </summary>
	[Fact]
	public void FloatTest()
	{
		FloatManager arguments = GetArgumentsInstance<FloatManager>("-value 77.5");
		Assert.Equal(77.5f, arguments.Value);
	}

	[Fact]
	public void NullableFloatTest()
	{
		NullableFloatManager arguments = GetArgumentsInstance<NullableFloatManager>("-value 77.5");
		Assert.Equal(77.5f, arguments.Value);

		arguments = GetArgumentsInstance<NullableFloatManager>("");
		Assert.Null(arguments.Value);
	}

	/// <summary>
	/// Decimal.
	/// </summary>
	[Fact]
	public void DecimalTest()
	{
		DecimalManager arguments = GetArgumentsInstance<DecimalManager>("-value 77.5");
		Assert.Equal(77.5m, arguments.Value);
	}

	[Fact]
	public void NullableDecimalTest()
	{
		NullableDecimalManager arguments = GetArgumentsInstance<NullableDecimalManager>("-value 77.5");
		Assert.Equal(77.5m, arguments.Value);

		arguments = GetArgumentsInstance<NullableDecimalManager>("");
		Assert.Null(arguments.Value);
	}
}