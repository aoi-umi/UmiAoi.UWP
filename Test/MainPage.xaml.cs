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
    }
}
