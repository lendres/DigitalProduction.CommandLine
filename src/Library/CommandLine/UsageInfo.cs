using C5;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using SCG = System.Collections.Generic;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Represents the properties of a <see cref="CommandLineManagerAttribute"/> (or rather the object to which its 
/// applied) that describe the command line syntax.
/// </summary>
/// <remarks>This class is the only way to programatically set usage descriptions, group names and similar, which 
/// is required if globalization of the usage description is desired.  Users can not instantiate objects of this 
/// class, but they are retrieved by the <see cref="CommandLineParser.UsageInfo"/> property.</remarks>
public sealed class UsageInfo
{
	#region Fields

	private readonly CommandLineParser							mParser;
	private readonly TreeDictionary<string, OptionGroupInfo>	mGroups				= [];
	private readonly TreeDictionary<string, OptionInfo>			mOptions			= [];
	private int													mColumnSpacing		= 3;
	private int													mIndentWidth		= 3;

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="UsageInfo"/> class.
	/// </summary>
	/// <param name="options">The options.</param>
	/// <param name="optionStyles">The option styles.</param>
	/// <param name="parser">The parser.</param>
	internal UsageInfo(SCG.IEnumerable<KeyValuePair<string, IOption>> options, OptionStyles optionStyles, CommandLineParser parser)
	{
		mParser = parser;
		foreach (KeyValuePair<string, IOption> entry in options)
		{
			if (entry.Value is Option option)
			{
				if (option.Group != null)
				{
					if (!mGroups.Contains(option.Group.Id))
					{
						mGroups.Add(option.Group.Id, new OptionGroupInfo(this, option.Group, optionStyles));
					}
				}
				else
				{
					Debug.Assert(!mOptions.Contains(option.Name));
					mOptions.Add(option.Name, new OptionInfo(this, option, optionStyles));
				}
			}
		}
	}

	#endregion

	#region Properties

	/// <summary>
	/// Gets or sets the name of the application.
	/// </summary>
	/// <value>The name of the application.</value>
	public string ApplicationName { get => mParser.ApplicationName; set =>  mParser.ApplicationName = value; }

	/// <summary>
	/// Gets or sets the application version.
	/// </summary>
	/// <value>The application version.</value>
	public string ApplicationVersion { get => mParser.ApplicationVersion; set =>  mParser.ApplicationVersion = value; }

	/// <summary>
	/// Gets or sets the application copyright.
	/// </summary>
	/// <value>The application copyright.</value>
	public string ApplicationCopyright { get => mParser.ApplicationCopyright; set =>  mParser.ApplicationCopyright = value; }

	/// <summary>
	/// Gets or sets the application description.
	/// </summary>
	/// <value>The application description.</value>
	public string ApplicationDescription { get => mParser.ApplicationDescription; set => mParser.ApplicationDescription = value; }

	/// <summary>
	/// Gets an enumeration of <see cref="OptionInfo"/> objects describing the options of this 
	/// command line manager that are <i>not</i> part of any option group.
	/// </summary>
	/// <value>an enumeration of <see cref="OptionInfo"/> objects describing the options of this 
	/// command line manager that are <i>not</i> part of any option group.</value>
	public SCG.IEnumerable<OptionInfo> Options { get => mOptions.Values; }

	/// <summary>
	/// Gets an enumeration of the <see cref="OptionGroupInfo"/> objects describin the option groups
	/// of this command line manager.
	/// </summary>
	/// <value>an enumeration of the <see cref="OptionGroupInfo"/> objects describin the option groups
	/// of this command line manager.</value>
	public SCG.IEnumerable<OptionGroupInfo> Groups { get => mGroups.Values; }

	/// <summary>
	/// Gets or sets the column spacing to use for any string formatting involving multiple columns.
	/// </summary>
	/// <value>The column spacing used for any string formatting involving multiple columns.</value>
	public int ColumnSpacing
	{
		get => mColumnSpacing;
		set
		{
			if (value < 0)
			{
				throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.ArgMustBeNonNegative, "value"), nameof(value));
			}
			mColumnSpacing = value;
		}
	}

	/// <summary>
	/// Gets or sets the width of the indent to use for any string formatting by this <see cref="UsageInfo"/>.
	/// </summary>
	/// <value>the width of the indent to use for any string formatting by this <see cref="UsageInfo"/>.</value>
	public int IndentWidth
	{
		get => mIndentWidth;
		set
		{
			if (value < 0)
			{
				throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.ArgMustBeNonNegative, "value"), nameof(value));
			}
			mIndentWidth = value;
		}
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Gets the option with the specified name.
	/// </summary>
	/// <param name="name">The name of the option to retrieve.</param>
	/// <returns>The option in the option manager described by this object with the specified name, or
	/// a null reference if no such option exists.</returns>        
	public OptionInfo? GetOption(string name)
	{
		if (!mOptions.Find(ref name, out OptionInfo? description))
		{
			// Search through all groups for this option
			foreach (OptionGroupInfo gdesc in mGroups.Values)
			{
				if ((description = gdesc.GetOption(name)) != null)
				{
					return description;
				}
			}
			return null;
		}
		return description;
	}

	/// <summary>
	/// Gets the option group with the specified id.
	/// </summary>
	/// <param name="id">The id of the option group to retrieve.</param>
	/// <returns>The option group with the specified id of the option manager described 
	/// by this object, or a null reference if no such option group exists.</returns>
	public OptionGroupInfo? GetGroup(string id)
	{
		if (!mGroups.Find(ref id, out OptionGroupInfo desc))
		{
			return null;
		}
		return desc;
	}

	/// <summary>
	/// Gets a string consisting of the program name, version and copyright notice. 
	/// </summary>
	/// <param name="width">The total width in characters in which the string should be fitted</param>
	/// <returns>a string consisting of the program name, version and copyright notice.</returns>
	/// <remarks>This string is suitable for printing as the first output of a console application.</remarks>
	public string GetHeaderAsString(int? width = null)
	{
		StringBuilder result = new();
		result.Append(ApplicationName ?? "Unnamed application");
		if (ApplicationVersion != null)
		{
			result.Append("  ");
			result.Append(CommandLineStrings.Version);
			result.Append(' ');
			result.Append(ApplicationVersion);
		}
		result.Append(Environment.NewLine);

		if (ApplicationCopyright != null)
		{
			result.Append(ApplicationCopyright);
			result.Append(Environment.NewLine);
		}

		if (width != null)
		{
			return StringFormatter.WordWrap(result.ToString(), (int)width);
		}
		else
		{
			return result.ToString();
		}
	}

	/// <summary>
	/// Gets a formatted string describing the options and groups available.
	/// </summary>
	/// <param name="width">The maximum width of each line in the returned string.</param>
	/// <returns>A formatted string describing the options available in this parser</returns>
	/// <exception cref="ArgumentException">The specified width was too small to generate the requested list.</exception>
	public string GetOptionsAsString(int width = 10000)
	{
		// Remove spacing between columns.
		width -= ColumnSpacing;

		if (width < 2)
		{
			throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.WidthMustNotBeLessThan0, ColumnSpacing + 2), nameof(width));
		}

		// Default minimum width.
		int maxNameWidth = 5;

		// Get the maximum option name length from options not in groups.
		foreach (OptionInfo option in mOptions.Values)
		{
			maxNameWidth = Math.Max(option.Name.Length, maxNameWidth);
			foreach (string alias in option.Aliases)
			{
				maxNameWidth = Math.Max(alias.Length, maxNameWidth);
			}
		}

		// Get the maximum option name length from option inside groups.
		foreach (OptionGroupInfo group in mGroups.Values)
		{
			foreach (OptionInfo option in group.Options)
			{
				maxNameWidth = Math.Max(option.Name.Length, maxNameWidth);
				foreach (string alias in option.Aliases)
				{
					maxNameWidth = Math.Max(alias.Length, maxNameWidth);
				}
			}
		}

		// Add room for '--' and comma after the option name.
		maxNameWidth += 3;

		// Make sure the name column isn't more than half the specified width.
		maxNameWidth = Math.Min(width / 2, maxNameWidth);

		return GetOptionsAsString(maxNameWidth, width - maxNameWidth);
	}

	/// <summary>
	/// Gets a string describing all the options of this option manager. Usable for displaying as a help 
	/// message to the user, provided that descriptions for all options and groups are provided.
	/// </summary>
	/// <param name="nameColumnWidth">The width in characters of the column holding the names of the options.</param>
	/// <param name="descriptionColumnWidth">The width in characters of the column holding the descriptions of the options.</param>
	/// <returns>A string describing all the options of this option manager.</returns>
	public string GetOptionsAsString(int nameColumnWidth, int descriptionColumnWidth)
	{
		StringBuilder result = new();

		if (!mOptions.IsEmpty)
		{
			result.Append(StringFormatter.WordWrap(CommandLineStrings.Options, nameColumnWidth + descriptionColumnWidth + ColumnSpacing, WordWrappingMethod.Optimal, Alignment.Left, ' '));
			result.Append(Environment.NewLine);
			foreach (OptionInfo option in mOptions.Values)
			{
				result.Append(option.ToString(IndentWidth, nameColumnWidth, descriptionColumnWidth - IndentWidth));
				result.Append(Environment.NewLine);
			}
			result.Append(Environment.NewLine);
		}

		foreach (OptionGroupInfo group in mGroups.Values)
		{
			result.Append(group.ToString(0, nameColumnWidth, descriptionColumnWidth));
			result.Append(Environment.NewLine);
		}
		return result.ToString();
	}

	/// <summary>
	/// Gets the list of errors as a formatted string.
	/// </summary>
	/// <param name="width">The width of the field in which to format the error list.</param>
	/// <returns>The list of errors formatted inside a field of the specified <paramref name="width"/></returns>
	public string GetErrorsAsString(int width = int.MaxValue)
	{
		if (width < IndentWidth + 7)
		{
			throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.Arg0MustNotBeLessThan1, "width", IndentWidth + 7), nameof(width));
		}

		StringBuilder result = new();
		result.Append(StringFormatter.WordWrap("Errors:", width));
		result.Append(Environment.NewLine);

		StringBuilder errors = new();
		foreach (ErrorInfo error in mParser.Errors)
		{
			errors.Append(error.Message);
			if (error.FileName != null)
			{
				errors.Append(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.OnLine0InFile1, error.Line, error.FileName));
			}
			result.Append(StringFormatter.FormatInColumns(IndentWidth, 1, new ColumnInfo(1, "*"), new ColumnInfo(width - 1 - IndentWidth - 1, errors.ToString())));
			result.Append('\n');
			errors.Length = 0;
		}

		return result.ToString();
	}

	/// <summary>
	/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
	/// </summary>
	/// <returns>
	/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
	/// </returns>
	/// <remarks>This is equivalent to calling <see cref="ToString(int)">ToString(78)</see></remarks>
	public override string ToString()
	{
		return ToString(78);
	}

	/// <summary>
	/// Converts this <see cref="UsageInfo"/> instance to a string.
	/// </summary>
	/// <param name="width">The width of the field (in characters) in which to format the usage description.</param>
	/// <returns>A string including the header, and a complete list of the options and their descriptions
	/// available in this <see cref="UsageInfo"/> object.</returns>
	public string ToString(int width)
	{
		return ToString(width, false);
	}

	/// <summary>
	/// Converts this <see cref="UsageInfo"/> instance to a string.
	/// </summary>
	/// <param name="width">The width of the field (in characters) in which to format the usage description.</param>
	/// <param name="includeErrors">if set to <c>true</c> any errors that occured during parsing the command line will be included
	/// in the output.</param>
	/// <returns>A string including the header, optionally errors, and a complete list of the options and their descriptions
	/// available in this <see cref="UsageInfo"/> object.</returns>
	public string ToString(int width, bool includeErrors)
	{
		StringBuilder result = new();
		result.Append(GetHeaderAsString(width));
		result.Append(Environment.NewLine);

		if (mParser.HasErrors && includeErrors)
		{
			result.Append(GetErrorsAsString(width));
			result.Append(Environment.NewLine);
		}

		result.Append(GetOptionsAsString(width));
		result.Append(Environment.NewLine);
		return result.ToString();
	}

	#endregion
}