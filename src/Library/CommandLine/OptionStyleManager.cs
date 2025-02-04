using System;
using System.Diagnostics;
using System.Globalization;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Utility class for manipulating <see cref="OptionStyles"/>.
/// </summary>
public static class OptionStyleManager
{
	/// <summary>
	/// Determines whether all of the specified <paramref name="flags"/> are enabled in the specified <paramref name="optionStyle"/>.
	/// </summary>
	/// <param name="optionStyle">The option style.</param>
	/// <param name="flags">The flags.</param>
	/// <returns>
	/// 	<c>true</c> if all of the specified <paramref name="flags"/> are enabled in the specified <paramref name="optionStyle"/>; otherwise, <c>false</c>.
	/// </returns>
	public static bool IsAllEnabled(OptionStyles optionStyle, OptionStyles flags)
	{
		return (optionStyle & flags) == flags;
	}

	/// <summary>
	/// Determines whether any of the specified <paramref name="flags"/> are enabled in the specified <paramref name="optionStyle"/>.
	/// </summary>
	/// <param name="optionStyle">The option style.</param>
	/// <param name="flags">The flags.</param>
	/// <returns>
	/// 	<c>true</c> if any of the specified <paramref name="flags"/> are enabled in the specified <paramref name="optionStyle"/>; otherwise, <c>false</c>.
	/// </returns>
	public static bool IsAnyEnabled(OptionStyles optionStyle, OptionStyles flags)
	{
		return (optionStyle & flags) != OptionStyles.None;
	}

	/// <summary>
	/// Determines whether the specified option style is valid.
	/// </summary>
	/// <param name="optionStyle">The option style.</param>
	/// <returns>
	/// 	<c>true</c> if the specified option style is valid; otherwise, <c>false</c>.
	/// </returns>
	/// <remarks>An option style is invalid if the <see cref="OptionStyles.ShortUnix"/> flag is not enabled, but the 
	/// <see cref="OptionStyles.Group"/> or <see cref="OptionStyles.Plus"/> is. This normally doesn't occur
	/// if you only use the binary or to combine flags however, since the values of the group and plus 
	/// options also include the short unix style.</remarks>
	public static bool IsValid(OptionStyles optionStyle)
	{
		return !((optionStyle & OptionStyles.ShortUnix) == OptionStyles.None && (optionStyle & (OptionStyles.Plus | OptionStyles.Group)) != OptionStyles.None);
	}

	/// <summary>
	/// Gets the switch character used to specify an option of the specified style on the command line.
	/// </summary>
	/// <param name="optionStyle">The option style.</param>
	/// <param name="optionName">The name of the option.</param>
	/// <returns>the switch character used to specify this option.</returns>
	/// <remarks><para>If <paramref name="optionStyle"/> includes several option styles, the switch for the most 
	/// specific one representing a single option style prefix is returned. The switches for the corresponding
	/// styles are as follows (in order from most specific to least):</para>
	/// <para>
	/// <list type="table">
	/// <listheader>
	/// <item>
	/// <term>OptionStyle</term>
	/// <term>Prefix</term>
	/// </item>
	/// </listheader>
	/// <item>
	/// <term><i>All</i></term>
	/// <term><c>+</c></term>
	/// </item>
	/// <item>
	/// <term><b>Plus</b></term>
	/// <term><c>+</c></term>
	/// </item>
	/// <item>
	/// <term><i>Group</i></term>
	/// <term><c>-</c></term>
	/// </item>
	/// <item>
	/// <term><b>ShortUnix</b></term>
	/// <term><c>-</c></term>
	/// </item>
	/// <item>
	/// <term><b>File</b></term>
	/// <term><c>@</c></term>
	/// </item>
	/// <item>
	/// <term><b>LongUnix</b></term>
	/// <term><c>--</c></term>
	/// </item>
	/// <item>
	/// <term><b>Windows</b></term>
	/// <term><c>/</c></term>
	/// </item>
	/// </list>
	/// (Items in <i>italics</i> does not represent a single unique prefix)
	/// </para>
	/// </remarks>
	public static string GetPrefix(OptionStyles optionStyle, string optionName)
	{
		ArgumentNullException.ThrowIfNull(optionName);

		// The ordering here is important.
		if (IsAllEnabled(optionStyle, OptionStyles.LongUnix) && optionName.Length > 1)
		{
			return "--";
		}
		if (IsAllEnabled(optionStyle, OptionStyles.Plus))
		{
			return "+";
		}
		else if (IsAllEnabled(optionStyle, OptionStyles.ShortUnix))
		{
			return "-";
		}
		else if (IsAllEnabled(optionStyle, OptionStyles.File))
		{
			return "@";
		}
		else if (IsAnyEnabled(optionStyle, OptionStyles.LongUnix))
		{
			return "--";
		}
		else if (IsAllEnabled(optionStyle, OptionStyles.Windows))
		{
			return "/";
		}
		else
		{
			string message = String.Format(
				CultureInfo.CurrentUICulture,
				"Internal error: An OptionNameToken was created with an unsupported set of option style flags ({0})",
				optionStyle.ToString()
			);
			throw new NotImplementedException(message);
		}
	}

	/// <summary>
	/// Prefixes the option name with the prefix(es) with which it should be used.
	/// </summary>
	/// <param name="optionStyle">The option style.</param>
	/// <param name="optionName">Name of the option.</param>
	/// <returns>A string representing the name of the option prefixed according to the enabled option styles specified.</returns>
	/// <remarks>This method always prefers prefixing options with unix style to windows style prefixes if both are 
	/// enabled.  If <see cref="OptionStyles.Plus"/> is enabled, the option will be prefixed with "[+|-]" to indicate
	/// that either prefix may be used.</remarks>
	public static string PrefixOptionForDescription(OptionStyles optionStyle, string optionName)
	{
		ArgumentNullException.ThrowIfNull(optionName);

		if (IsAllEnabled(optionStyle, OptionStyles.Plus))
		{
			return "[+|-]" + optionName;
		}
		else if (optionName.Length == 1)
		{
			if (IsAllEnabled(optionStyle, OptionStyles.ShortUnix))
			{
				return "-" + optionName;
			}
			else if (IsAllEnabled(optionStyle, OptionStyles.LongUnix))
			{
				return "--" + optionName;
			}
			else
			{
				Debug.Assert(IsAllEnabled(optionStyle, OptionStyles.Windows));
				return "/" + optionName;
			}
		}
		else if (!IsAllEnabled(optionStyle, OptionStyles.Group) && IsAllEnabled(optionStyle, OptionStyles.ShortUnix))
		{
			return "-" + optionName;
		}
		else if (IsAllEnabled(optionStyle, OptionStyles.LongUnix))
		{
			return "--" + optionName;
		}
		else
		{
			Debug.Assert(IsAllEnabled(optionStyle, OptionStyles.Windows));
			return "/" + optionName;
		}
	}
}