using DigitalProduction.CommandLine;

namespace UnitTests;

/// <summary>
/// Test using +/- for boolean prefixes.
/// </summary>
[CommandLineManager(EnabledOptionStyles = OptionStyles.Unix | OptionStyles.Plus)]
class BoolPrefixManager
{
	[CommandLineOption(Name = "v", Aliases = "value", BoolFunction = BoolFunction.UsePrefix, Description = "Specifies the test value.")]
	public bool Value { get; set; } = false;

	[CommandLineOption(Name = "lvalue", Aliases = "longvalue", BoolFunction = BoolFunction.UsePrefix, Description = "Specifies the second test value.")]
	public string LongValue { get; set; } = string.Empty;
}