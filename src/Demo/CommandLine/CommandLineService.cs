using DigitalProduction.CommandLine;

namespace DigitalProduction.Demo;

[CommandLineManager(ApplicationName = "Test 1", Copyright = "Copyright (c) Lance A. Endres")]
public class CommandLineService : ICommandLine
{
	[CommandLineOption(Name = "filename", Description = "Specifies the input file.")]
	public string? FileName { get; set; } = "";

	[CommandLineOption(Name = "run", BoolFunction = BoolFunction.TrueIfPresent, Description = "If true, the application will automatically start running.")]
	public bool? Run { get; set; } = false;

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

		// Parse the command line arguments.  The parser will call Properties for each command line argument it finds with a matching
		// attribute.
		parser.Parse();
	}
}