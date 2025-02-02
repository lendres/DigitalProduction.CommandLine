/* Copyright (c) Peter Palotas 2007
 *  
 *  All rights reserved.
 *  
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions are
 *  met:
 *  
 *      * Redistributions of source code must retain the above copyright 
 *        notice, this list of conditions and the following disclaimer.    
 *      * Redistributions in binary form must reproduce the above copyright 
 *        notice, this list of conditions and the following disclaimer in 
 *        the documentation and/or other materials provided with the distribution.
 *      * Neither the name of the copyright holder nor the names of its 
 *        contributors may be used to endorse or promote products derived 
 *        from this software without specific prior written permission.
 *  
 *  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 *  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 *  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 *  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 *  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 *  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 *  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 *  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *  
 *  $Id: CommandLineManagerAttribute.cs 4 2007-07-30 18:32:30Z palotas $
 */
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