using System;
using System.Diagnostics;

namespace DigitalProduction.CommandLine;

/// <summary>
/// <see cref="Token"/> representing an option name as found on the command line.
/// </summary>
internal class OptionNameToken : Token
{
	/// <summary>
	/// Initializes a new instance of the <see cref="OptionNameToken"/> class.
	/// </summary>
	/// <param name="name">The name of this option.</param>
	/// <param name="optionStyle">The option style.</param>
	public OptionNameToken(string name, OptionStyles optionStyle) :
		base(TokenTypes.OptionNameToken, OptionStyleManager.GetPrefix(optionStyle, name) + name)
	{
		Debug.Assert(!String.IsNullOrEmpty(name));
		OptionStyle	= optionStyle;
		Name		= name;
	}

	/// <summary>
	/// Gets the option style.
	/// </summary>
	/// <value>The option style.</value>
	public OptionStyles OptionStyle { get; private set; }

	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>The name.</value>
	public string Name { get; private set; }
}