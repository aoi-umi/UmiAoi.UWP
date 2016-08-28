using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.ForegroundColor = Colors.White;
                statusBar.BackgroundColor = Colors.Black;
                statusBar.BackgroundOpacity = 1;
            }
        }

        ApplicationView view = ApplicationView.GetForCurrentView();
        private void EnterFullScreen_Click(object sender, RoutedEventArgs e)
        {
            bool isInFullScreenMode = view.IsFullScreenMode;
            switch (cb.SelectedIndex)
            {
                case 0:
                    view.TryEnterFullScreenMode();
                    break;
                case 1:
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
                    break;
                case 2:
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
                    break;
                case 3:
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
                    break;
                case 4:
                    // 在全屏状态下，是否显示系统 UI，比如标题栏和任务栏
                    view.FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Minimal;
                    view.ShowStandardSystemOverlays();
                    break;
                case 5:
                    // 在全屏状态下，是否显示系统 UI，比如标题栏和任务栏
                    view.FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Standard;
                    view.ShowStandardSystemOverlays();
                    break;
            }
        }

        private void ExitFullScreen_Click(object sender, RoutedEventArgs e)
        {
            view.ExitFullScreenMode();
        }
    }
}
