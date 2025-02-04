using System;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Specifies how a boolean command line option should be evaluated.
/// </summary>
/// <remarks>This enumeration applies to the <see cref="CommandLineOptionAttribute.BoolFunction"/> property, and 
/// is ignored unless the attribute is applied to an attribute with the base type of <b>bool</b>. 
/// Options that are boolean values can be treated in different ways according to the values of this enumeration.</remarks>
public enum BoolFunction
{
	/// <summary>
	/// The option is set to the value specified, which means that the option requires a value (unless a default value 
	/// is assigned, see <see cref="CommandLineOptionAttribute.DefaultAssignmentValue"/>). The value must be specified as 
	/// either <see cref="Boolean.TrueString"/> or <see cref="Boolean.FalseString"/>.
	/// </summary>
	Value,

	/// <summary>
	/// The option does not accept a value, and if present on the command line the member to which the attribute is
	/// applied will be set to <b>true</b>.
	/// </summary>
	TrueIfPresent,

	/// <summary>
	/// The option does not accept a value, and if present on the command line the member to which the attribute is
	/// applied will be set to <b>false</b>.
	/// </summary>
	FalseIfPresent,

	/// <summary>
	/// The option does not accept a value. Instead if present on the command line the member to which the attribute
	/// is applied will be set to <b>true</b> if the prefix of the option was '+', otherwise; <b>false</b>. It is an
	/// error to use this enumeration if <see cref="OptionStyles.Plus"/> was not specified.
	/// </summary>
	UsePrefix
}