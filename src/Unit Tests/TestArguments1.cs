using DigitalProduction.CommandLine;

namespace UnitTests;

public class TestArguments1
{
	[Fact]
	public void Test1()
	{
		Arguments1 arguments = new();
		try
		{
			CommandLineParser parser = new(arguments);

			string[] rawArguments = ["-filename", "\"Test File.txt\"", "-run"];
			QuotationInfo quotationInfo = new('\"');
			quotationInfo.AddEscapeCode('\"', '\"');
			parser.AddQuotation(quotationInfo);
			parser.Parse(rawArguments, false);
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine(exception.ToString());
		}

		Assert.Equal("Test File.txt", arguments.FileName);
		Assert.True(arguments.Run);
	}
}