namespace DigitalProduction.CommandLine;

/// <summary>
/// The type of a record on the lexer stack, combining a lexer with its lookahead token and file name.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LexerStackRecord"/> class.
/// </remarks>
/// <param name="lexer">The lexer.</param>
/// <param name="fileName">Name of the file or null if no file name is available.</param>
internal class LexerStackRecord(Lexer lexer, string? fileName)
{

	/// <summary>
	/// Gets the lexer.
	/// </summary>
	/// <value>The lexer.</value>
	public Lexer Lexer
	{
		get { return mLexer; }
	}

	/// <summary>
	/// Gets or sets the lookahead token.
	/// </summary>
	/// <value>The lookahead token.</value>
	public Token? LA1Token
	{
		get { return mLA1Token; }
		set { mLA1Token = value; }
	}

	/// <summary>
	/// Gets the name of the file.
	/// </summary>
	/// <value>The name of the file.</value>
	public string? FileName
	{
		get { return mFileName; }
	}

	private readonly string? mFileName = fileName;
	private readonly Lexer mLexer = lexer;
	private Token? mLA1Token;
}