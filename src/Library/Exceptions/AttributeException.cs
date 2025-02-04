using System;
using System.Globalization;
using System.Reflection;

namespace DigitalProduction.CommandLine;

// Indicates programming error
/// <summary>
/// The exception is thrown when the values of the attributes <see cref="CommandLineManagerAttribute"/>, <see cref="CommandLineOptionAttribute"/>
/// and <see cref="CommandLineOptionGroupAttribute"/> are set incorrectly or the attributes are used in an
/// erroneous way.
/// </summary>
/// <remarks>This exception indicates a programming error rather than a user error, and should never be thrown in 
/// a finished program.</remarks>
[Serializable]
public class AttributeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeException"/> class.
    /// </summary>
    public AttributeException() :
		base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeException"/> class.
    /// </summary>
    /// <param name="attributeType">Type of the attribute on which the error is present.</param>
    /// <param name="objectType">Type of the object implementing the attribute on which the error occured.</param>
    /// <param name="message">The error message.</param>
    public AttributeException(Type attributeType, Type objectType, string message) :
		this(String.Format(CultureInfo.CurrentUICulture, "In attribute {0} defined on {1}; {2}", attributeType.Name, objectType.FullName, message))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeException"/> class.
    /// </summary>
    /// <param name="attributeType">Type of the attribute on which the error is present.</param>
    /// <param name="member">The member assigned the attribute with the error.</param>
    /// <param name="message">The error message.</param>
    public AttributeException(Type attributeType, MemberInfo member, string message) :
		this(String.Format(CultureInfo.CurrentUICulture, "In attribute {0} defined on member \"{1}\" of {2}; {3}", attributeType.Name, member.Name, member.DeclaringType?.FullName, message))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public AttributeException(string message) :
		base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AttributeException(string message, Exception innerException) :
		base(message, innerException)
    {
    }
}