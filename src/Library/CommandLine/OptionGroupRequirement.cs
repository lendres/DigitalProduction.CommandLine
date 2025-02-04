namespace DigitalProduction.CommandLine;

/// <summary>
/// Specifies the requirements on an option group.
/// </summary>
public enum OptionGroupRequirement
{
	/// <summary>
	/// Indicates that no requirement is placed on the options of this group.
	/// </summary>
	None,

	/// <summary>
	/// Indicates that at most one of the options in this group may be specified on the command line.
	/// </summary>
	AtMostOne,

	/// <summary>
	/// Indicates that at least one of the options in this group may be specified on the command line.
	/// </summary>
	AtLeastOne,

	/// <summary>
	/// Indicates that exactly one of the options in this group must be specified on the command line.
	/// </summary>
	ExactlyOne,

	/// <summary>
	/// Indicates that all options of this group must be specified.
	/// </summary>
	All
}