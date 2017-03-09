using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.System.Profile;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        
        public static int GetStringAvailableIndex(MeasureFontRequest req)
        {
            if (string.IsNullOrEmpty(req.Input)) return -1;
            var cloneReq = req.Clone();
            var availableSize = req.AvailableSize.Width * req.AvailableSize.Height;
            var maxCharNum = req.Input.Length;
            //判断宽高是否为一个可读数
            int availableSizeInt;
            if (Int32.TryParse(availableSize.ToString(), out availableSizeInt))
            {
                //一个字符的宽高
                cloneReq.Input = "a";
                cloneReq.AvailableSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
                var charSize = MeasureStringSize(cloneReq);
                maxCharNum = (int)(availableSize / charSize.Width / charSize.Height + req.AvailableSize.Width / charSize.Width);
            }
            if(req.IsNext)
                cloneReq.Input = maxCharNum >= req.Input.Length ? req.Input : req.Input.Substring(0, maxCharNum);
            else
                cloneReq.Input = maxCharNum >= req.Input.Length ? req.Input : req.Input.Substring(req.Input.Length - maxCharNum, maxCharNum);

            cloneReq.AvailableSize = req.AvailableSize;
            var size = MeasureStringSize(cloneReq);
            if (size.Height > req.AvailableSize.Height)
            {
                if (cloneReq.Input.Length - 1 == 0) return 0;
                if(req.IsNext)
                    cloneReq.Input = cloneReq.Input.Substring(0, cloneReq.Input.Length - 1);
                else
                    cloneReq.Input = cloneReq.Input.Substring(1, cloneReq.Input.Length - 1);
                return GetStringAvailableIndex(cloneReq);
            }
            else return cloneReq.Input.Length;
        }

        public static Size MeasureStringSize(MeasureFontRequest req)
        {
            var tb = new TextBlock();
            tb.MaxLines = req.MaxLines;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Text = req.Input;
            tb.FontFamily = req.FontFamily;
            tb.FontSize = req.FontSize;
            tb.Measure(req.AvailableSize);
            Size actualSize = new Size();
            actualSize.Width = tb.ActualWidth;
            actualSize.Height = tb.ActualHeight;
            return actualSize;
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

    public class MeasureFontRequest
    {
        public int MaxLines { get; set; }
        public string Input { get; set; }
        public FontFamily FontFamily { get; set; }
        public double FontSize { get; set; }
        public Size AvailableSize { get; set; }
        public bool IsNext { get; set; }
        public MeasureFontRequest()
        {
            FontSize = 15;
            FontFamily = new FontFamily("Segoe UI");
            AvailableSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
        }

        public MeasureFontRequest Clone()
        {
            var clone = new MeasureFontRequest()
            {
                Input = Input,
                FontFamily = FontFamily,
                FontSize = FontSize,
                AvailableSize = AvailableSize,
                IsNext = IsNext,
            };
            return clone;
        }
    }
}
