using DigitalProduction.CommandLine;

namespace UnitTests;

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableStringManager
{
	[CommandLineOption(Name = "filename", Description = "Specifies the input file.")]
	public string? FileName { get; set; } = null;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableBoolManager
{
	[CommandLineOption(Name = "run", BoolFunction = BoolFunction.TrueIfPresent, Description = "If true, the application will automatically start running.")]
	public bool? Run { get; set; } = null;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class BoolManager
{
	[CommandLineOption(Name = "run", BoolFunction = BoolFunction.TrueIfPresent, Description = "If true, the application will automatically start running.")]
	public bool Run
	{ 
		get;
		set;
	}
}