using Microsoft.Maui.ApplicationModel;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif
namespace MauiTest;

public partial class MainPage : ContentPage
{


    [DllImport("user32.dll")]
    static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref RECT pvParam, uint fWinIni);
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    [Flags]
    enum SetWindowPosFlags : uint
    {
        SWP_NOSIZE = 0x0001,
        SWP_NOZORDER = 0x0004,
        // Other flags can be added here as needed
    }


    public MainPage()
    {
        InitializeComponent();
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
#if WINDOWS

            var nativeWindow = handler.PlatformView;
            nativeWindow.Activate();
            nativeWindow.ExtendsContentIntoTitleBar = false;
            IntPtr windowHandler = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandler);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var presenter = appWindow.Presenter as OverlappedPresenter;
            if (presenter != null)
            {
                presenter.IsAlwaysOnTop = true;
                presenter.SetBorderAndTitleBar(false, false);
            }
        
                appWindow.Move(new Windows.Graphics.PointInt32(0, 0));
                appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
#endif
        });
        setWorkArea();
    }

    private void setWorkArea()
    {
        IntPtr desktopWindow = GetDesktopWindow();
        var setPos = SetWindowPos(desktopWindow, IntPtr.Zero, 0, 100, 0, 0, (uint)(SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOZORDER));//in windows 11 v21H2 return true
        int error1 = Marshal.GetLastWin32Error();
        Console.WriteLine(error1);
        RECT rect = new RECT();
        rect.Left = 0;
        rect.Top = 100;
        rect.Right = 1920;
        rect.Bottom = 980;
        var sysparam = SystemParametersInfo(0x002F, 0, ref rect, 0x0002);//return true
        error1 = Marshal.GetLastWin32Error();
        Console.WriteLine(error1);
    }

    private void OnExitClicked(object sender, EventArgs e)
    {
        IntPtr desktopWindow = GetDesktopWindow();
        SetWindowPos(desktopWindow, IntPtr.Zero, 0, 0, 0, 0, (uint)(SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOZORDER));//resize the working area back to normal.
        RECT rect = new RECT();
        rect.Left = 0;
        rect.Top = 0;
        rect.Right = 1920;
        rect.Bottom = 1080;
        SystemParametersInfo(0x002F, 0, ref rect, 0x0002);

    }
}

