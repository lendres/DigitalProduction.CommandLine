namespace DigitalProduction.Demo;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	private void OnNavigateForward(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync(nameof(SecondPage), true, new Dictionary<string, object> { });
	}
}