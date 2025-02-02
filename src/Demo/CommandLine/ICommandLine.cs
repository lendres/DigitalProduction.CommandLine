namespace DigitalProduction.Demo;

public interface ICommandLine
{
	public string? FileName { get; set; }

	public bool? Run { get; set; }

	/// <summary>
	/// Parse the command line arguments.
	/// </summary>
	public void ParseCommandLine();
}