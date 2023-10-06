using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System;

using ProductivWinUINav.Activation;
using ProductivWinUINav.Contracts.Services;
using ProductivWinUINav.Core.Contracts.Services;
using ProductivWinUINav.Core.Services;
using ProductivWinUINav.Helpers;
using ProductivWinUINav.Models;
using ProductivWinUINav.Notifications;
using ProductivWinUINav.Services;
using ProductivWinUINav.ViewModels;
using ProductivWinUINav.Views;
using WinUIEx.Messaging;

namespace ProductivWinUINav;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar { get; set; }

    private GlobalHooks _GlobalHooks;

    #region wndProc prevent minimize

    private const int WM_SYSCOMMAND = 0x0112;
    private const int SC_MINIMIZE = 0xf020;



    protected override void WndProc(ref Message m)
    {
        // Check to see if we've received any Windows messages telling us about our hooks
        if (m.Msg == WM_SYSCOMMAND)
        {
            if (m.WParam.ToInt32() == SC_MINIMIZE)
            {

                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000);
                this.Hide();

                m.Result = IntPtr.Zero;
                return;
            }
        }
        if (_GlobalHooks != null)
            _GlobalHooks.ProcessWindowMessage(ref m);

        base.WndProc(ref m);
    }

    private Win32Interop.WndProc originalWndProc;


    #endregion

    public App()
    {
        InitializeComponent();

        var windowHandle = GeneralHelper.GetWindowHandle();

        _GlobalHooks = new GlobalHooks(windowHandle);

        _GlobalHooks.CBT.Activate += new GlobalHooks.WindowEventHandler(_GlobalHooks_CbtActivate);
        _GlobalHooks.CBT.CreateWindow += new GlobalHooks.WindowEventHandler(_GlobalHooks_CbtCreateWindow);
        _GlobalHooks.CBT.DestroyWindow += new GlobalHooks.WindowEventHandler(_GlobalHooks_CbtDestroyWindow);
        _GlobalHooks.CBT.MinMax += new GlobalHooks.WindowEventHandler(_GlobalHooks_CbtMinMax);
        _GlobalHooks.Shell.WindowActivated += new GlobalHooks.WindowEventHandler(_GlobalHooks_ShellWindowActivated);
        _GlobalHooks.Shell.WindowCreated += new GlobalHooks.WindowEventHandler(_GlobalHooks_ShellWindowCreated);
        _GlobalHooks.Shell.WindowDestroyed += new GlobalHooks.WindowEventHandler(_GlobalHooks_ShellWindowDestroyed);
        _GlobalHooks.Shell.Redraw += new GlobalHooks.WindowEventHandler(_GlobalHooks_ShellRedraw);
        //_GlobalHooks.MouseLL.MouseMove += new MouseEventHandler(MouseLL_MouseMove);

        //_GlobalHooks.MouseLL.Start();

        _GlobalHooks.CBT.Start();
        _GlobalHooks.Shell.Start();


        originalWndProc = (IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam) =>
        {
            // Handle custom window messages here
            switch (msg)
            {
                // Custom message example
                case Constants.WM_CUSTOM_MESSAGE:
                    // Handle custom message
                    // ...
                    return IntPtr.Zero; // Message handled

                default:
                    return Win32Interop.CallWindowProc(originalWndProc, hWnd, msg, wParam, lParam);
            }
        };

        PInvokeMethods.SetWindowLongPtr(windowHandle, PInvokeMethods.GWLP_WNDPROC, originalWndProc);


        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers
            services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

            // Services
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IAppNotificationService, AppNotificationService>();
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<AboutViewModel>();
            services.AddTransient<AboutPage>();
            services.AddTransient<DebugViewModel>();
            services.AddTransient<DebugPage>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<HomePage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        App.GetService<IAppNotificationService>().Initialize();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        await App.GetService<IActivationService>().ActivateAsync(args);
    }
}
