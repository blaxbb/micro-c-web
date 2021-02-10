using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static MicroCLib.Models.BuildComponent;

namespace micro_c_web.Client
{
    public class Settings : INotifyPropertyChanged
    {
        private string store;
        private float taxRate;
        private List<ComponentType> categories;
        private string categoriesPreset;

        public string Store { get => store; set => SetValue(ref store, value); }
        public float TaxRate { get => taxRate; set => SetValue(ref taxRate, value); }
        public List<ComponentType> Categories { get => categories; set => SetValue(ref categories, value); }
        public string CategoriesPreset { get => categoriesPreset; set => SetValue(ref categoriesPreset, value); }

        public Settings()
        {
            Categories = new List<ComponentType>()
            {
                ComponentType.Desktop,
                ComponentType.Laptop,
                ComponentType.CPU,
                ComponentType.GPU
            };
            CategoriesPreset = "Custom";
            Store = "141";
            TaxRate = .075f;
        }

        public static async Task<Settings> Get(IJSRuntime js)
        {
            var settingsString = await js.InvokeAsync<string>("GetLocalStorage", "user-settings");
            if (settingsString == null)
            {
                return new Settings();
            }
            else
            {
                return System.Text.Json.JsonSerializer.Deserialize<Settings>(settingsString);
            }
        }

        private void SetValue<T>(ref T field, object value, [CallerMemberName] string propertyName = "")
        {
            field = (T)value;
            OnPropertyChanged(propertyName);
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
