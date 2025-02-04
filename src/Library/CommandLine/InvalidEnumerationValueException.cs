using System;

namespace DigitalProduction.CommandLine;

[Serializable]
internal class InvalidEnumerationValueException : ParseException
{
	public InvalidEnumerationValueException() :
		base()
	{
	}

	public InvalidEnumerationValueException(string message) :
		base(message)
	{
	}

	public InvalidEnumerationValueException(string message, Exception innerException) :
		base(message, innerException)
	{
	}
}