using System;

namespace DigitalProduction.CommandLine;

// Indicates programming error
/// <summary>
/// Base class for exceptions epresenting a programming error (as opposed to a runtime usage error).
/// </summary>
[Serializable]
public class LogicException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LogicException"/> class.
	/// </summary>
	public LogicException() :
		base()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LogicException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	public LogicException(string message) :
		base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LogicException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerException">The inner exception.</param>
	public LogicException(string message, Exception innerException) :
		base(message, innerException)
	{
	}
}