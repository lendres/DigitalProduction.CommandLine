namespace DigitalProduction.CommandLine;

/// <summary>
/// <see cref="Token"/> representing the special symbol used to stop the parsing of options, normally "--".
/// </summary>
internal class EndToken : Token
{
	/// <summary>
	/// Initializes a new instance of the <see cref="EndToken"/> class.
	/// </summary>
	public EndToken() :
		base(TokenTypes.EndToken, CommandLineStrings.EndOfInput)
	{
	}
}