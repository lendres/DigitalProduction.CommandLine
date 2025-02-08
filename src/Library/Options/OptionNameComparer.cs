using System;
using System.Globalization;
using SCG = System.Collections.Generic;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Comparer implementing compareres for collections keyed by <see cref="OptionGroup"/> or 
/// strings. The comparisons will be case-sensitive or case-insensitive depending on the 
/// flag passed to the constructor.
/// </summary>
internal class OptionNameComparer(bool isCaseSensitive) : SCG.IComparer<OptionGroup>, SCG.IEqualityComparer<OptionGroup>, SCG.IComparer<string>, SCG.IEqualityComparer<string>
{
	#region Properties

	public bool IsCaseSensitive { get; } = isCaseSensitive;

	#endregion

	#region IComparer<OptionGroup> Members

	public int Compare(OptionGroup? x, OptionGroup? y)
	{
		ArgumentNullException.ThrowIfNull(x);

		ArgumentNullException.ThrowIfNull(y);

		return String.Compare(x.Id, y.Id, !IsCaseSensitive, CultureInfo.CurrentUICulture);
	}

	#endregion

	#region IEqualityComparer<OptionGroup> Members

	public bool Equals(OptionGroup? x, OptionGroup? y)
	{
		return Compare(x, y) == 0;
	}

	public int GetHashCode(OptionGroup obj)
	{
		ArgumentNullException.ThrowIfNull(obj);

		return IsCaseSensitive ? obj.Id.GetHashCode() : obj.Id.ToUpper(CultureInfo.CurrentUICulture).GetHashCode();
	}

	#endregion

	#region IComparer<string> Members

	public int Compare(string? x, string? y)
	{
		return String.Compare(x, y, !IsCaseSensitive, CultureInfo.CurrentUICulture);
	}

	#endregion

	#region IEqualityComparer<string> Members

	public bool Equals(string? x, string? y)
	{
		return Compare(x, y) == 0;
	}

	public int GetHashCode(string obj)
	{
		ArgumentNullException.ThrowIfNull(obj);

		return IsCaseSensitive ? obj.GetHashCode() : obj.ToUpper(CultureInfo.CurrentUICulture).GetHashCode();
	}

	#endregion
}