using System;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Attribute used to specify an option group in a command line option manager object.
/// </summary>
/// <remarks>Option groups are used for logical grouping of options. This is in part useful for 
/// grouping related options in the usage documentation generated, but also to place certain 
/// restrictions on a group of options.</remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="CommandLineOptionGroupAttribute"/> class.
/// </remarks>
/// <param name="id">The id.</param>
/// <remarks>The id must be unique within the option manager object.</remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class CommandLineOptionGroupAttribute(string id) : System.Attribute
{
	#region Private Fields

	private bool?					_requireExplicitAssignment;

	#endregion

	#region Properties

	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	/// <value>The name.</value>
	/// <remarks>This is the name that will be displayed as a headline for the options contained in the
	/// group in any generated documentation. If not explicitly set it will be the same as <see cref="Id"/>.</remarks>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the description.
	/// </summary>
	/// <value>The description.</value>
	public string? Description { get; set; } = null;

	/// <summary>
	/// Gets the id.
	/// </summary>
	/// <value>The id.</value>
	public string Id { get; } = id;

	/// <summary>
	/// Gets or sets the requirements placed on the options in this group.
	/// </summary>
	/// <value>requirements placed on the options in this group.</value>
	public OptionGroupRequirement Require { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether explicit assignment is required for the options
	/// of this group.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if explicit assignment is required by the options of this group; otherwise, <c>false</c>.        
	/// </value>
	/// <remarks>This defaults all options in this group to the specified value, but setting another value 
	/// explicitly on an option overrides this setting.</remarks>
	public bool RequireExplicitAssignment
	{
		get
		{
			System.Diagnostics.Debug.Assert(_requireExplicitAssignment != null);
			return _requireExplicitAssignment.Value;
		}
		set => _requireExplicitAssignment = value;
	}

	#endregion

	#region Internal Properties

	/// <summary>
	/// Gets a value indicating whether this instance has 
	/// </summary>
	/// <value>
	/// 	<c>true</c> if this instance has specified a require explicit assignment value; otherwise, <c>false</c>.
	/// </value>
	internal bool HasRequireExplicitAssignment { get => _requireExplicitAssignment.HasValue; }

	#endregion
}