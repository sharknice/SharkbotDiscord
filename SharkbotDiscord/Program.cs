using SharkbotDiscord.Startup;

// TODO: connect to the image generation api, draw users based on their known data, also draw based on saying "draw X", "show me X", etc.
// TODO: get reactions working correctly
class Program
{
    public static Task Main(string[] args)
        => Startup.RunAsync(args);
}
