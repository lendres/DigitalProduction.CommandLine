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

	[Fact]
	public void TestString()
	{
		StringManager arguments = GetArgumentsInstance<StringManager>("-filename \"Test File.txt\"");
		Assert.Equal("Test File.txt", arguments.FileName);
	}

	[Fact]
	public void TestNullableString()
	{
		NullableStringManager arguments = GetArgumentsInstance<NullableStringManager>("-filename \"Test File.txt\"");
		Assert.Equal("Test File.txt", arguments.FileName);

		arguments = GetArgumentsInstance<NullableStringManager>("");
		Assert.Null(arguments.FileName);
	}

	[Fact]
	public void TestBool()
	{
		BoolManager arguments = GetArgumentsInstance<BoolManager>("-run");
		Assert.True(arguments.Run);
	}

	[Fact]
	public void TestNullableBool()
	{
		NullableBoolManager arguments = GetArgumentsInstance<NullableBoolManager>("-run");
		Assert.True(arguments.Run);

		arguments = GetArgumentsInstance<NullableBoolManager>("");
		Assert.Null(arguments.Run);
	}

	[Fact]
	public void TestByte()
	{
		ByteManager arguments = GetArgumentsInstance<ByteManager>("-value 0x32");
		Assert.Equal(0x32, arguments.Value);
	}

	[Fact]
	public void TestNullableByte()
	{
		NullableByteManager arguments = GetArgumentsInstance<NullableByteManager>("-value 0x32");
		Assert.Equal((byte)0x32, arguments.Value);

		arguments = GetArgumentsInstance<NullableByteManager>("");
		Assert.Null(arguments.Value);
	}

	[Fact]
	public void TestChar()
	{
		CharManager arguments = GetArgumentsInstance<CharManager>("-value x");
		Assert.Equal('x', arguments.Value);

		CommandLineParser parser = GetParser<CharManager>("-value xx");
		Assert.Equal("The value \"xx\" is not valid for option \"value\"; Character options must be a single character.", parser.Errors.First().Message);
	}

	[Fact]
	public void TestNullableChar()
	{
		NullableCharManager arguments = GetArgumentsInstance<NullableCharManager>("-value x");
		Assert.Equal('x', arguments.Value);

		arguments = GetArgumentsInstance<NullableCharManager>("");
		Assert.Null(arguments.Value);
	}

	[Fact]
	public void TestInt()
	{
		IntManager arguments = GetArgumentsInstance<IntManager>("-value 77");
		Assert.Equal(77, arguments.Value);
	}

	[Fact]
	public void TestNullableInt()
	{
		NullableIntManager arguments = GetArgumentsInstance<NullableIntManager>("-value 77");
		Assert.Equal(77, arguments.Value);

		arguments = GetArgumentsInstance<NullableIntManager>("");
		Assert.Null(arguments.Value);
	}
}