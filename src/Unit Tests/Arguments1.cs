using DigitalProduction.CommandLine;

namespace UnitTests;

/// <summary>
/// A simple command line manager.
/// </summary>
[CommandLineManager(ApplicationName = "Test 1", Copyright = "Copyright (c) Lance A. Endres")]
class Arguments1
{
	/// <summary>
	/// This creates an option used as
	/// -filename "File Name.txt"
	/// By specifying an minimum occurance of one, it makes this argument mandatory.
	/// </summary>
	[CommandLineOption(Name = "filename", Description = "Specifies the input file.", MinOccurs=1)]
	public string FileName { get; set; } = "";

	/// <summary>
	/// This creates an option used as
	/// -run
	/// If present, the property will be set to true (an argument is not requried).
	/// </summary>
	[CommandLineOption(Name = "run", BoolFunction = BoolFunction.TrueIfPresent, Description = "Automatically start running at star tup.")]
	public bool Run { get; set; } = false;
}