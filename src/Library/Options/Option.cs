using C5;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using SCG = System.Collections.Generic;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Representation of a command line option. 
/// </summary>
/// <remarks>Objects of this class is created by the constructor of <see cref="CommandLineParser"/>. This object is then used
/// for setting and getting values of the option manager object used by the parser.</remarks>
internal class Option : IOption
{
	#region Private Fields

	/// <summary>
	/// Counts the number of times the value was set for this option
	/// </summary>
	private int								_setCount;
	private readonly Type					_optionType;
	private readonly MemberInfo				_member;
	private string							_description					= string.Empty;
	private readonly string					_name							= string.Empty;
	private readonly object					_object;
	private readonly OptionGroup?			_group;
	private readonly int					_maxOccurs;
	private readonly int					_minOccurs;
	private readonly BoolFunction			_usage;
	private readonly ArrayList<Option>		_prohibitedBy					= [];
	private readonly NumberFormatInfo		_numberFormatInfo;
	private readonly object?				_defaultValue;
	private bool							_requireExplicitAssignment;
	private readonly object?				_minValue;
	private readonly object?				_maxValue;
	private readonly ArrayList<string>		_aliases						= [];
	private readonly TreeSet<string>		_enumerationValues				= [];

	#endregion

	#region Constructor

	/// <summary>
	/// Initializes a new instance of the <see cref="Option"/> class.
	/// </summary>
	/// <param name="attribute">The attribute describing this option.</param>
	/// <param name="memberInfo">The <see cref="MemberInfo"/> object pointing to the member to which the attribute was applied.</param>
	/// <param name="cmdLineObject">The command line manager object.</param>
	/// <param name="optionGroups">A complete list of all available option groups.</param>
	/// <param name="numberFormatInfo">The number format info to use for parsing numerical arguments.</param>
	public Option(CommandLineOptionAttribute attribute, MemberInfo memberInfo, object cmdLineObject, ICollection<OptionGroup> optionGroups, NumberFormatInfo numberFormatInfo)
	{
		_object				= cmdLineObject;
		_member				= memberInfo;
		_usage				= attribute.BoolFunction;
		_description		= attribute.Description;
		_numberFormatInfo	= numberFormatInfo ?? CultureInfo.CurrentCulture.NumberFormat;
		_defaultValue		= attribute.DefaultAssignmentValue;
		_minValue			= attribute.MinValue;
		_maxValue			= attribute.MaxValue;

		// Check the validity of the member for which this attribute was defined
		switch (memberInfo.MemberType)
		{
			case MemberTypes.Field:
				FieldInfo fieldInfo = (FieldInfo)memberInfo;
				if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral)
				{
					throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo, "Illegal field for this attribute; field must be writeable");
				}

				_optionType = fieldInfo.FieldType;
				break;
			case MemberTypes.Method:
				MethodInfo method = (MethodInfo)memberInfo;
				ParameterInfo[] parameters = method.GetParameters();

