using C5;
using System;
using System.Globalization;
using System.Text;
using SCG = System.Collections.Generic;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Represents the descriptive properties of a command line option.
/// </summary>
public sealed class OptionInfo : IDisposable
{
	#region Fields

	private readonly ArrayList<string>	_aliases = [];
	private readonly OptionStyles		_optionStyles;
	private readonly Option				_option;
	private readonly UsageInfo			_usageInfo;

	#endregion

	#region Constructors and Disposing

	/// <summary>
	/// Initializes a new instance of the <see cref="OptionInfo"/> class.
	/// </summary>
	/// <param name="usageInfo">The <see cref="UsageInfo" /> creating this OptionInfo</param>
	/// <param name="option">The option.</param>
	/// <param name="optionStyle">The option style.</param>
	internal OptionInfo(UsageInfo usageInfo, Option option, OptionStyles optionStyle)
	{
		_option = option;
		_optionStyles = optionStyle;
		_usageInfo = usageInfo;

		foreach (string alias in _option.Aliases)
		{
			_aliases.Add(OptionStyleManager.PrefixOptionForDescription(_optionStyles, alias));
		}
	}

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	/// </summary>
	public void Dispose()
	{
		_aliases.Dispose();
	}

	#endregion

	#region Properties

	/// <summary>
	/// Gets an enumeration containing strings representing the prefixed names of the aliases of this option.
	/// </summary>
	/// <value>an enumeration containing strings representing the prefixed names of the aliases of this option.</value>
	public SCG.IEnumerable<string> Aliases { get => _aliases; }

	/// <summary>
	/// Gets or sets the description.
	/// </summary>
	/// <value>The description.</value>
	public string Description { get => _option.Description; set => _option.Description = value; }

	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>The name.</value>
	public string Name { get => OptionStyleManager.PrefixOptionForDescription(_optionStyles, _option.Name); }

	/// <summary>
	/// Gets the id. 
	/// </summary>
	/// <value>The id.</value>
	/// <remarks>The id of an option is the same as its <see cref="Name"/>.</remarks>
	public string Id { get => _option.Name; }

	#endregion

	#region Public Methods

	/// <summary>
	/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
	/// </summary>
	/// <returns>
	/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
	/// </returns>
	public override string ToString()
	{
		return ToString(0, 25, 50);
	}

	/// <summary>
	/// Returns a formatted string describing this option and its aliases.
	/// </summary>
	/// <param name="indent">The indentation to use.</param>
	/// <param name="nameColumnWidth">Width of the name column.</param>
	/// <param name="descriptionColumnWidth">Width of the description column.</param>
	/// <returns>a formatted string describing this option and its aliases that is suitable for displaying 
	/// as a help message.</returns>
	public string ToString(int indent, int nameColumnWidth, int descriptionColumnWidth)
	{
		if (nameColumnWidth < 1)
		{
			throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.ArgMustBeGreaterThanZero, "nameColumnWidth"), nameof(nameColumnWidth));
		}

		if (descriptionColumnWidth < 1)
		{
			throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.ArgMustBeGreaterThanZero, "descriptionColumnWidth"), nameof(descriptionColumnWidth));
		}

		if (indent < 0)
		{
			throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.ArgMustBeNonNegative, "indent"), nameof(indent));
		}

		StringBuilder names = new();

		names.Append(Name);
		foreach (string alias in _option.Aliases)
		{
			names.Append(", ");
			names.Append(OptionStyleManager.PrefixOptionForDescription(_optionStyles, alias));
		}

		ColumnInfo nameColumn = new(nameColumnWidth, names.ToString(), Alignment.Left);
		ColumnInfo descColumn = new(descriptionColumnWidth, Description ?? "e", Alignment.Left, VerticalAlignment.Bottom);
		return StringFormatter.FormatInColumns(indent, _usageInfo.ColumnSpacing, nameColumn, descColumn);
	}

	#endregion
}