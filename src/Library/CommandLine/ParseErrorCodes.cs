namespace DigitalProduction.CommandLine;

/// <summary>
/// Indicates the type of the error of a <see cref="ErrorInfo"/>.
/// </summary>
/// <seealso cref="CommandLineParser.Errors"/>
public enum ParseErrorCodes
{
	/// <summary>
	/// A value was quoted but no closing quote was found on the command line.
	/// </summary>
	MissingClosingQuote,

	/// <summary>
	/// An option switch character was specified but without an option name following it.
	/// </summary>
	EmptyOptionName,

	/// <summary>
	/// The option specified does not exist.
	/// </summary>
	UnknownOption,

	/// <summary>
	/// A required option was not specified.
	/// </summary>
	MissingRequiredOption,

	/// <summary>
	/// An assignment character was found where it should not have been on the command line.
	/// </summary>
	UnexpectedAssignment,

	/// <summary>
	/// An option is specified either too many times or too few times on the command line.
	/// </summary>
	IllegalCardinality,

	/// <summary>
	/// An option requiring a value was specified without a value.
	/// </summary>
	MissingValue,

	/// <summary>
	/// An option not accepting a value was explicitly assigned a value.
	/// </summary>
	AssignmentToNonValueOption,

	/// <summary>
	/// An option specified was prohibited by another option that was also specified.
	/// </summary>
	OptionProhibited,

	/// <summary>
	/// A value assigned to a numerical option was outside the range specified by 
	/// <see cref="CommandLineOptionAttribute.MinValue"/> and <see cref="CommandLineOptionAttribute.MaxValue"/>.
	/// </summary>
	Overflow,

	/// <summary>
	/// The value specified for an option was in an invalid format, or an illegal value for the specified type.
	/// </summary>
	InvalidFormat,

	/// <summary>
	/// The file specified was not found, or could not be opened for reading.
	/// </summary>
	FileNotFound,

	/// <summary>
	/// The value specified for the option failed user validation.
	/// </summary>
	InvalidValue,

	/// <summary>
	/// An unknown error occured.
	/// </summary>
	UnknownError
}