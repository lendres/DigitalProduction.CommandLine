using DigitalProduction.CommandLine;

namespace UnitTests;

public abstract class TestingBase
{
	private struct ParserArgumentPair<T>
	{
		public T Arguments;
		public CommandLineParser Parser;
	}

	public TestingBase()
	{
		QuotationInfo = new('\"');
		QuotationInfo.AddEscapeCode('\"', '\"');
	}

	public QuotationInfo QuotationInfo { get; private set; }


	protected static T GetArgumentsInstance<T>(string commandLineString, bool containsExecutable = false) where T : class, new()
	{
		ParserArgumentPair<T> parsingPair = TryParsing<T>(commandLineString, containsExecutable);
		return parsingPair.Arguments;
	}

	protected static CommandLineParser GetParser<T>(string commandLineString, bool containsExecutable = false) where T : class, new()
	{
		ParserArgumentPair<T> parsingPair = TryParsing<T>(commandLineString, containsExecutable);
		return parsingPair.Parser;
	}

	private static ParserArgumentPair<T> TryParsing<T>(string commandLineString, bool containsExecutable) where T : class, new ()
	{
		try
		{
			T arguments = new();
			ParserArgumentPair<T> parsingPair = new()
			{
				Arguments = arguments,
				Parser = new(arguments)
			};

			Assert.NotNull(parsingPair.Parser);
			parsingPair.Parser.Parse(commandLineString, containsExecutable);

			// If parsing errors occured, write them to the output and continue.
			if (parsingPair.Parser.HasErrors)
			{
				System.Diagnostics.Trace.WriteLine("");
				System.Diagnostics.Trace.Write(parsingPair.Parser.UsageInfo.GetErrorsAsString());
			}

			return parsingPair;
		}
		catch (Exception exception)
		{
			System.Diagnostics.Trace.WriteLine("");
			System.Diagnostics.Trace.WriteLine(exception.ToString());
			throw;
		}
	}
}
