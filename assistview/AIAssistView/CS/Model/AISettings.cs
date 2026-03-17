#region Copyright Syncfusion® Inc. 2001-2026.
// Copyright Syncfusion® Inc. 2001-2026. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Threading.Tasks;

namespace AISettingsWindow
{
    // Minimal static AI settings helper used by the form.
    internal static class AISettings
    {
        public static string ErrorMessage { get; private set; }
        public static string ModelName { get; set; }
        public static string Key { get; set; }
        public static string EndPoint { get; set; }

        public static async Task ValidateCredentials()
        {
            await Task.Delay(600); // simulate validation

            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(EndPoint) || string.IsNullOrWhiteSpace(Key))
            {
                ErrorMessage = "Endpoint or Key cannot be empty.";
                return;
            }

            // In a real implementation, perform network validation here and set ErrorMessage on failure.
        }
    }
}
