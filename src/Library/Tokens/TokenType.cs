using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Represents the various token types
/// </summary>
internal enum TokenTypes
{
	/// <summary>
	/// A token of type <see cref="ValueToken"/>
	/// </summary>
	ValueToken,

	/// <summary>
	/// A token of type <see cref="AssignmentToken"/>
	/// </summary>
	AssignmentToken,

	/// <summary>
	/// A token of type <see cref="OptionNameToken"/>
	/// </summary>
	OptionNameToken,

	/// <summary>
	/// A token of type <see cref="EndToken"/>
	/// </summary>
	EndToken,

	/// <summary>
	/// A token of type <see cref="OptionFileToken"/>
	/// </summary>
	OptionFileToken
}
