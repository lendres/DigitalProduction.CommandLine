namespace UnlockingFlyout;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnNavigateForward(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync(nameof(SecondPage), true, new Dictionary<string, object> { });
	}
}