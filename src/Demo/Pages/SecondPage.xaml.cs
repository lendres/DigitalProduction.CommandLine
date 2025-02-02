namespace UnlockingFlyout;

public partial class SecondPage : ContentPage
{
	public SecondPage()
	{
		InitializeComponent();
	}

	void OnNavigateBack(object sender, EventArgs eventArgs)
	{
		Shell.Current.GoToAsync("../");
	}
}