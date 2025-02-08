using System.Text;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Represents an error generated by the <see cref="CommandLineParser"/>
/// </summary>
public class ErrorInfo
{
	#region Private Fields

	private string?				_fileName;
	private ParseErrorCodes		_errorCode;
	private int?				_line;
	private string				_message;
	private string?				_optionName;

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="ErrorInfo"/> class.
	/// </summary>
	/// <param name="errorCode">The error code.</param>
	/// <param name="errorMessage">The error message.</param>
	/// <param name="optionName">Name of the option.</param>
	/// <param name="fileName">Name of the file, or null if no file name was available.</param>
	/// <param name="line">The line number on which the error occured, or null if no line information was available.</param>
	internal ErrorInfo(ParseErrorCodes errorCode, string errorMessage, string? optionName, string? fileName, int? line)
	{
		_errorCode = errorCode;
		_message = errorMessage;
		_optionName = optionName;
		_line = line;
		_fileName = fileName;
	}

	#endregion

	#region Public Properties

	/// <summary>
	/// Gets or sets the message.
	/// </summary>
	/// <value>The message.</value>
	public string Message { get => _message; set => _message = value; }

	/// <summary>
	/// Gets or sets the error code.
	/// </summary>
	/// <value>The error code.</value>
	public ParseErrorCodes ErrorCode { get => _errorCode; set => _errorCode = value; }

	/// <summary>
	/// Gets a value indicating whether this error originates from a file, meaning that a file name will be available.
	/// </summary>
	/// <value><c>true</c> if this error originates from a file; otherwise, <c>false</c>.</value>
	public bool OriginatesFromFile {get => _fileName != null; }

	/// <summary>
	/// Gets or sets the name of the option causing this error.
	/// </summary>
	/// <value>The name of the option causing this error, or null if no option name is available.</value>
	public string? OptionName { get => _optionName; set => _optionName = value; }

	/// <summary>
	/// Gets a value indicating whether this instance has option name set.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if this instance has option name set; otherwise, <c>false</c>.
	/// </value>
	public bool HasOptionName { get => _optionName != null; }

	/// <summary>
	/// Gets or sets the line on which the error occured.
	/// </summary>
	/// <value>The line on which the error occured, or null if no such information is available.</value>
	public int? Line { get => _line; set => _line = value; }

	/// <summary>
	/// Gets or sets the name of the file.
	/// </summary>
	/// <value>The name of the file in which the error occured, or null if no file name is available.</value>
	public string? FileName { get => _fileName; set => _fileName = value; }

	#endregion

	#region Public Methods

	/// <summary>
	/// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
	/// </summary>
	/// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
	/// <returns>
	/// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
	/// </returns>
	public override bool Equals(object? obj)
	{
		if (obj is not ErrorInfo error)
		{
			return false;
		}

		return _errorCode.Equals(error._errorCode) && _message.Equals(error._message) && (_optionName == null ? error._optionName == null : _optionName.Equals(error._optionName));
	}

	/// <summary>
	/// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
	/// </summary>
	/// <returns>
	/// A hash code for the current <see cref="T:System.Object"></see>.
	/// </returns>
	public override int GetHashCode()
	{
		return _message.GetHashCode() ^ _errorCode.GetHashCode() ^ (_optionName == null ? 0 : _optionName.GetHashCode());
	}

	/// <summary>
	/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
	/// </summary>
	/// <returns>
	/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
	/// </returns>
	public override string ToString()
	{
		StringBuilder str = new();
		str.Append("Error");

		if (_fileName != null)
		{
			str.Append(" in file \"");
			str.Append(_fileName);
			str.Append('"');
		}

		if (_line.HasValue)
		{
			str.Append(" on line ");
			str.Append(_line.Value);
		}
		str.Append(": ");
		str.Append(_message);
		return str.ToString();
	}

	#endregion
}