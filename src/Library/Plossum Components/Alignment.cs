namespace DigitalProduction.CommandLine;

/// <summary>
/// Represents the alignment of text for use with <see cref="StringFormatter"/>
/// </summary>
public enum Alignment
{
	/// <summary>
	/// Text will be left aligned
	/// </summary>
	Left,

	/// <summary>
	/// Text will be right aligned
	/// </summary>
	Right,

	/// <summary>
	/// Text will be centered within the specified field
	/// </summary>
	Center,

	/// <summary>
	/// Spaces between words will be expanded so the string takes up the full width of the field.
	/// </summary>
	Justified
}