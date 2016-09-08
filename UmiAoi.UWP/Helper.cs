using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UmiAoi.UWP
{
    public static class Helper
    {
        public static void BindingHelper(BindingModel bindingModel)
        {
            if (bindingModel.BindingElement == null) throw new NullReferenceException(nameof(bindingModel.BindingElement));
            var binding = new Binding();
            if (bindingModel.Source != null) binding.Source = bindingModel.Source;
            if (bindingModel.Path != null) binding.Path = new PropertyPath(bindingModel.Path);
            if (bindingModel.ElementName != null) binding.ElementName = bindingModel.ElementName;
            binding.Mode = bindingModel.BindingMode;
            bindingModel.BindingElement.SetBinding(bindingModel.Property, binding);
        }

        public static async void ShowMessage(string message, string title = null)
        {
            await new MessageDialog(message, title).ShowAsync();
        }

        public static DeviceFamily GetDeviceFamily()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Mobile":
                    return DeviceFamily.Mobile;
                case "Windows.Desktop":
                default:
                    return DeviceFamily.Desktop;
            }
        }
    }

    public enum DeviceFamily
    {
        Desktop,
        Mobile
    }

    public class BindingModel
    {
        public FrameworkElement BindingElement { get; set; }
        public DependencyProperty Property { get; set; }
        public Object Source { get; set; }
        public string Path { get; set; }
        public BindingMode BindingMode { get; set; }
        public String ElementName { get; set; }
    }
}
