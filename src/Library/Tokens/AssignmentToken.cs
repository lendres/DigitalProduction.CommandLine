namespace DigitalProduction.CommandLine;

/// <summary>
/// <see cref="Token"/> representing an assignment character.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AssignmentToken"/> class.
/// </remarks>
/// <param name="assignmentChar">The assignment char.</param>
internal class AssignmentToken(char assignmentChar) : Token(TokenTypes.AssignmentToken, assignmentChar.ToString())
{
	private readonly char mAssignmentChar = assignmentChar;

	/// <summary>
	/// Gets the assignment character.
	/// </summary>
	/// <value>The assignment character.</value>
	public char AssignmentCharacter { get => mAssignmentChar; }
}