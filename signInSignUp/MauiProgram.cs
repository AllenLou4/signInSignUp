using CommunityToolkit.Maui; // Add this at the top
using Microsoft.Extensions.Logging;

namespace signInSignUp;

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
                fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                fonts.AddFont("Inter_18pt-Bold.ttf", "Inter18Bold"); // OpenSansSemibold
                fonts.AddFont("Inter_24pt-ExtraBold.ttf", "Inter24ExtraBold"); // MontserratSemiBold
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
