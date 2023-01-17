namespace MauiTest;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {

        var window = base.CreateWindow(activationState);
        window.Height = 100;
        window.MinimumHeight = 100;
        window.MaximumHeight = 100;
        return window;
    }
}