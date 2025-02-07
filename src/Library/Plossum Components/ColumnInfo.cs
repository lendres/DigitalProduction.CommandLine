namespace DigitalProduction.CommandLine;

/// <summary>
/// Represents a column to be used with <see cref="StringFormatter.FormatInColumns"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ColumnInfo"/> struct.
/// </remarks>
/// <param name="width">The width of the column in (fixed-width) characters.</param>
/// <param name="content">The content of this column.</param>
/// <param name="alignment">The alignment to use for this column.</param>
/// <param name="verticalAlignment">The vertical alignment to use for this column</param>        
/// <param name="method">The word wrapping method to use for this column</param>
public struct ColumnInfo(int width, string content, Alignment alignment, VerticalAlignment verticalAlignment, WordWrappingMethod method)
{
	#region Private Fields

	private readonly WordWrappingMethod mWordWrappingMethod     = method;
	private readonly Alignment          mAlignment              = alignment;
	private readonly int                mWidth                  = width;
	private readonly string             mContent                = content;
	private VerticalAlignment           mVerticalAlignment      = verticalAlignment;

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="ColumnInfo"/> struct.
	/// </summary>
	/// <param name="width">The width of the column in (fixed-width) characters.</param>
	/// <param name="content">The content of this column.</param>
	/// <param name="alignment">The alignment to use for this column.</param>
	/// <param name="verticalAlignment">The vertical alignment to use for this column</param>
	public ColumnInfo(int width, string content, Alignment alignment, VerticalAlignment verticalAlignment) :
		this(width, content, alignment, verticalAlignment, WordWrappingMethod.Optimal)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ColumnInfo"/> struct.
	/// </summary>
	/// <param name="width">The width of the column in (fixed-width) characters.</param>
	/// <param name="content">The content of this column.</param>
	/// <param name="alignment">The alignment to use for this column.</param>
	/// <remarks>The word wrapping method used will be the one described by <see cref="Plossum.WordWrappingMethod.Optimal"/>.</remarks>
	public ColumnInfo(int width, string content, Alignment alignment) :
		this(width, content, alignment, VerticalAlignment.Top)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ColumnInfo"/> struct.
	/// </summary>
	/// <param name="width">The width of the column in (fixed-width) characters.</param>
	/// <param name="content">The content of this column.</param>
	/// <remarks>The word wrapping method used will be the one described by <see cref="Plossum.WordWrappingMethod.Optimal"/>, and 
	/// each line in this column will be left aligned.</remarks>
	public ColumnInfo(int width, string content) :
		this(width, content, Alignment.Left)
	{
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Implements the operator ==.
	/// </summary>
	/// <param name="lhs">The LHS.</param>
	/// <param name="rhs">The RHS.</param>
	/// <returns>The result of the operator.</returns>
	public static bool operator ==(ColumnInfo lhs, ColumnInfo rhs)
	{
		return lhs.Equals(rhs);
	}

	/// <summary>
	/// Implements the operator !=.
	/// </summary>
	/// <param name="lhs">The LHS.</param>
	/// <param name="rhs">The RHS.</param>
	/// <returns>The result of the operator.</returns>
	public static bool operator !=(ColumnInfo lhs, ColumnInfo rhs)
	{
		return !lhs.Equals(rhs);
	}

	/// <summary>
	/// Indicates whether this instance and a specified object are equal.
	/// </summary>
	/// <param name="obj">Another object to compare to.</param>
	/// <returns>
	/// true if obj and this instance are the same type and represent the same value; otherwise, false.
	/// </returns>
	public override readonly bool Equals(object? obj)
	{
		if (obj is not ColumnInfo)
		{
			return false;
		}

		ColumnInfo ci = (ColumnInfo)obj;

		return Width.Equals(ci.Width) && Content.Equals(ci.Content) && Alignment.Equals(ci.Alignment) && WordWrappingMethod.Equals(ci.WordWrappingMethod);
	}

	/// <summary>
	/// Returns the hash code for this instance.
	/// </summary>
	/// <returns>
	/// A 32-bit signed integer that is the hash code for this instance.
	/// </returns>
	public override readonly int GetHashCode()
	{
		return Width.GetHashCode() ^ Content.GetHashCode() ^ Alignment.GetHashCode() ^ WordWrappingMethod.GetHashCode();
	}

	#endregion

	#region Public Properties 

	/// <summary>
	/// Gets the width of this column in fixed width characters.
	/// </summary>
	/// <value>the width of this column in fixed width characters.</value>
	public readonly int Width { get => mWidth; }

	/// <summary>
	/// Gets the content of this column.
	/// </summary>
	/// <value>The content of this column.</value>
	public readonly string Content { get => mContent; }

	/// <summary>
	/// Gets the alignment of this column.
	/// </summary>
	/// <value>The alignment of this column.</value>
	public readonly Alignment Alignment { get => mAlignment; }

	/// <summary>
	/// Gets the word wrapping method to use for this column.
	/// </summary>
	/// <value>The the word wrapping method to use for this column.</value>
	public readonly WordWrappingMethod WordWrappingMethod { get => mWordWrappingMethod; }

	/// <summary>
	/// Gets or sets the vertical alignment of the contents of this column.
	/// </summary>
	/// <value>The vertical alignment of this column.</value>
	public VerticalAlignment VerticalAlignment { readonly get => mVerticalAlignment; set => mVerticalAlignment = value; }

	#endregion
}