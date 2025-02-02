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

	#endregion

} // End class.