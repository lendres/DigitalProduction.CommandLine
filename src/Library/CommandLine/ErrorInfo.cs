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
 */
using System.Text;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Represents an error generated by the <see cref="CommandLineParser"/>
/// </summary>
public class ErrorInfo
{
	#region Private Fields

	private string?				mFileName;
	private ParseErrorCodes		mErrorCode;
	private int?				mLine;
	private string				mMessage;
	private string?				mOptionName;

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="ErrorInfo"/> class.
	/// </summary>
	/// <param name="errorCode">The error code.</param>
	/// <param name="errorMessage">The error message.</param>
	/// <param name="optionName">Name of the option.</param>
	/// <param name="fileName">Name of the file, or null if no file name was available.</param>
	/// <param name="line">The line number on which the error occured, or null if no line information was available.</param>
	internal ErrorInfo(ParseErrorCodes errorCode, string errorMessage, string? optionName, string? fileName, int? line)
	{
		mErrorCode = errorCode;
		mMessage = errorMessage;
		mOptionName = optionName;
		mLine = line;
		mFileName = fileName;
	}

	#endregion

	#region Public Properties

	/// <summary>
	/// Gets or sets the message.
	/// </summary>
	/// <value>The message.</value>
	public string Message { get => mMessage; set => mMessage = value; }

	/// <summary>
	/// Gets or sets the error code.
	/// </summary>
	/// <value>The error code.</value>
	public ParseErrorCodes ErrorCode { get => mErrorCode; set => mErrorCode = value; }

	/// <summary>
	/// Gets a value indicating whether this error originates from a file, meaning that a file name will be available.
	/// </summary>
	/// <value><c>true</c> if this error originates from a file; otherwise, <c>false</c>.</value>
	public bool OriginatesFromFile {get => mFileName != null; }

	/// <summary>
	/// Gets or sets the name of the option causing this error.
	/// </summary>
	/// <value>The name of the option causing this error, or null if no option name is available.</value>
	public string? OptionName { get => mOptionName; set => mOptionName = value; }

	/// <summary>
	/// Gets a value indicating whether this instance has option name set.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if this instance has option name set; otherwise, <c>false</c>.
	/// </value>
	public bool HasOptionName { get => mOptionName != null; }

	/// <summary>
	/// Gets or sets the line on which the error occured.
	/// </summary>
	/// <value>The line on which the error occured, or null if no such information is available.</value>
	public int? Line { get => mLine; set => mLine = value; }

	/// <summary>
	/// Gets or sets the name of the file.
	/// </summary>
	/// <value>The name of the file in which the error occured, or null if no file name is available.</value>
	public string? FileName { get => mFileName; set => mFileName = value; }

	#endregion

	#region Public Methods

	/// <summary>
	/// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
	/// </summary>
	/// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
	/// <returns>
	/// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
	/// </returns>
	public override bool Equals(object? obj)
	{
		if (obj is not ErrorInfo error)
		{
			return false;
		}

		return mErrorCode.Equals(error.mErrorCode) && mMessage.Equals(error.mMessage) && (mOptionName == null ? error.mOptionName == null : mOptionName.Equals(error.mOptionName));
	}

	/// <summary>
	/// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
	/// </summary>
	/// <returns>
	/// A hash code for the current <see cref="T:System.Object"></see>.
	/// </returns>
	public override int GetHashCode()
	{
		return mMessage.GetHashCode() ^ mErrorCode.GetHashCode() ^ (mOptionName == null ? 0 : mOptionName.GetHashCode());
	}

	/// <summary>
	/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
	/// </summary>
	/// <returns>
	/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
	/// </returns>
	public override string ToString()
	{
		StringBuilder str = new();
		str.Append("Error");

		if (mFileName != null)
		{
			str.Append(" in file \"");
			str.Append(mFileName);
			str.Append('"');
		}

		if (mLine.HasValue)
		{
			str.Append(" on line ");
			str.Append(mLine.Value);
		}
		str.Append(": ");
		str.Append(mMessage);
		return str.ToString();
	}

	#endregion
}