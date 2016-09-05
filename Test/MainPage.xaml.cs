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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
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
            InitStoryBoard();
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

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            SetRect(sv.VerticalOffset + sv.ActualHeight, true);
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            SetRect(sv.VerticalOffset - sv.ActualHeight, false);
        }

        private readonly Storyboard _storyboard = new Storyboard();
        private readonly DoubleAnimation _XAnim = new DoubleAnimation();
        private async void SetRect(double offset, bool next)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(sv);
            rect.Source = bitmap;
            sv.ScrollToVerticalOffset(offset);
            _XAnim.From = 0;
            _XAnim.To = next ? -sv.ActualWidth : sv.ActualWidth;
            _storyboard.Begin();
        }

        private async void InitStoryBoard()
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(sv);
            rect.Source = bitmap;
            var element = rectGrid;
            var ct = new CompositeTransform();
            element.RenderTransform = ct;
            _XAnim.EasingFunction = new ExponentialEase { Exponent = 4 };
            _XAnim.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            Storyboard.SetTarget(_XAnim, element.RenderTransform);
            Storyboard.SetTargetProperty(_XAnim, nameof(CompositeTransform.TranslateX));
            _storyboard.Children.Add(_XAnim);
        }
    }
}
