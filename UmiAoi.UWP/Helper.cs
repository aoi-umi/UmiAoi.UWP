using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.System.Profile;
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
