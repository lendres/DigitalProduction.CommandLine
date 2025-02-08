using C5;
using System;
using System.Globalization;
using System.Text;
using SCG = System.Collections.Generic;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Represents the descriptive properties of an option group.
/// </summary>
public sealed class OptionGroupInfo
{
	#region Fields

	private readonly TreeDictionary<string, OptionInfo>		mOptions = [];
	private readonly OptionGroup							mOptionGroup;
	private readonly UsageInfo								mUsageInfo;

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="OptionGroupInfo"/> class.
	/// </summary>
	/// <param name="usageInfo">The <see cref="UsageInfo"/> containing this <see cref="OptionGroupInfo"/></param>
	/// <param name="optionGroup">The option group.</param>
	/// <param name="optionStyles">The option styles.</param>
	internal OptionGroupInfo(UsageInfo usageInfo, OptionGroup optionGroup, OptionStyles optionStyles)
	{
		mOptionGroup	= optionGroup;
		mUsageInfo		= usageInfo;

		foreach (SCG.KeyValuePair<string, Option> entry in optionGroup.Options)
		{
			mOptions.Add(entry.Key, new OptionInfo(mUsageInfo, entry.Value, optionStyles));
		}
	}

	#endregion

	#region Properties

	/// <summary>
	/// Gets an enumeration of the options included in this group.
	/// </summary>
	/// <value>an enumeration of the options included in this group.</value>
	public SCG.IEnumerable<OptionInfo> Options { get => mOptions.Values; }

	/// <summary>
	/// Gets or sets the description.
	/// </summary>
	/// <value>The description.</value>
	public string Description { get => mOptionGroup.Description; set => mOptionGroup.Description = value; }

	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	/// <value>The name.</value>
	public string Name { get => mOptionGroup.Name ?? mOptionGroup.Id; set => mOptionGroup.Name = value; }

	/// <summary>
	/// Gets the id.
	/// </summary>
	/// <value>The id.</value>
	public string Id { get => mOptionGroup.Id; }

	#endregion

	#region Methods

	/// <summary>
	/// Retrieves a formatted string describing this option group and its options, suitable for displaying
	/// to the user as a help message.
	/// </summary>
	/// <param name="indent">The indentation to use.</param>
	/// <param name="nameColumnWidth">Width of the name column.</param>
	/// <param name="descriptionColumnWidth">Width of the description column.</param>
	/// <returns>a formatted string describing this option group and its options, suitable for displaying
	/// to the user as a help message.</returns>
	public string ToString(int indent, int nameColumnWidth, int descriptionColumnWidth)
	{
		if (nameColumnWidth < 1)
		{
			throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.ArgMustBeGreaterThanZero, "nameColumnWidth"), nameof(nameColumnWidth));
		}

		if (nameColumnWidth < 1)
		{
			throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.ArgMustBeGreaterThanZero, "descriptionColumnWidth"), nameof(descriptionColumnWidth));
		}

		if (indent < 0)
		{
			throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.ArgMustBeNonNegative, "indent"), nameof(indent));
		}

		StringBuilder result = new();
		result.Append(StringFormatter.FormatInColumns(indent, 0, new ColumnInfo(nameColumnWidth + descriptionColumnWidth + mUsageInfo.ColumnSpacing, Name + ":")));
		//result.Append(Environment.NewLine);
		int newIndent = mUsageInfo.IndentWidth;

		if (Description != null)
		{
			result.Append(StringFormatter.FormatInColumns(newIndent + indent, 0, new ColumnInfo(nameColumnWidth + descriptionColumnWidth, Description, Alignment.Left, VerticalAlignment.Top, WordWrappingMethod.Optimal)));
			result.Append(Environment.NewLine);
			newIndent += mUsageInfo.IndentWidth;
		}

		foreach (SCG.KeyValuePair<string, OptionInfo> entry in mOptions)
		{
			result.Append(entry.Value.ToString(indent + newIndent, nameColumnWidth, descriptionColumnWidth - newIndent));
			result.Append(Environment.NewLine);
		}
		return result.ToString();
	}

	/// <summary>
	/// Gets the option with the specified name from this group.
	/// </summary>
	/// <param name="optionName">Name of the option.</param>
	/// <returns>the option with the specified name from this group if one exists; otherwise a null reference is returned.</returns>
	public OptionInfo? GetOption(string optionName)
	{
		if (!mOptions.Find(ref optionName, out OptionInfo description))
		{
			return null;
		}
		return description;
	}

	#endregion
}