using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UmiAoi.UWP;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
            var testString = ""; 
            for(var i = 0; i < 5;i++)
            {
                testString += "abcdefghijklmnopqrstuvtxyz0123456789";
            }
            FontMeasureInput.Text = testString + testString.ToUpper();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {            
            //menu.Items.Remove(sender);            
            //menu.Items.Insert(new Random().Next(0, menu.Items.Count), sender);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            menu.Items.Insert(new Random().Next(0, menu.Items.Count), new AppBarButton()
            {
                Icon = new SymbolIcon() { Symbol = Symbol.Add },
                Background = new SolidColorBrush(Colors.BlueViolet)
            });
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if(menu.Items.Count != 0)
            menu.Items.RemoveAt(new Random().Next(0, menu.Items.Count));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Helper.ShowToastNotification(null, "提示", Notification.Reminder);
        }

        private void FontMeasureInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            FontMeasureInput.TextChanged -= FontMeasureInput_TextChanged;

            double maxHeight;
            if (!Double.TryParse(MaxHeightBox.Text, out maxHeight)) maxHeight = 60;
            var req = new MeasureFontRequest()
            {
                Input = FontMeasureInput.Text,
                AvailableSize = new Size(b.ActualWidth, maxHeight),
                FontFamily = FontMeasureInput.FontFamily,
                FontSize = FontMeasureInput.FontSize,
            };
            var index1 = Helper.GetStringAvailableIndex(req);

            FontMeasureOutput1.Text = FontMeasureInput.Text.Substring(0, index1);
            if (index1 < FontMeasureInput.Text.Length)
            {
                req.Input = req.Input.Substring(index1);
                var index2 = Helper.GetStringAvailableIndex(req);
                FontMeasureOutput2.Text = FontMeasureInput.Text.Substring(index1, index2);
            }
            else FontMeasureOutput2.Text = "";
            FontMeasureInput.TextChanged += FontMeasureInput_TextChanged;
        }
    }
}
