using System.Diagnostics;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Represents a command line token.
/// </summary>
internal class Token
{
	#region Public Types

	/// <summary>
	/// Represents the various token types
	/// </summary>
	public enum TokenTypes
	{
		/// <summary>
		/// A token of type <see cref="ValueToken"/>
		/// </summary>
		ValueToken,

		/// <summary>
		/// A token of type <see cref="AssignmentToken"/>
		/// </summary>
		AssignmentToken,

		/// <summary>
		/// A token of type <see cref="OptionNameToken"/>
		/// </summary>
		OptionNameToken,

		/// <summary>
		/// A token of type <see cref="EndToken"/>
		/// </summary>
		EndToken,

		/// <summary>
		/// A token of type <see cref="OptionFileToken"/>
		/// </summary>
		OptionFileToken
	}

	#endregion

	#region Constructor

	/// <summary>
	/// Initializes a new instance of the <see cref="Token"/> class.
	/// </summary>
	/// <param name="tokenType">Type of the token.</param>
	/// <param name="text">The text representing this token. (Mainly for use in error messages etc)</param>
	public Token(TokenTypes tokenType, string text)
	{
		Debug.Assert(text != null);
		TokenType	= tokenType;
		Text		= text;
	}

	#endregion

	#region Properties

	/// <summary>
	/// Gets the type of the token.
	/// </summary>
	/// <value>The type of the token.</value>
	public TokenTypes TokenType { get; private set; }

	/// <summary>
	/// Gets the text.
	/// </summary>
	/// <value>The text.</value>
	public string Text { get; private set; }

	#endregion
}