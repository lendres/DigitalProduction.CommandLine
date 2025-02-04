namespace DigitalProduction.CommandLine;

/// <summary>
/// A <see cref="Token"/> representing a value on the command line. A value is basically anything that 
/// is not the name of an option or another character sequence with a special meaning.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ValueToken"/> class.
/// </remarks>
/// <param name="value">The value.</param>
internal class ValueToken(string value) : Token(TokenTypes.ValueToken, value)
{
	/// <summary>
	/// Gets the value.
	/// </summary>
	/// <value>The value.</value>
	public string Value { get; } = value;
}