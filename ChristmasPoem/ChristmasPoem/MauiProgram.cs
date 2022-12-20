using ChristmasPoem.Services;
using ChristmasPoem.Services.Implementations;
using ChristmasPoem.ViewModels;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace ChristmasPoem;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.Services.AddTransient<MainPage>()
				.AddTransient<MainPageViewModel>()
				.AddTransient<IAIService, AIService>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
