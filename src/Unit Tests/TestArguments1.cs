using DigitalProduction.CommandLine;

namespace UnitTests;

public class TestArguments1
{
	readonly QuotationInfo _quotationInfo = new('\"');
			
	public TestArguments1()
	{
		_quotationInfo.AddEscapeCode('\"', '\"');
	}

	[Fact]
	public void TestRawArguments()
	{
		Arguments1 arguments = new();
		try
		{
			CommandLineParser parser = new(arguments);

			string[] input = ["-filename", "\"Test File.txt\"", "-run"];
			parser.Parse(input, false);
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine(exception.ToString());
		}

		Assert.Equal("Test File.txt", arguments.FileName);
		Assert.True(arguments.Run);
	}

	[Fact]
	public void TestRawWithExecutable()
	{
		Arguments1 arguments = new();
		CommandLineParser? parser = null;
		try
		{
			parser = new(arguments);

			string[] input = ["\"C:\\Temp\\Example.exe\"", "-filename", "\"Test File.txt\"", "-run"];
			//parser.AddQuotation(_quotationInfo);
			parser.Parse(input, true);
			
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine(exception.ToString());
		}

		Assert.NotNull(parser);
		Assert.Equal(@"C:\Temp\Example.exe", parser.ExecutablePath);
		Assert.Equal("Test File.txt", arguments.FileName);
		Assert.True(arguments.Run);
	}

	[Fact]
	public void TestStringExecutable()
	{
		Arguments1 arguments = new();
		CommandLineParser? parser = null;
		try
		{
			parser = new(arguments);

			string input = "\"C:\\Temp\\Example.exe\" -filename \"Test File.txt\" -run";
			parser.Parse(input, true);
			
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine(exception.ToString());
		}

		Assert.NotNull(parser);
		Assert.Equal(@"C:\Temp\Example.exe", parser.ExecutablePath);
		Assert.Equal("Test File.txt", arguments.FileName);
		Assert.True(arguments.Run);
	}
}