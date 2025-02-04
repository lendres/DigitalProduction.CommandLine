using System;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Exception indicating that a closing quote was missing from a value on the command line.
/// </summary>
[Serializable]
public class MissingClosingQuoteException : ParseException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MissingClosingQuoteException"/> class.
	/// </summary>
	public MissingClosingQuoteException() :
		base(CommandLineStrings.MissingClosingQuote)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MissingClosingQuoteException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	public MissingClosingQuoteException(string message) :
		base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MissingClosingQuoteException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerException">The inner exception.</param>
	public MissingClosingQuoteException(string message, Exception innerException) :
		base(message, innerException)
	{
	}
}