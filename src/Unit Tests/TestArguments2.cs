using DigitalProduction.CommandLine;

namespace UnitTests;

public class TestArguments2 : TestingBase
{
	public TestArguments2()
	{
	}

	[Fact]
	public void TestShortArguments()
	{
		Arguments2 arguments =  GetArgumentsInstance<Arguments2>("-f \"C:\\Temp\\Test File.txt\" -v true -z true -c true");

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
		Arguments2 arguments =  GetArgumentsInstance<Arguments2>("-f \"C:\\Temp\\Test File.txt\" -vzc");
		
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
		CommandLineParser parser =  GetParser<Arguments2>("-cx");

		foreach (var error in parser.Errors)
		{
			switch (error.ErrorCode)
			{
				case ParseErrorCodes.MissingRequiredOption:
					Assert.Equal("Missing required option \"f\".", error.Message);
					break;
				case ParseErrorCodes.IllegalCardinality:
					Assert.Equal("Only one of the options \"c\", \"h\", \"x\" may be specified", error.Message);
					break;
				default:
					Assert.Fail("Invalid error code found.");
					break;
			}
		}	
	}
}