#region Copyright Syncfusion® Inc. 2001-2026.
// Copyright Syncfusion® Inc. 2001-2026. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;

namespace AISettingsWindow
{
    // Minimal view model used by the WinForms form.
    public class DemoBrowserViewModel : INotifyPropertyChanged
    {
        private string modelName;
        private string key;
        private string endPoint;
        private bool isAISettingsWindowBusy;

        public event PropertyChangedEventHandler PropertyChanged;
        public Action<string> ClickAction { get; set; }

        public string ModelName
        {
            get => modelName;
            set { modelName = value; RaisePropertyChanged(nameof(ModelName)); }
        }

        public string Key
        {
            get => key;
            set { key = value; RaisePropertyChanged(nameof(Key)); }
        }

        public string EndPoint
        {
            get => endPoint;
            set { endPoint = value; RaisePropertyChanged(nameof(EndPoint)); }
        }

        public bool IsAISettingsWindowBusy
        {
            get => isAISettingsWindowBusy;
            set { isAISettingsWindowBusy = value; RaisePropertyChanged(nameof(IsAISettingsWindowBusy)); }
        }

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
