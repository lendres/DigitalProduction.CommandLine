namespace DigitalProduction.CommandLine;

/// <summary>
/// Flags indicating the option styles recognized by the <see cref="CommandLineParser"/>. 
/// </summary>
/// <remarks>Option styles indicates how options may be specified on the command line, for an example the 
/// <see cref="Windows"/> style dictate that options are prefixed by a slash '/' while the <see cref="LongUnix"/>
/// style is prefixed by a double dash ('--'). Options may be freely combined using the binary or (|) operator.
/// <note>Note that both <see cref="Plus"/> and <see cref="Group"/> implies the <see cref="ShortUnix"/> option 
/// style. You should never use binary operators to prevent this implication.</note></remarks>

// This is deliberate, they are still flags.
public enum OptionStyles : int
{
	/// <summary>
	/// No options are enabled nor will be parsed. Everything on the command line is treated 
	/// as remaining arguments (see <see cref="CommandLineParser.RemainingArguments"/>).
	/// </summary>
	None = 0x00,

	/// <summary>
	/// The windows style for options is recognized. This means that options are prefixed 
	/// with the slash '/' character. An option specified in this style will never be 
	/// grouped (see <see cref="Group"/>).
	/// </summary>
	Windows = 0x01,

	/// <summary>
	/// The long unix style for options is recognized. This means that options with long 
	/// names (more than one character) are prefixed with two dashes '--'. (Also options
	/// of only one character in length can be specified in this style). Options specified
	/// in this style will never be grouped (see <see cref="Group"/>).
	/// </summary>
	LongUnix = 0x02,

	/// <summary>
	/// Option files are recognized and parsed automatically by the <see cref="CommandLineParser"/>.
	/// This allows specifying a file name prefixed by the '@' character on the command line. Any 
	/// such file will then be opened and parsed for additional command line options by the 
	/// command line parser.  The syntax for the options (or additional arguments) specified in the
	/// file is the very same as that for the command line itself).
	/// </summary>
	File = 0x04,

	/// <summary>
	/// The short unix style for options is recognized. This means that options are prefixed
	/// by a single dash ('-').  If <see cref="Group"/> is also specified, any options 
	/// specified in this style will be grouped.
	/// </summary>
	ShortUnix = 0x08,

	/// <summary>
	/// The plus style for options is recognized. Specifying this style implies the <see cref="ShortUnix"/>
	/// style. This means that options can be prefixed with either the dash ('-') character or the 
	/// ('+') character. This can be useful for boolean options when used with the <see cref="BoolFunction.UsePrefix"/>
	/// option, in which case the prefix of the option will indicate what value to set the option to.
	/// Options specified in this style will be grouped if the <see cref="Group"/> option is also specified.
	/// </summary>
	Plus = 0x10 | ShortUnix,

	/// <summary>
	/// Grouping of options is enabled for the <see cref="ShortUnix"/> and <see cref="Plus"/> style. Specifying
	/// this style also implies the <see cref="ShortUnix"/> style.  Grouping of options means that several options
	/// with names only one character long can be concatenated on the command line. For an example the command line
	/// "tar -xvzf file.tar.gz" would be interpreted as "tar -x -v -z -f file.tar.gz". If this option is specified
	/// and option styles with names longer than one character should also be recognized the <see cref="LongUnix"/>
	/// option style should also be enabled.
	/// </summary>
	Group = 0x20 | ShortUnix,

	/// <summary>
	/// This option style indicates a combination of the <see cref="ShortUnix"/> and <see cref="LongUnix"/> flags.
	/// </summary>
	Unix = ShortUnix | LongUnix,

	/// <summary>
	/// This means all option styles above are enabled.
	/// </summary>
	All = 0x3F
}