				if (parameters.Length != 1)
				{
					throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
						"Illegal method for this attribute; the method must accept exactly one parameter");
				}

				if (parameters[0].IsOut)
				{
					throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
						"Illegal method for this attribute; the parameter of the method must not be an out parameter");
				}

				if (IsArray(parameters[0].ParameterType) || IsCollectionType(parameters[0].ParameterType))
				{
					throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
						"Illegal method for this attribute; the parameter of the method must be a non-array and non-collection type");
				}

				_optionType = parameters[0].ParameterType;
				break;
			case MemberTypes.Property:
				PropertyInfo propInfo = (PropertyInfo)memberInfo;

				if (!propInfo.CanWrite && !IsCollectionType(propInfo.PropertyType))
				{
					throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
						"Illegal property for this attribute; property for non-collection type must be writable");
				}

				if (!propInfo.CanRead && IsCollectionType(propInfo.PropertyType))
				{
					throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
						"Illegal property for this attribute; property for collection type must be readable");
				}

				if (!(propInfo.CanRead && propInfo.CanWrite) && IsArray(propInfo.PropertyType))
				{
					throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
						"Illegal property for this attribute; property representing array type must be both readable and writeable");
				}

				_optionType = propInfo.PropertyType;
				break;
			default:
				throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
					"Illegal member for this attribute; member must be a property, method (accepting one parameter) or a field");
		}

		_minOccurs = attribute.MinOccurs;

		// MaxOccurs does not have a default value (since this is different for various types), so we set it here.
		if (!attribute.IsMaxOccursSet)
		{
			// Use default setting for MaxOccurs
			if (IsArray(_optionType) || IsCollectionType(_optionType))
			{
				_maxOccurs = 0; // Unlimited 
			}
			else
			{
				_maxOccurs = 1;
			}
		}
		else
		{
			_maxOccurs = attribute.MaxOccurs;
		}

		if (_minOccurs > _maxOccurs && _maxOccurs > 0)
		{
			throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
				String.Format(CultureInfo.CurrentUICulture, "MinOccurs ({0}) must not be larger than MaxOccurs ({1})", _minOccurs, _maxOccurs));
		}

		if (_maxOccurs != 1 && !(IsArray(_optionType) || IsCollectionType(_optionType)) && _member.MemberType != MemberTypes.Method)
		{
			throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
				"Invalid cardinality for member; MaxOccurs must be equal to one (1) for any non-array or non-collection type");
		}

		CommandLineManagerAttribute objectAttr = Attribute.GetCustomAttribute(_object.GetType(), typeof(CommandLineManagerAttribute)) as CommandLineManagerAttribute ??
			throw new AttributeException(string.Format(CultureInfo.CurrentUICulture,
					"Class {0} contains a CommandLineOptionAttribute, but does not have the attribute CommandLineObjectAttribute", _object.GetType().FullName));


		// Assign the name of this option from the member itself if no name is explicitly provided 
		if (attribute.Name == null)
		{
			_name = memberInfo.Name;
		}
		else
		{
			_name = attribute.Name;
		}

		// Find the group (if any) that this option belongs to in the list of available option groups.
		if (attribute.GroupId != null)
		{
			if (!optionGroups.Find(new Func<OptionGroup, bool>(
				delegate (OptionGroup searchGroup)
				{
					return attribute.GroupId.Equals(searchGroup.Id);
				}), out _group))
			{
				throw new LogicException(String.Format(CultureInfo.CurrentUICulture, "Undefined group {0} referenced from  member {1} in {2}", attribute.GroupId, memberInfo.Name, cmdLineObject.GetType().FullName));
			}
			_group.Options.Add(_name, this);
		}

		// Recursively find out if this option requires explicit assignment.
		if (attribute.DoesRequireExplicitAssignment.HasValue)
		{
			_requireExplicitAssignment = attribute.DoesRequireExplicitAssignment.Value;
		}
		else if (_group != null)
		{
			_requireExplicitAssignment = _group.RequireExplicitAssignment;
		}
		else
		{
			_requireExplicitAssignment = objectAttr.RequireExplicitAssignment;
		}

		// Make sure the type of the field, property, or method is supported.
		if (!IsTypeSupported(_optionType))
		{
			throw new AttributeException(typeof(CommandLineOptionAttribute), _member, "Unsupported type for command line option.");
		}

		// Make sure MinValue and MaxValue is not specified for any non-numerical type.
		if (_minValue != null || _maxValue != null)
		{
			if (!IsNumericalType)
			{
				throw new AttributeException(typeof(CommandLineOptionAttribute), _member, "MinValue and MaxValue must not be specified for a non-numerical type");
			}
			else
			{
				Debug.Assert(_minValue != null);
				if (!_minValue.GetType().IsAssignableFrom(GetBaseType(_optionType)))
				{
					throw new AttributeException(typeof(CommandLineOptionAttribute), _member, "Illegal value for MinValue or MaxValue, not the same type as option");
				}
			}
		}

		// Some special checks for numerical types
		if (IsNumericalType)
		{
			// Assign the default MinValue if it was not set and this is a numerical type
			if (IsNumericalType && _minValue == null)
			{
				object? value = GetBaseType(_optionType)?.GetField("MinValue", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
				Debug.Assert(value != null);
				_minValue = value;
			}

			// Assign the defaul MaxValue if it was not set and this is a numerical type
			if (IsNumericalType && _maxValue == null)
			{
				object? value = GetBaseType(_optionType)?.GetField("MaxValue", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
				Debug.Assert(value != null);
				_maxValue = value;
			}

			// Check that MinValue <= MaxValue
			object? minValue = MinValue;
			Debug.Assert(minValue != null);
			if (IsNumericalType && ((IComparable)minValue).CompareTo(MaxValue) > 0)
			{
				throw new AttributeException(typeof(CommandLineOptionAttribute), _member, "MinValue must not be greater than MaxValue");
			}
		}

		// Check that the DefaultValue is not set if the option does not require explicit assignment. 
		// If it were allowed, it would be ambiguos for an option separated from a value by a white space character
		// since we wouldn't know whether that would set the default value or assign it to the following value.
		if (_defaultValue != null && !_requireExplicitAssignment)
		{
			throw new AttributeException(typeof(CommandLineOptionAttribute), _member, "DefaultValue must not be specified when RequireExplicitAssignment is set to false");
		}

		// Check that the type of any set default value matches that of this option, or is string, and
		// convert it to the type of this option.
		if (_defaultValue != null)
		{
			if (_defaultValue.GetType() == typeof(string))
			{
				try
				{
					_defaultValue = GetCheckedValueForSetOperation(_defaultValue);
				}
				catch (OverflowException)
				{
					throw new AttributeException(typeof(CommandLineOptionAttribute), _member,
						"DefaultValue was less than MinValue or greater than MaxValue for this option");
				}
				catch (FormatException)
				{
					throw new AttributeException(typeof(CommandLineOptionAttribute), _member,
						"DefaultValue was not specified in the correct format for the type of this option");
				}
			}
			else if (GetBaseType(_optionType) != _defaultValue.GetType())
			{
				try
				{
					Type? baseType = GetBaseType(_optionType);
					Debug.Assert(baseType != null);
					_defaultValue = Convert.ChangeType(_defaultValue, baseType, _numberFormatInfo);
				}
				catch (InvalidCastException)
				{
					throw new AttributeException(typeof(CommandLineOptionAttribute), _member,
						"The type of the DefaultValue specified is not compatible with the type of the member to which this attribute applies");
				}
			}
		}

		// If this is an enum, check that it doesn't have members only distinguishable by case, and
		// add the members to the mEnumerationValues set for speedy access when checking values assigned 
		// to this member.
		Type? type = GetBaseType(_optionType);
		Debug.Assert(type != null);
		if (type.IsEnum)
		{
			_enumerationValues = new TreeSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (FieldInfo field in type.GetFields())
			{
				if (field.IsLiteral)
				{
					if (_enumerationValues.Contains(field.Name))
					{
						throw new AttributeException(typeof(CommandLineOptionAttribute), _member,
							"This enumeration is not allowed as a command line option since it contains fields that differ only by case");
					}
					_enumerationValues.Add(field.Name);
				}
			}
		}
	}

	#endregion

	#region Public Properties

	/// <summary>
	/// Sets the value of the member of the option manager representing this option.
	/// </summary>
	/// <value>the value of the member of the option manager representing this option.</value>
	public object Value
	{
		set
		{
			if (++_setCount > MaxOccurs && MaxOccurs != 0)
			{
				throw new InvalidOperationException(CommandLineStrings.InternalErrorOptionValueWasSetMoreThanMaxOccursNumberOfTimes);
			}

			switch (_member.MemberType)
			{
				case MemberTypes.Field:
					SetFieldValue(value);
					break;
				case MemberTypes.Property:
					SetPropertyValue(value);
					break;
				case MemberTypes.Method:
					SetMethodValue(value);
					break;
				default:
					throw new LogicException(
						String.Format(CultureInfo.CurrentUICulture,
						"Internal error; unimplemented member type {0} found in Option.Value", _member.MemberType.ToString()));
			}
		}
	}

	/// <summary>
	/// Gets a value indicating whether this option requires a value to be assigned to it.
	/// </summary>
	/// <value><c>true</c> if this option requires a value to be assigned to it; otherwise, <c>false</c>.</value>
	public bool RequiresValue { get => AcceptsValue && !HasDefaultValue; }

	/// <summary>
	/// Gets or sets a value indicating whether this option requires explicit assignment.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if this option requires explicit assignment; otherwise, <c>false</c>.
	/// </value>
	public bool RequireExplicitAssignment { get => _requireExplicitAssignment; set => _requireExplicitAssignment = value; }

	/// <summary>
	/// Gets the collection containing the options prohibiting this option from being specified.
	/// </summary>
	/// <value>The collection containing the options prohibiting this option from being specified.</value>
	public ICollection<Option> ProhibitedBy { get => _prohibitedBy; }

	/// <summary>
	/// Gets the group to which this option belongs, or null if this option does not belong to any group.
	/// </summary>
	/// <value>The group to which this option belongs, or null if this option does not belong to any group.</value>
	public OptionGroup? Group {	get => _group; }

	/// <summary>
	/// Gets the name of this option
	/// </summary>
	/// <value>The name of this option</value>
	public string Name { get => _name; }

	/// <summary>
	/// Gets the bool function used by this option
	/// </summary>
	/// <value>The bool function used by this option</value>
	public BoolFunction BoolFunction { get => _usage; }

	/// <summary>
	/// Gets the max occurs.
	/// </summary>
	/// <value>The max occurs.</value>
	public int MaxOccurs { get => _maxOccurs; }

	/// <summary>
	/// Gets the min occurs.
	/// </summary>
	/// <value>The min occurs.</value>
	public int MinOccurs { get => _minOccurs; }

	/// <summary>
	/// Gets or sets the description.
	/// </summary>
	/// <value>The description.</value>
	public string Description { get => _description; set => _description = value; }

	/// <summary>
	/// Gets or sets the number of times the value of this option has been set.
	/// </summary>
	/// <value>The number of times the value of this option has been set.</value>
	public int SetCount { get => _setCount; set => _setCount = value; }

	/// <summary>
	/// Gets a value indicating whether a value may be assigned to this option.
	/// </summary>
	/// <value><c>true</c> if a value may be assigned to this option; otherwise, <c>false</c>.</value>
	public bool AcceptsValue
	{
		get
		{
			return	(GetBaseType(_optionType) != typeof(bool) && 
					 GetBaseType(_optionType) != typeof(bool?)) ||
					 BoolFunction == BoolFunction.Value;
		}
	}

	/// <summary>
	/// Gets a value indicating whether this instance has default value.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if this instance has default value; otherwise, <c>false</c>.
	/// </value>
	public bool HasDefaultValue { get => _defaultValue != null; }

	/// <summary>
	/// Gets a value indicating whether this instance is boolean type.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if this instance is boolean type; otherwise, <c>false</c>.
	/// </value>
	public bool IsBooleanType
	{
		get
		{
			return	(GetBaseType(_optionType)?.Equals(typeof(bool)) ?? false) ||
					(GetBaseType(_optionType)?.Equals(typeof(bool?)) ?? false);
		}
	}

	/// <summary>
	/// Gets a value indicating whether this instance is an alias.
	/// </summary>
	/// <value><c>true</c> if this instance is alias; otherwise, <c>false</c>.</value>
	public bool IsAlias { get => false; }

	/// <summary>
	/// Gets the defining option.
	/// </summary>
	/// <value>The defining option.</value>
	/// <remarks>For an <see cref="Option"/> the value returned will be equal to the Option itself, for an <see cref="OptionAlias"/> it
	/// will be the <see cref="Option"/> to which the alias refers.</remarks>
	public Option DefiningOption { get => this; }

	/// <summary>
	/// Gets the min value.
	/// </summary>
	/// <value>The min value, or null if no minimum value was specified.</value>
	public object? MinValue { get => _minValue; }

	/// <summary>
	/// Gets the max value.
	/// </summary>
	/// <value>The max value or null if no maximum value was specified.</value>
	public object? MaxValue { get => _maxValue; }

	/// <summary>
	/// Gets a value indicating whether this instance is integral type.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if this instance is integral type; otherwise, <c>false</c>.
	/// </value>
	public bool IsIntegralType
	{
		get
		{
			Type? baseType = GetBaseType(_optionType);
			return baseType == typeof(byte) ||
				baseType == typeof(short) ||
				baseType == typeof(int) ||
				baseType == typeof(long) ||
				baseType == typeof(ushort) ||
				baseType == typeof(uint) ||
				baseType == typeof(ulong);

		}
	}

	/// <summary>
	/// Gets a value indicating whether this instance is floating point type.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if this instance is floating point type; otherwise, <c>false</c>.
	/// </value>
	public bool IsFloatingPointType
	{
		get
		{
			Type? baseType = GetBaseType(_optionType);
			return baseType == typeof(float) || baseType == typeof(double);
		}
	}

	/// <summary>
	/// Gets a value indicating whether this instance is decimal type.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if this instance is decimal type; otherwise, <c>false</c>.
	/// </value>
	public bool IsDecimalType { get => GetBaseType(_optionType) == typeof(decimal); }

	/// <summary>
	/// Gets a value indicating whether this instance is numerical type.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if this instance is numerical type; otherwise, <c>false</c>.
	/// </value>
	public bool IsNumericalType { get => IsIntegralType || IsDecimalType || IsFloatingPointType; }

	/// <summary>
	/// Gets the valid enumeration values.
	/// </summary>
	/// <value>The valid enumeration values of this option if the base type for this option is an enum, or a null reference otherwise.</value>
	public ICollection<string> ValidEnumerationValues { get => new GuardedCollection<string>(_enumerationValues); }

	/// <summary>
	/// Gets the names of the aliases referring to this option.
	/// </summary>
	/// <value>The names of the aliases referring to this option.</value>
	public SCG.IEnumerable<string> Aliases { get => _aliases; }

	#endregion

	#region Public Methods

	/// <summary>
	/// Sets this option to its default value.
	/// </summary>
	public void SetDefaultValue()
	{
		Debug.Assert(_defaultValue != null);
		Value = _defaultValue;
	}

	/// <summary>
	/// Adds the alias.
	/// </summary>
	/// <param name="alias">The alias.</param>
	public void AddAlias(string alias)
	{
		_aliases.Add(alias);
	}

	#endregion

	#region Private Methods

	private static void AppendToCollection(object collection, object value)
	{
		Debug.Assert(IsCollectionType(collection.GetType()));
		collection.GetType().InvokeMember("Add", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, collection, [value], CultureInfo.CurrentUICulture);
	}

	private Array AppendToArray(Array array, object value)
	{
		// Get the length of the current array
		int arrayLength = array.GetLength(0);

		// Allocate a new array of the same type but one item longer
		Type? type = _optionType.GetElementType();
		Debug.Assert(type != null);
		Array newArray = Array.CreateInstance(type, arrayLength + 1);

		// Copy the contents of the old array to the new array
		Array.Copy((Array)array, newArray, arrayLength);

		// Set the value of the new (last) element to the specified value
		newArray.SetValue(GetCheckedValueForSetOperation(value), arrayLength);

		return newArray;
	}

	private void SetFieldValue(object value)
	{
		FieldInfo field = (FieldInfo)_member;
		if (field.FieldType.IsArray)
		{
			Array? array = field.GetValue(_object) as Array;
			Debug.Assert(array != null);
			field.SetValue(_object, AppendToArray(array, value));
		}
		else if (IsCollectionType(_optionType))
		{
			object? fieldValue = field.GetValue(_object);
			Debug.Assert(fieldValue != null);
			AppendToCollection(fieldValue, value);
		}
		else
		{
			// Otherwise we just assign the value
			field.SetValue(_object, GetCheckedValueForSetOperation(value));
		}
	}

	private void SetPropertyValue(object value)
	{
		PropertyInfo property = (PropertyInfo)_member;
		if (property.PropertyType.IsArray)
		{
			Array? array = property.GetValue(_object, null) as Array;
			Debug.Assert(array != null);
			property.SetValue(_object, AppendToArray(array, value), null);
		}
		else if (IsCollectionType(_optionType))
		{
			Array? array = property.GetValue(_object, null) as Array;
			Debug.Assert(array != null);
			AppendToCollection(array, value);
		}
		else
		{
			property.SetValue(_object, GetCheckedValueForSetOperation(value), null);
		}
	}

	private void SetMethodValue(object value)
	{
		MethodInfo method = (MethodInfo)_member;
		method.Invoke(_object, [GetCheckedValueForSetOperation(value)]);
	}

	private static bool IsArray(Type type)
	{
		return type.IsArray;
	}

	private static bool IsNonGenericCollectionType(Type type)
	{
		return type.GetInterface("System.Collections.ICollection") != null;
	}

	private static bool IsGenericCollectionType(Type type)
	{
		return type.GetInterface("System.Collections.Generic.ICollection`1") != null || type.GetInterface("C5.IExtensible`1") != null;
	}

	private static bool IsCollectionType(Type type)
	{
		return IsNonGenericCollectionType(type) || IsGenericCollectionType(type);
	}

	private static Type? GetBaseType(Type type)
	{
		if (IsArray(type))
		{
			return type.GetElementType();
		}
		else if (IsNonGenericCollectionType(type))
		{
			return typeof(object);
		}
		else if (IsGenericCollectionType(type))
		{
			Debug.Assert(type.GetGenericArguments().Length == 1);
			return type.GetGenericArguments()[0];
		}
		else
		{
			return type;
		}
	}

	private static bool IsTypeSupported(Type type)
	{
		if (IsArray(type) && type.GetArrayRank() != 1)
		{
			return false;
		}
		else if (IsNonGenericCollectionType(type))
		{
			return true;
		}

		Type? baseType = GetBaseType(type);
		Debug.Assert(baseType != null);

		return
			baseType.Equals(typeof(bool)) ||
			baseType.Equals(typeof(bool?)) ||
			baseType.Equals(typeof(byte)) ||
			baseType.Equals(typeof(byte?)) ||
			baseType.Equals(typeof(sbyte)) ||
			baseType.Equals(typeof(sbyte?)) ||
			baseType.Equals(typeof(char)) ||
			baseType.Equals(typeof(char?)) ||
			baseType.Equals(typeof(decimal)) ||
			baseType.Equals(typeof(decimal?)) ||
			baseType.Equals(typeof(double)) ||
			baseType.Equals(typeof(double?)) ||
			baseType.Equals(typeof(float)) ||
			baseType.Equals(typeof(float?)) ||
			baseType.Equals(typeof(int)) ||
			baseType.Equals(typeof(int?)) ||
			baseType.Equals(typeof(uint)) ||
			baseType.Equals(typeof(uint?)) ||
			baseType.Equals(typeof(long)) ||
			baseType.Equals(typeof(long?)) ||
			baseType.Equals(typeof(ulong)) ||
			baseType.Equals(typeof(ulong?)) ||
			baseType.Equals(typeof(short)) ||
			baseType.Equals(typeof(short?)) ||
			baseType.Equals(typeof(ushort)) ||
			baseType.Equals(typeof(ushort?)) ||
			baseType.Equals(typeof(string)) ||
			baseType.IsEnum;
	}

	/// <summary>
	/// Gets the checked value for set operation.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns></returns>
	/// <exception cref="FormatException">The string supplied for the value was not in the correct format</exception>
	/// <exception cref="OverflowException">The value supplied represented a number less than <see cref="MinValue"/> or greater than <see cref="MaxValue"/></exception>
	private object GetCheckedValueForSetOperation(object value)
	{
		ArgumentNullException.ThrowIfNull(value);

		try
		{
			value = ConvertValueTypeForSetOperation(value);
		}
		catch (TargetInvocationException tie)
		{
			// We need to transform this exception to an OverflowException or FormatException
			// if that is the type of the inner exception.
			if (tie.InnerException?.GetType() == typeof(OverflowException) || tie.InnerException?.GetType() == typeof(FormatException)
				|| tie.InnerException?.GetType() == typeof(InvalidOptionValueException))
			{
				throw tie.InnerException;
			}
			else
			{
				throw;
			}
		}

		if (IsNumericalType)
		{
			Debug.Assert(MinValue != null && MaxValue != null);
			// All numerical types are comparable and MinValue and MaxValue are both automatically set
			// in the constructor for any numerical type supported by this class.
			IComparable comparable = (IComparable)value;
			if (comparable.CompareTo(MinValue) < 0 || comparable.CompareTo(MaxValue) > 0)
			{
				throw new OverflowException(CommandLineStrings.ValueWasEitherTooLargeOrTooSmallForThisOptionType);
			}
		}

		return value;
	}

	/// <summary>
	/// Checks if an object is of the an acceptable type.  Performs additional checks to allow for Nullable objects to be
	/// equal to their non-nullable underlying type.  Allows strings to pass.
	/// 
	/// 
	/// That is:
	/// typeof(bool) == typeof(bool?) is considered true.
	/// </summary>
	/// <param name="value">Object to check.</param>
	/// <param name="type">Type to check against.</param>
	/// <returns>True of the object is considered of type input type, false otherwise.</returns>
	private bool IsObjectOfType(object value, Type type)
	{
		Type valueType = value.GetType();

		// Strings always pass.
		if (valueType == typeof(string))
		{
			return true;
		}

		if (type.IsGenericType)
		{
			// Check if we have a generic nullable.
			if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				// Test to consider nullables equal to the underlying type.
				// typeof(bool) == typeof(bool?) will be true for our purposes.
				return valueType == Nullable.GetUnderlyingType(type);
			}
		}

		return valueType == GetBaseType(_optionType);
	}

	private object ConvertValueTypeForSetOperation(object value)
	{
		Debug.Assert(IsObjectOfType(value, _optionType));

		ArgumentNullException.ThrowIfNull(value);

		Type? type = GetBaseType(_optionType);
		Debug.Assert(type != null);
		if (value.GetType() == typeof(string))
		{
			string stringValue = (string)value;
			if (type.Equals(typeof(string)))
			{
				return value;
			}
			else if (type.IsEnum)
			{

				if (!_enumerationValues.Contains(stringValue))
				{
					throw new InvalidEnumerationValueException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.InvalidEnumerationValue0, stringValue));
				}
				return Enum.Parse(type, stringValue, true);
			}
			else if (type.Equals(typeof(bool)) || type.Equals(typeof(bool?)))
			{
				return bool.Parse(stringValue);
			}
			else if (type.Equals(typeof(char)) || type.Equals(typeof(char?)))
			{
				if (stringValue.Length > 1)
				{
					throw new InvalidOptionValueException("Character options must be a single character.");
				}
				return char.Parse(stringValue);
			}
			else // We have a numerical type.
			{
				// For nullable types, we don't want to parse by the nullable type, we want to parse by the underlying type.
				Type? parseType = type;
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					parseType = Nullable.GetUnderlyingType(type);
					Debug.Assert(parseType != null);
				}
				object? returnValue = parseType.InvokeMember("Parse", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, [value, _numberFormatInfo], CultureInfo.CurrentUICulture);
				Debug.Assert(returnValue != null);
				return returnValue;
			}
		}
		else
		{
			return value;
		}
	}

	#endregion
}