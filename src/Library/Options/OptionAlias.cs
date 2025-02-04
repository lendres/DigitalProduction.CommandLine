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