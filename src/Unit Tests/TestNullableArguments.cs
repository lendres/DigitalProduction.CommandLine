using DigitalProduction.CommandLine;

namespace UnitTests;

public class TestNullableArguments
{
	readonly QuotationInfo _quotationInfo = new('\"');

	private NullableBoolManager _emptyArguments;
			
	public TestNullableArguments()
	{
		_quotationInfo.AddEscapeCode('\"', '\"');
		//_emptyArguments = GetArgumentsInstance("");
	}

	private T GetArgumentsInstance<T>(string commandLineString) where T : class, new()
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

	[Fact]
	public void TestString()
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

		//arguments = GetArgumentsInstance<NullableBoolManager>("");
		//Assert.Null(_emptyArguments.Run);
	}
}