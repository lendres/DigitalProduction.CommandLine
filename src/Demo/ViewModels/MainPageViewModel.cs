using CommunityToolkit.Mvvm.ComponentModel;

namespace DigitalProduction.Demo;

public partial class MainPageViewModel : ObservableObject
{
	#region Fields

	private readonly ICommandLine _commandLineArguments;

	#endregion

	#region Construction

	public MainPageViewModel(ICommandLine commandLineArguments)
    {
		_commandLineArguments = commandLineArguments;

		InitializeValues();
	}

	private void InitializeValues()
	{
		try
		{
			_commandLineArguments.ParseCommandLine();
		}
		catch (Exception exception)
		{
			ErrorOccured = true;
			ErrorMessage = exception.Message;
		}

		FileName		= _commandLineArguments.FileName		?? string.Empty;
		Run				= _commandLineArguments.Run				?? false;
		Header			= _commandLineArguments.Header			?? string.Empty;
		Help			= _commandLineArguments.Help			?? string.Empty;
		Errors			= _commandLineArguments.Errors			?? string.Empty;
	}

	#endregion

	#region Properties

	[ObservableProperty]
	public partial string			FileName { get; set; }					= string.Empty;

	[ObservableProperty]
	public partial bool				Run { get; set; }						= false;

	[ObservableProperty]
	public partial bool				ErrorOccured { get; set; }				= false;

	[ObservableProperty]
	public partial string			ErrorMessage { get; set; }				= string.Empty;

	[ObservableProperty]
	public partial string			Header { get; set; }					= string.Empty;

	[ObservableProperty]
	public partial string			Help { get; set; }						= string.Empty;

	[ObservableProperty]
	public partial string			Errors{ get; set; }						= string.Empty;

	#endregion

} // End class.