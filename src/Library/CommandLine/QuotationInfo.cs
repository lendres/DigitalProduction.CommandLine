using C5;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Represents information about the escapable characters within a quoted value.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="QuotationInfo"/> class.
/// </remarks>
/// <param name="quotationMark">The quotation mark used for this quotation.</param>
public class QuotationInfo(char quotationMark)
{
	#region Fields

	private readonly HashDictionary<char, char>		mEscapeCodes		= [];
	private readonly char							mQuotationMark		= quotationMark;

	#endregion

	#region Constructors

	#endregion

	#region Properties

	/// <summary>
	/// Gets the quotation mark.
	/// </summary>
	/// <value>The quotation mark.</value>
	public char QuotationMark { get => mQuotationMark; }

	#endregion

	#region Methods

	/// <summary>
	/// Adds the escape code to this quotation.
	/// </summary>
	/// <param name="code">The code, i.e. the character that will be used after the escape 
	/// character to denote this escape sequence.</param>
	/// <param name="replacement">The character with which the escape sequence will be replaced.</param>
	public void AddEscapeCode(char code, char replacement)
	{
		mEscapeCodes.Add(code, replacement);
	}

	/// <summary>
	/// Removes an escape code from this quotation.
	/// </summary>
	/// <param name="code">The code to remove.</param>
	public void RemoveEscapeCode(char code)
	{
		mEscapeCodes.Remove(code);
	}

	/// <summary>
	/// Determines whether the specified character is an escape code within this quotation.
	/// </summary>
	/// <param name="code">The code.</param>
	/// <returns>
	/// 	<c>true</c> if the specified character is an escape code within this quotation; otherwise, <c>false</c>.
	/// </returns>
	public bool IsEscapeCode(char code)
	{
		return mEscapeCodes.Contains(code);
	}

	/// <summary>
	/// Escapes the escape code character specified.
	/// </summary>
	/// <param name="code">The code.</param>
	/// <returns>The replacement character for the specified escape code if one is available in this 
	/// quotation, otherwise returns the character unchanged.</returns>
	public char EscapeCharacter(char code)
	{
		if (!mEscapeCodes.Find(ref code, out char replacement))
		{
			return code;
		}
		else
		{
			return replacement;
		}
	}

	/// <summary>
	/// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
	/// </summary>
	/// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
	/// <returns>
	/// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
	/// </returns>
	public override bool Equals(object? obj)
	{
		if (obj is not QuotationInfo qi)
		{
			return false;
		}

		return mQuotationMark.Equals(qi.mQuotationMark) && mEscapeCodes.Equals(qi.mEscapeCodes);
	}

	/// <summary>
	/// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
	/// </summary>
	/// <returns>
	/// A hash code for the current <see cref="T:System.Object"></see>.
	/// </returns>
	public override int GetHashCode()
	{
		return mQuotationMark.GetHashCode() ^ mEscapeCodes.GetHashCode();
	}

	#endregion
}