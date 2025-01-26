using DigitalProduction.CommandLine;

namespace UnitTests;

public class TestArguments2
{
	public TestArguments2()
	{
	}

	[Fact]
	public void TestShortArguments()
	{
		Arguments2 arguments = new();
		CommandLineParser? parser = null;
		try
		{
			parser = new(arguments);

			string input = "-f \"C:\\Temp\\Test File.txt\" -v true -z true -c true";
			parser.Parse(input, false);
			
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine(exception.ToString());
		}

		Assert.NotNull(parser);
		Assert.Equal(@"C:\Temp\Test File.txt", arguments.FileName);
		Assert.True(arguments.Verbose);
		Assert.True(arguments.UseCompression);
		Assert.True(arguments.Create);
		Assert.False(arguments.Extract);
	}

	/// <summary>
	/// Tests:
	/// EnabledOptionStyles = OptionStyles.Group.
	/// </summary>
	[Fact]
	public void TestGroupedArguments()
	{
		Arguments2 arguments = new();
		CommandLineParser? parser = new(arguments);
		string input = "-f \"C:\\Temp\\Test File.txt\" -vzc";
		parser.Parse(input, false);
			
		Assert.NotNull(parser);
		Assert.Equal(@"C:\Temp\Test File.txt", arguments.FileName);
		Assert.True(arguments.Verbose);
		Assert.True(arguments.UseCompression);
		Assert.True(arguments.Create);
		Assert.False(arguments.Extract);
	}

	/// <summary>
	/// Tests:
	/// MinOccurs = 1 for the file name.
	/// Require = OptionGroupRequirement.ExactlyOne.
	/// </summary>
	[Fact]
	public void TestExactlyOne()
	{
		Arguments2 arguments = new();
		CommandLineParser? parser = new(arguments);
		string input = "-cx";
		parser.Parse(input, false);
			
		Assert.NotNull(parser);
		Assert.Equal("Missing required option \"f\".", parser.Errors.ElementAt(0).Message);
		Assert.Equal("Only one of the options \"c\", \"h\", \"x\" may be specified", parser.Errors.ElementAt(1).Message);
	}
}