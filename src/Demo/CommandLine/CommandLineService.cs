using CommunityToolkit.Mvvm.ComponentModel;
using DigitalProduction.CommandLine;

namespace DigitalProduction.Demo;

[CommandLineManager(ApplicationName = "Test 1", Copyright = "Copyright (c) Lance A. Endres")]
[CommandLineOptionGroup("commands", Name = "Commands", Require = OptionGroupRequirement.AtMostOne)]
public partial class CommandLineService : ICommandLine
{
	[CommandLineOption(
		Name			= "filename",
		Description		= "Specifies the input file."
	)]
	public string? FileName { get; set; } = null;

	[CommandLineOption(
		Name			= "outputfile",
		Aliases			= "o",
		Description		= "Where the output is written to."
	)]
	public string? OutputFile { get; set; } = null;


	[CommandLineOption(
		Name			= "run",
		Aliases			= "r",
		BoolFunction	= BoolFunction.TrueIfPresent,
		GroupId			= "commands",
		Description		= "If true, the application will start running.  Cannot be combined with the exit command."
	)]
	public bool? Run { get; set; } = false;

	[CommandLineOption(
		Name			= "exit",
		Aliases			= "e",
		BoolFunction	= BoolFunction.TrueIfPresent,
		GroupId			= "commands",
		Description		= "Specifies that the software should close after running.  Cannot be combined with the run command."
	)]
	public bool? Exit { get; set; } = null;

	public string Header { get; private set; } = string.Empty;

	public string Help { get; private set; } = string.Empty;
	
	public string Errors { get; private set; } = string.Empty;

	/// <summary>
	/// Parse the command line arguments.
	/// </summary>
	public void ParseCommandLine()
	{
		// Create the parser to handle the command line arguments.  See the properties to determine valid command line arguments.
		CommandLineParser parser = new(this);

		// We need to replace the existing quotation information because if we don't it replaces network address such as "\\computer" 
		// with "\computer" because it escapes the escape character.
		QuotationInfo quotationInfo = new('\"');
		quotationInfo.AddEscapeCode('\"', '\"');
		parser.AddQuotation(quotationInfo);

		// Parse the command line arguments.  The parser will call Properties for each command line argument it finds with a matching attribute.
		parser.Parse();

		Header	= parser.UsageInfo.GetHeaderAsString();
		Help	= parser.UsageInfo.GetOptionsAsString();
		Errors	= parser.HasErrors ? parser.UsageInfo.GetErrorsAsString() : "No parsing errors.";
	}
}