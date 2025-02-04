using DigitalProduction.CommandLine;

namespace UnitTests;

/// <summary>
/// String.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class StringManager
{
	[CommandLineOption(Name = "filename", Description = "Specifies the input file.")]
	public string FileName { get; set; } = string.Empty;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableStringManager
{
	[CommandLineOption(Name = "filename", Description = "Specifies the input file.")]
	public string? FileName { get; set; } = null;
}

/// <summary>
/// Boolean.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class BoolManager
{
	[CommandLineOption(Name = "run", BoolFunction = BoolFunction.TrueIfPresent, Description = "If true, the application will automatically start running.")]
	public bool Run { get; set; }
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableBoolManager
{
	[CommandLineOption(Name = "run", BoolFunction = BoolFunction.TrueIfPresent, Description = "If true, the application will automatically start running.")]
	public bool? Run { get; set; } = null;
}

/// <summary>
/// Byte.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class ByteManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public byte Value { get; set; } = 0;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableByteManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public byte? Value { get; set; } = null;
}

/// <summary>
/// Short byte.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class ShortByteManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public sbyte Value { get; set; } = 0;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableShortByteManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public sbyte? Value { get; set; } = null;
}

/// <summary>
/// Character.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class CharManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public char Value { get; set; } = ' ';
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableCharManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public char? Value { get; set; } = null;
}

/// <summary>
/// Integer.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class IntManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public int Value { get; set; } = 0;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableIntManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public int? Value { get; set; } = null;
}

/// <summary>
/// Unsigned integer.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class UIntManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public uint Value { get; set; } = 0u;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableUIntManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public uint? Value { get; set; } = null;
}

/// <summary>
/// Short integer.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class ShortManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public short Value { get; set; } = 0;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableShortManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public short? Value { get; set; } = null;
}

/// <summary>
/// Unsigned short integer.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class UShortManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public ushort Value { get; set; } = 0;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableUShortManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public ushort? Value { get; set; } = null;
}

/// <summary>
/// Long integer.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class LongManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public long Value { get; set; } = 0L;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableLongManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public long? Value { get; set; } = null;
}

/// <summary>
/// Unsigned long integer.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class ULongManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public ulong Value { get; set; } = 0UL;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableULongManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public ulong? Value { get; set; } = null;
}

/// <summary>
/// Double.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class DoubleManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public double Value { get; set; } = 0.0;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableDoubleManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public double? Value { get; set; } = null;
}

/// <summary>
/// Float.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class FloatManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public float Value { get; set; } = 0f;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableFloatManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public float? Value { get; set; } = null;
}

/// <summary>
/// Decimal.
/// </summary>
[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class DecimalManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public decimal Value { get; set; } = 0m;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableDecimalManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public decimal? Value { get; set; } = null;
}