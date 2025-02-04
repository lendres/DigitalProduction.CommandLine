using C5;

namespace DigitalProduction.CommandLine;

internal interface IOption
{
	#region Public Properties

	object Value { set; }

	bool RequiresValue { get; }

	bool RequireExplicitAssignment { get; set; }

	ICollection<Option> ProhibitedBy { get; }

	OptionGroup? Group { get; }

	string Name { get; }

	BoolFunction BoolFunction { get; }

	int MaxOccurs { get; }

	int MinOccurs { get; }

	string Description { get; }
	
	int SetCount { get; set; }

	bool AcceptsValue { get; }

	bool HasDefaultValue { get; }

	bool IsBooleanType { get; }

	bool IsAlias { get; }

	Option DefiningOption { get; }

	bool IsIntegralType { get; }

	bool IsFloatingPointType { get; }

	bool IsDecimalType { get; }

	bool IsNumericalType { get; }

	object? MinValue { get; }

	object? MaxValue { get; }

	#endregion

	#region Public methods

	void SetDefaultValue();

	#endregion
}