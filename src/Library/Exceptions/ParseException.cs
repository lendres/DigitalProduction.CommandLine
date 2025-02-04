using System;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Base class for any exceptions specific to the command line parser. Indicates an error during the
/// parsing of the command line.
/// </summary>
[Serializable]
public abstract class ParseException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ParseException"/> class.
	/// </summary>
	protected ParseException() :
		base(CommandLineStrings.DefaultCommandLineExceptionErrorMessage)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ParseException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	protected ParseException(string message) :
		base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ParseException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerException">The inner exception.</param>
	protected ParseException(string message, Exception innerException) :
		base(message, innerException)
	{
	}
}