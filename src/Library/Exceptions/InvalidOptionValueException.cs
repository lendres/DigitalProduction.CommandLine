using System;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Exception that may be thrown by the setter method of a property or a method attributed with the <see cref="CommandLineOptionAttribute"/>.
/// </summary>
/// <remarks>Throwing this exception from a setter method of a property or a method with the attribute <see cref="CommandLineOptionAttribute"/>
/// will cause the command line parser to insert an error message indicating that there was an error on the command line, after which it 
/// will continue to parse the rest of the command line. The error can later be retrieved using the property <see cref="CommandLineParser.Errors"/>.
/// </remarks>
[Serializable]
public class InvalidOptionValueException : ParseException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="InvalidOptionValueException"/> class.
	/// </summary>
	public InvalidOptionValueException() :
		base(String.Empty)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="InvalidOptionValueException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	public InvalidOptionValueException(string message) :
		base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="InvalidOptionValueException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="includeDefaultMessage">if set to <c>true</c> a default message will be prefixed to the error message specified, which 
	/// will be something similar to "Invalid value &lt;value&gt; for option &lt;optionName&gt;: ".</param>
	public InvalidOptionValueException(string message, bool includeDefaultMessage) :
		base(message)
	{
		IncludeDefaultMessage = includeDefaultMessage;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="InvalidOptionValueException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerException">The inner exception.</param>
	public InvalidOptionValueException(string message, Exception innerException) :
		base(message, innerException)
	{
	}

	/// <summary>
	/// Gets a value indicating whether the default message will be included in the error list supplied by <see cref="CommandLineParser.Errors"/>.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if the default message is included in the error list supplied by <see cref="CommandLineParser.Errors"/>; otherwise, <c>false</c>.
	/// </value>
	public bool IncludeDefaultMessage { get; private set; } = true;
}