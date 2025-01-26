using DigitalProduction.CommandLine;

namespace UnitTests;

[CommandLineManager(ApplicationName = "Test 1", Copyright = "Copyright (c) Lance A. Endres")]
class Arguments1
{
	[CommandLineOption(Name = "filename", Description = "Specifies the input file.")]
	public string FileName { get; set; } = "";

	[CommandLineOption(Name = "run", BoolFunction = BoolFunction.TrueIfPresent, Description = "If true, the application will automatically start running.")]
	public bool Run { get; set; } = false;
}