using DigitalProduction.CommandLine;

namespace UnitTests;

public abstract class TestingBase
{
	public TestingBase()
	{
		QuotationInfo = new('\"');
		QuotationInfo.AddEscapeCode('\"', '\"');
	}

	public QuotationInfo QuotationInfo { get; private set; }


	protected static T GetArgumentsInstance<T>(string commandLineString, bool containsExecutable = false) where T : class, new()
	{
		T nullableArguments	= new();
		try
		{
			CommandLineParser parser = new(nullableArguments);
			Assert.NotNull(parser);
			parser.Parse(commandLineString, containsExecutable);
			if (parser.HasErrors)
			{
				
			}
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine(exception.ToString());
		}
		return nullableArguments;
	}

	protected static CommandLineParser GetParser<T>(string commandLineString, bool containsExecutable = false) where T : class, new()
	{
		T nullableArguments	= new();
		try
		{
			CommandLineParser parser = new(nullableArguments);
			Assert.NotNull(parser);
			parser.Parse(commandLineString, containsExecutable);
			return parser;
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine(exception.ToString());
			throw;
		}
	}
}
