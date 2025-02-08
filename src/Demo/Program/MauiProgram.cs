using Microsoft.Extensions.Logging;

namespace DigitalProduction.Demo;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("IBMPlexMono-Bold.ttf", "IBMPlexMono-Bold");
				fonts.AddFont("IBMPlexMono-Regular.ttf", "IBMPlexMono-Regular");
			});

		builder.Services.AddSingleton<ICommandLine, CommandLineService>();
		builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<MainPage>();

		#if DEBUG
			builder.Logging.AddDebug();
		#endif

		return builder.Build();
	}
}