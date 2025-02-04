using System;

namespace DigitalProduction.CommandLine;

internal class OptionFileToken : Token
{
	/// <summary>
	/// Initializes a new instance of the <see cref="OptionFileToken"/> class.
	/// </summary>
	public OptionFileToken(string fileName) :
		base(TokenTypes.OptionFileToken, "@" + fileName)
	{
		ArgumentNullException.ThrowIfNull(fileName);
		FileName = fileName;
	}

	public string FileName { get; private set; }
}