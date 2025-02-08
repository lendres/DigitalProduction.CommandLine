namespace DigitalProduction.Demo;

public interface ICommandLine
{
	public string? FileName { get; set; }

	public string? OutputFile { get; set; }

	public bool? Run { get; set; }

	public bool? Exit { get; set; }

	public string Header { get; }

	public string Help { get; }

	public string Errors { get; }

	/// <summary>
	/// Parse the command line arguments.
	/// </summary>
	public void ParseCommandLine();
}