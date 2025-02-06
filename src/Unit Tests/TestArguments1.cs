using DigitalProduction.CommandLine;

namespace UnitTests;

public class TestArguments1 : TestingBase
{
	public TestArguments1()
	{
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
		string              input       = "\"C:\\Temp\\Example.exe\" -filename \"Test File.txt\" -run";
		Arguments1			arguments	= GetArgumentsInstance<Arguments1>(input, true);
		CommandLineParser	parser		= GetParser<Arguments1>(input, true);

		Assert.NotNull(parser);
		Assert.Equal(@"C:\Temp\Example.exe", parser.ExecutablePath);
		Assert.Equal("Test File.txt", arguments.FileName);
		Assert.True(arguments.Run);
	}
}