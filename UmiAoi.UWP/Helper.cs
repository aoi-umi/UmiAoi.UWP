using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.System.Profile;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

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

        private static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public static void SettingSet(string key, object value)
        {
            LocalSettings.Values[key] = value;
        }

        public static T SettingGet<T>(string key)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (T)LocalSettings.Values[key];
            }
            return default(T);
        }

        public static async Task<IReadOnlyList<StorageFile>> GetFileList(IList<string> FileTypeFilter)
        {
            if (FileTypeFilter == null || FileTypeFilter.Count == 0) throw new Exception(nameof(FileTypeFilter) + "不能为空");
            var Picker = new FileOpenPicker();
            Picker.ViewMode = PickerViewMode.List;
            foreach (var fileTypeFilter in FileTypeFilter)
            {
                Picker.FileTypeFilter.Add(fileTypeFilter);
            }
            var fileList = await Picker.PickMultipleFilesAsync();
            if (fileList != null)
            {
                foreach(var file in fileList)
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", file);
            }
            return fileList;
        }

        public static async void ShowMessage(string message, string title = null)
        {
            await new MessageDialog(message, title).ShowAsync();
        }

        public static DeviceFamily CurrDeviceFamily
        {
            get
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

        public static DependencyObject GetParent(DependencyObject reference, Type targetType)
        {
            return GetParent(reference, targetType, 1);
        }

        public static DependencyObject GetParent(DependencyObject reference, Type targetType, int level)
        {
            if (level <= 0) return null;
            var parent = VisualTreeHelper.GetParent(reference);
            if (parent == null)
                return parent;
            else if(parent.GetType() == targetType)
            {
                if(level == 1)
                    return parent;
                else
                    return GetParent(parent, targetType, level - 1);
            }
            return GetParent(parent, targetType, level);
        }

        public static void ShowToastNotification(string str)
        {
            ShowToastNotification(null, str, Notification.Reminder);
        }

        public static void ShowToastNotification(string assetsImageFileName, string str, Notification audioName)
        {
            if (string.IsNullOrWhiteSpace(assetsImageFileName)) assetsImageFileName = "StoreLogo.png";
            //create element
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            //provide text
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(str));

            //provide image
            XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
            ((XmlElement)toastImageAttributes[0]).SetAttribute("src", $"ms-appx:///assets/{assetsImageFileName}");
            ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "logo");

            //duration
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "short");

            //audio
            XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", $"ms-winsoundevent:Notification.{audioName.ToString().Replace("_", ".")}");
            toastNode.AppendChild(audio);

            //app launch parameter
            ((XmlElement)toastNode).SetAttribute("launch", "{\"type\":\"toast\",\"param1\":\"12345\",\"param2\":\"67890\"}");
            ShowToastNotification(toastXml);
        }

        public static void ShowToastNotification(XmlDocument toastXml)
        {
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }

    public class BindingModel
    {
        public FrameworkElement BindingElement { get; set; }
        public DependencyProperty Property { get; set; }
        public Object Source { get; set; }
        public string Path { get; set; }
        public BindingMode BindingMode { get; set; } = BindingMode.OneWay;
        public String ElementName { get; set; }
    }

    public enum DeviceFamily
    {
        Desktop,
        Mobile
    }

    public enum Notification
    {
        Default,
        IM,
        Mail,
        Reminder,
        SMS,
        Looping_Alarm,
        Looping_Alarm2,
        Looping_Alarm3,
        Looping_Alarm4,
        Looping_Alarm5,
        Looping_Alarm6,
        Looping_Alarm7,
        Looping_Alarm8,
        Looping_Alarm9,
        Looping_Alarm10,
        Looping_Call,
        Looping_Call2,
        Looping_Call3,
        Looping_Call4,
        Looping_Call5,
        Looping_Call6,
        Looping_Call7,
        Looping_Call8,
        Looping_Call9,
        Looping_Call10,
    }
}
