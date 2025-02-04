using System;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Exception indicating that an option switch character was specified on the command line, but the name
/// for the option was missing.
/// </summary>
[Serializable]
public class MissingOptionNameException : ParseException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MissingOptionNameException"/> class.
	/// </summary>
	public MissingOptionNameException() :
		base(CommandLineStrings.DefaultMissingOptionNameExceptionMessage)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MissingOptionNameException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	public MissingOptionNameException(string message) :
		base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MissingOptionNameException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerException">The inner exception.</param>
	public MissingOptionNameException(string message, Exception innerException) :
		base(message, innerException)
	{
	}
}