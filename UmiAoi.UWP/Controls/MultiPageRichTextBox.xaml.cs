using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Documents;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UmiAoi.UWP.Controls
{
    public sealed partial class MultiPageRichTextBox : UserControl
    {
        public MultiPageRichTextBox()
        {
            this.InitializeComponent();
            AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(PointerReleased), true);
            InitStoryBoard();
            var p1 = new Paragraph();
            p1.Inlines.Add(new Run() { Text = "aaaaaaaaaaaaaaaaaaaaa1" });
            var p2 = new Paragraph();
            p2.Inlines.Add(new Run() { Text = "aaaaaaaaaaaaaaaaaaaaa2" });
            var p3 = new Paragraph();
            p3.Inlines.Add(new Run() { Text = "aaaaaaaaaaaaaaaaaaaaa3" });
            rtb.Blocks.Add(p1);
            rtb.Blocks.Add(p2);
            rtb.Blocks.Add(p3);
        }

        private readonly Storyboard storyboard = new Storyboard();
        private readonly DoubleAnimation xAnim = new DoubleAnimation();
        private void InitStoryBoard()
        {
            storyboard.Completed += Storyboard_Completed;
            var element = img;
            var ct = new CompositeTransform();
            element.RenderTransform = ct;
            xAnim.EasingFunction = new ExponentialEase { Exponent = 4 };
            xAnim.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            Storyboard.SetTarget(xAnim, element.RenderTransform);
            Storyboard.SetTargetProperty(xAnim, nameof(CompositeTransform.TranslateX));
            storyboard.Children.Add(xAnim);
        }

        private void Storyboard_Completed(object sender, object e)
        {
            img.Visibility = Visibility.Collapsed;
        }

        private void PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((UIElement)sender);
            var pos = point.Position;
            if (pos.X > ActualWidth / 2)
            {
                Animation(sv.VerticalOffset + sv.ActualHeight, true);
            }
            else
            {
                Animation(sv.VerticalOffset - sv.ActualHeight, false);
            }
        }

        private async void Animation(double offset, bool toLeft)
        {
            img.Visibility = Visibility.Visible;
            RenderTargetBitmap bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(sv);
            img.Source = bitmap;
            sv.ScrollToVerticalOffset(offset);
            xAnim.From = 0;
            xAnim.To = toLeft ? -ActualWidth : ActualWidth;
            storyboard.Stop();
            storyboard.Begin();
        }

        ~MultiPageRichTextBox()
        {
            storyboard.Completed -= Storyboard_Completed;
            RemoveHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(PointerReleased));
        }
    }
}
