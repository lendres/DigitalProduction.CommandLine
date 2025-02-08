using DigitalProduction.CommandLine;

namespace UnitTests;

/// <summary>
/// An example using groups and argument constraints.
/// 
/// Using "OptionStyles.Group" allows grouping arguments as "-vzc".
/// Because "OptionStyles.Group" is specified, you cannot access the aliases via "-file", you must use the double dash as "--file".
/// 
/// Using "OptionGroupRequirement.ExactlyOne" means only one of c, x, or h can be specified.
/// </summary>
[CommandLineManager(ApplicationName = "Test 2", Copyright = "Copyright (C) Peter Palotas 2007", EnabledOptionStyles = OptionStyles.Group | OptionStyles.LongUnix)]
[CommandLineOptionGroup("commands", Name = "Commands", Require = OptionGroupRequirement.ExactlyOne)]
[CommandLineOptionGroup("options", Name = "Run Options")]
class Arguments2
{
	/// <summary>
	/// The "MinOccurs = 1" means the argument is required (one and exactly one).
	/// </summary>
	[CommandLineOption(Name = "f", Aliases = "file", Description = "Specify the file name of the archive.", MinOccurs = 1)]
	public string FileName { get; set; } = "";

	/// <summary>
	/// Options.
	/// </summary>
	[CommandLineOption(Name = "filter", RequireExplicitAssignment = true, Description = "Specifies a filter on which files to include or exclude.", GroupId = "options")]
	public List<string> Filters { get; set; } = [];

	[CommandLineOption(Name = "v", Aliases = "verbose", Description = "Produce verbose output.", GroupId = "options")]
	public bool Verbose { get; set; }

	[CommandLineOption(Name = "z", Aliases = "usecompression", Description = "Compress or decompress the archive.", GroupId = "options")]
	public bool UseCompression { get; set; }

	/// <summary>
	/// Commands.  Only one is allowed.
	/// </summary>
	[CommandLineOption(Name = "c", Aliases = "create", Description = "Create a new archive.", GroupId = "commands")]
	public bool Create { get; set; }

	[CommandLineOption(Name = "x", Aliases = "extract", Description = "Extract files from archive.", GroupId = "commands")]
	public bool Extract { get; set; }

	[CommandLineOption(Name = "h", Aliases = "help", Description = "Shows this help text.", GroupId = "commands")]
	public bool Help { get; set; }
}