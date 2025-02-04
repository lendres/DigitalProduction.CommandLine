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
namespace DigitalProduction.CommandLine;

internal class OptionAlias(string aliasName, Option definingOption) : IOption
{
	#region Fields

	private readonly Option		mDefiningOption		= definingOption;

	#endregion

	#region Properties

	public object Value { set => mDefiningOption.Value = value; }

	public bool RequiresValue { get => mDefiningOption.RequiresValue; }

	public bool RequireExplicitAssignment
	{
		get => mDefiningOption.RequireExplicitAssignment;
		set => mDefiningOption.RequireExplicitAssignment = value;
	}

	public C5.ICollection<Option> ProhibitedBy { get => mDefiningOption.ProhibitedBy; }

	public OptionGroup? Group { get => mDefiningOption.Group; }

	public string Name { get; } = aliasName;

	public BoolFunction BoolFunction { get => mDefiningOption.BoolFunction; }

	public int MaxOccurs { get => mDefiningOption.MaxOccurs; }

	public int MinOccurs { get => mDefiningOption.MinOccurs; }

	public string Description { get => mDefiningOption.Description; }

	public int SetCount { get => mDefiningOption.SetCount; set => mDefiningOption.SetCount = value; }

	public bool AcceptsValue { get => mDefiningOption.AcceptsValue; }

	public bool HasDefaultValue { get => mDefiningOption.HasDefaultValue; }

	public bool IsBooleanType { get => mDefiningOption.IsBooleanType; }

	public bool IsAlias { get => true; }

	public Option DefiningOption { get => mDefiningOption; }

	public object? MinValue { get => mDefiningOption.MinValue; }

	public object? MaxValue { get => mDefiningOption.MaxValue; }

	#endregion

	#region IOption Properties

	public bool IsIntegralType { get => mDefiningOption.IsIntegralType; }

	public bool IsFloatingPointType { get => mDefiningOption.IsFloatingPointType; }

	public bool IsDecimalType { get => mDefiningOption.IsDecimalType; }

	public bool IsNumericalType { get => mDefiningOption.IsNumericalType; }

	#endregion

	#region Methods

	public void SetDefaultValue()
	{
		mDefiningOption.SetDefaultValue();
	}

	#endregion
}