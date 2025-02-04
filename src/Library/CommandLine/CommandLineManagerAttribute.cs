using System;
using System.Reflection;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Attribute describing the command line manager class that will be used for storing the values
/// of the command line options parsed by the <see cref="CommandLineParser"/>.
/// </summary>
/// <remarks>This attribute is required for any class that is to act as a command line manager for parsing. It
/// may only be specified on a class and only one occurence may be specified.</remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class CommandLineManagerAttribute : System.Attribute
{
	#region Private fields

	private string			mCopyright						= string.Empty;
	private string			mApplicationName				= string.Empty;
	private string			mDescription					= string.Empty;
	private bool			mIsCaseSensitive;
	private string			mVersion						= string.Empty;
	private OptionStyles	mOptionStyle					= OptionStyles.Windows | OptionStyles.File | OptionStyles.ShortUnix;
	private bool			mRequireExplicitAssignment;

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="CommandLineManagerAttribute"/> class.
	/// </summary>
	public CommandLineManagerAttribute()
	{
		Assembly? assembly = Assembly.GetEntryAssembly();
		System.Diagnostics.Debug.Assert(assembly != null);

		AssemblyName name	= assembly.GetName();
		Version				= name.Version?.ToString() ?? "";

		foreach (object objAttribute in assembly.GetCustomAttributes(false))
		{
			if (objAttribute is AssemblyCopyrightAttribute copyrightAttribute)
			{
				Copyright = copyrightAttribute.Copyright;
				continue;
			}

			if (objAttribute is AssemblyTitleAttribute titleAttribute)
			{
				ApplicationName = titleAttribute.Title;
				continue;
			}

			if (objAttribute is AssemblyDescriptionAttribute descriptionAttribute)
			{
				Description = descriptionAttribute.Description;
				continue;
			}
		}
	}

	#endregion

	#region Public Properties

	/// <summary>
	/// Gets or sets the name of the application.
	/// </summary>
	/// <value>The name of the application.</value>
	/// <remarks>If not explicitly specified, this value will be retrieved from the assembly information.</remarks>
	public string ApplicationName { get => mApplicationName; set => mApplicationName = value; }

	/// <summary>
	/// Gets or sets the copyright message.
	/// </summary>
	/// <value>The copyright message.</value>
	/// <remarks>If not explicitly specified, this value will be retrieved from the assembly information.</remarks>
	public string Copyright { get => mCopyright; set => mCopyright = value; }

	/// <summary>
	/// Gets or sets the description.
	/// </summary>
	/// <value>The description.</value>
	/// <remarks>If not explicitly specified, the application will be retrieved from the assembly information.</remarks>
	public string Description { get => mDescription; set => mDescription = value; }

	/// <summary>
	/// Gets or sets the version of this application.
	/// </summary>
	/// <value>The version of this application.</value>
	/// <remarks>If not explicitly specified, the application will be retrieved from the assembly information.</remarks>
	public string Version { get => mVersion; set => mVersion = value; }

	/// <summary>
	/// Gets or sets a value indicating whether options are case sensitive or not.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if options are case sensitive; otherwise, <c>false</c>.
	/// </value>
	public bool IsCaseSensitive { get => mIsCaseSensitive; set => mIsCaseSensitive = value; }

	/// <summary>
	/// Gets or sets the enabled option styles.
	/// </summary>
	/// <value>The enabled option styles.</value>
	/// <remarks>The default value for this property is to enable the Windows, ShortUnix and File style.</remarks>
	public OptionStyles EnabledOptionStyles { get => mOptionStyle; set => mOptionStyle = value; }

	/// <summary>
	/// Gets or sets a value indicating whether options in this manager requires explicit assignment or not.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if options require explicit assignment; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>Explicit assignment means that the assignment character must be used to assign a value
	/// to an option. If set to false a space character after an option followed by a value will be interpreted
	/// as an assignment.
	/// <note>This value sets the default value for all options contained in this manager, but may be overridden for
	/// individual groups.</note></remarks>
	public bool RequireExplicitAssignment { get => mRequireExplicitAssignment; set => mRequireExplicitAssignment = value; }

	#endregion
}