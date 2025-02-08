namespace DigitalProduction.CommandLine;

internal class OptionAlias(string aliasName, Option definingOption) : IOption
{
	#region Fields

	private readonly Option		_definingOption		= definingOption;

	#endregion

	#region Properties

	public object Value { set => _definingOption.Value = value; }

	public bool RequiresValue { get => _definingOption.RequiresValue; }

	public bool RequireExplicitAssignment
	{
		get => _definingOption.RequireExplicitAssignment;
		set => _definingOption.RequireExplicitAssignment = value;
	}

	public C5.ICollection<Option> ProhibitedBy { get => _definingOption.ProhibitedBy; }

	public OptionGroup? Group { get => _definingOption.Group; }

	public string Name { get; } = aliasName;

	public BoolFunction BoolFunction { get => _definingOption.BoolFunction; }

	public int MaxOccurs { get => _definingOption.MaxOccurs; }

	public int MinOccurs { get => _definingOption.MinOccurs; }

	public string Description { get => _definingOption.Description; }

	public int SetCount { get => _definingOption.SetCount; set => _definingOption.SetCount = value; }

	public bool AcceptsValue { get => _definingOption.AcceptsValue; }

	public bool HasDefaultValue { get => _definingOption.HasDefaultValue; }

	public bool IsBooleanType { get => _definingOption.IsBooleanType; }

	public bool IsAlias { get => true; }

	public Option DefiningOption { get => _definingOption; }

	public object? MinValue { get => _definingOption.MinValue; }

	public object? MaxValue { get => _definingOption.MaxValue; }

	#endregion

	#region IOption Properties

	public bool IsIntegralType { get => _definingOption.IsIntegralType; }

	public bool IsFloatingPointType { get => _definingOption.IsFloatingPointType; }

	public bool IsDecimalType { get => _definingOption.IsDecimalType; }

	public bool IsNumericalType { get => _definingOption.IsNumericalType; }

	#endregion

	#region Methods

	public void SetDefaultValue()
	{
		_definingOption.SetDefaultValue();
	}

	#endregion
}