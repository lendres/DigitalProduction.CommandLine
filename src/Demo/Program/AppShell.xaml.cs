namespace DigitalProduction.Demo;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(SecondPage), typeof(SecondPage));
	}
}
