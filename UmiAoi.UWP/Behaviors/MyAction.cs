using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UmiAoi.UWP.Behaviors
{
    public class MyAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            Debug.WriteLine("Execute");
            return true;
        }
    }
}
