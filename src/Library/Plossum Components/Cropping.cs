namespace DigitalProduction.CommandLine;

/// <summary>
/// Specifies how strings will be cropped in case they are too long for the specified field. Used by <see cref="StringFormatter"/>.
/// </summary>
public enum Cropping
{
	/// <summary>
	/// The left hand side of the string will be cropped, and only the rightmost part of the string will remain.
	/// </summary>
	Left,

	/// <summary>
	/// The right hand side of the string will be cropped, and only the leftmost part of the string will remain.
	/// </summary>
	Right,

	/// <summary>
	/// Both ends of the string will be cropped and only the center part will remain.
	/// </summary>
	Both
}