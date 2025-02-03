using DigitalProduction.CommandLine;

namespace UnitTests;

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


[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class ByteManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public byte Value { get; set; } = 0x00;
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableByteManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public byte? Value { get; set; } = null;
}

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

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class IntManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public int Value { get; set; } = ' ';
}

[CommandLineManager(ApplicationName = "Nullable Test", Copyright = "Copyright (c) Lance A. Endres")]
class NullableIntManager
{
	[CommandLineOption(Name = "value", Description = "Specifies the test value.")]
	public int? Value { get; set; } = null;
}

//sbyte
//decimal
//double
//float
//uint
//long
//ulong
//short
//ushort