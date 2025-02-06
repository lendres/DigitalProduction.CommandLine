using DigitalProduction.CommandLine;

namespace UnitTests;

/// <summary>
/// String.
/// </summary>
[CommandLineManager(ApplicationName = "Test", Copyright = "Copyright (c) Lance A. Endres")]
class BoolPrefixManager
{
	[CommandLineOption(Name = "v", Aliases = "value", BoolFunction = BoolFunction.UsePrefix, Description = "Specifies the test value.")]
	public bool Value { get; set; } = false;
}