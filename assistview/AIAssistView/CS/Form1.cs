#region Copyright Syncfusion® Inc. 2001-2026.
// Copyright Syncfusion® Inc. 2001-2026. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syncfusion.WinForms.AIAssistView;
using System.Collections.Specialized;
using System.Windows.Forms.Integration;
using System.Windows;
using AISettingsWindow;
using System.Runtime.Versioning;

namespace AssistViewDemo
{
#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public partial class Form1 : Form
    {
        AIAssistViewModel viewModel = new AIAssistViewModel();

        private Dictionary<TextMessage, ElementHost> botControlMapping = new Dictionary<TextMessage, ElementHost>();

#if NETFRAMEWORK
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int GetDpiForWindow(IntPtr hWnd);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    private const int LOGPIXELSX = 88;
    private const int LOGPIXELSY = 90;
#endif

        public Form1()
        {
            InitializeComponent();
            sfaiAssistView1.DataBindings.Add("Messages", viewModel, "Chats", true, DataSourceUpdateMode.OnPropertyChanged);
            sfaiAssistView1.DataBindings.Add("ShowTypingIndicator", viewModel, "ShowTypingIndicator", true, DataSourceUpdateMode.OnPropertyChanged);
            sfaiAssistView1.DataBindings.Add("Suggestions", viewModel, "Suggestion", true, DataSourceUpdateMode.OnPropertyChanged);

            viewModel.InitAI();

            if (viewModel.CurrentUser != null && !string.IsNullOrEmpty(sfaiAssistView1.User.Name))
            {
                viewModel.CurrentUser = sfaiAssistView1.User;
            }
            else
            {
                sfaiAssistView1.User = viewModel.CurrentUser;
            }

            sfaiAssistView1.Dock = DockStyle.Fill;
            sfaiAssistView1.ShowTypingIndicator = viewModel.ShowTypingIndicator;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;

            BannerTemplate();

            sfaiAssistView1.TypingIndicator.Author = new Author() { Name = "Bot", AvatarImage = Image.FromFile(@"Asset\AI_Assist.png") };
            sfaiAssistView1.TypingIndicator.DisplayText = "Typing";
            sfaiAssistView1.SuggestionSelected += OnSuggestionSelected;

            viewModel.Chats.CollectionChanged += Chats_CollectionChanged;
            this.Resize += Form1_Resize;
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            viewModel.Chats.CollectionChanged -= Chats_CollectionChanged;
            this.Resize -= Form1_Resize;
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;

            foreach (var kvp in botControlMapping)
            {
                if (kvp.Value is ElementHost host)
                {
                    if (host.Child is IDisposable disposableChild)
                        disposableChild.Dispose();
                    host.Child = null;
                    host.Dispose();
                }
            }
            botControlMapping.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowAISettings();
        }

        private void ShowAISettings()
        {
            var demoViewModel = new DemoBrowserViewModel();
            demoViewModel.EndPoint = AISettings.EndPoint;
            demoViewModel.ModelName = AISettings.ModelName;
            demoViewModel.Key = AISettings.Key;

            var settingsForm = new AISettingsForm(demoViewModel);
            settingsForm.StartPosition = FormStartPosition.CenterParent;
            if (settingsForm.ShowDialog(this) == DialogResult.OK)
            {
                viewModel.UpdateAI(demoViewModel.Key, demoViewModel.ModelName, demoViewModel.EndPoint);
            }
        }

        private void Chats_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if(item is TextMessage message && message.Author.Name == "Bot")
                    {
                        AddWPFControlToWF(message);
                    }
                }
            }
        }

        private void AddWPFControlToWF(TextMessage message)
        {
            var wpfHostControl = new WPFApplication();
            wpfHostControl.MarkDownText = message.Text;

            ElementHost wpfHost = new ElementHost()
            {
                AutoSize = false,
                BackColorTransparent = true,
                Child = wpfHostControl
            };

            botControlMapping[message] = wpfHost;

            sfaiAssistView1.SetBotView(message, wpfHost);

            wpfHostControl.Dispatcher.BeginInvoke(new Action(async () =>
            {
                await Task.Delay(100);
                UpdateHostControlSize(message, wpfHost);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public bool isUpdating = false;
        private void UpdateHostControlSize(TextMessage message, ElementHost wpfHost)
        {
            if (isUpdating) return;

            if(wpfHost.Child is WPFApplication wpfApplication)
            {
                try
                {
                    isUpdating = true;
                    double maxWidth = Math.Max(100, (double)this.ClientSize.Width * 0.65);
                    wpfApplication.MaxWidth = maxWidth;

                    wpfApplication.Measure(new System.Windows.Size(maxWidth, double.PositiveInfinity));
                    wpfApplication.Arrange(new Rect(new System.Windows.Point(0,0), wpfApplication.DesiredSize));

                    wpfApplication.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        wpfApplication.UpdateLayout();
                        wpfApplication.InvalidateMeasure();
                    }));

                    using (Graphics graphics = this.CreateGraphics())
                    {
                        float dpiX;
                        float dpiY;
#if NETCOREAPP
                        dpiX = this.DeviceDpi;
                        dpiY = this.DeviceDpi;
#else
                    try
                    {
                        int windowDpi = GetDpiForWindow(this.Handle);
                        if (windowDpi > 0)
                        {
                            dpiX = windowDpi;
                            dpiY = windowDpi;
                        }
                        else
                        {
                            var dpiProperty = this.GetType().GetProperty("DeviceDpi", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                            if (dpiProperty != null)
                            {
                                float deviceDpi = (float)(int)dpiProperty.GetValue(this);
                                dpiX = deviceDpi;
                                dpiY = deviceDpi;
                            }
                            else
                            {
                                IntPtr hdc = GetDC(this.Handle);
                                if (hdc != IntPtr.Zero)
                                {
                                    dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
                                    dpiY = GetDeviceCaps(hdc, LOGPIXELSY);
                                    ReleaseDC(this.Handle, hdc);
                                }
                                else
                                {
                                    dpiX = graphics.DpiX;
                                    dpiY = graphics.DpiY;
                                }
                            }
                        }
                    }
                    catch
                    {
                        dpiX = graphics.DpiX;
                        dpiY = graphics.DpiY;
                    }
#endif
                        float scaleX = dpiX / 96F;
                        float scaleY = dpiY / 96F;

                        int calculateWidth = (int)Math.Ceiling(wpfApplication.DesiredSize.Width * scaleX);
                        int calculateHeight = (int)Math.Ceiling(wpfApplication.DesiredSize.Height * scaleY);

                        wpfHost.Size = new System.Drawing.Size(calculateWidth, calculateHeight);
                        wpfHost.MinimumSize = new System.Drawing.Size(calculateWidth, calculateHeight);
                        wpfHost.MaximumSize = new System.Drawing.Size(calculateWidth, calculateHeight);
                    }

                    sfaiAssistView1.SetBotView(message, null);
                    sfaiAssistView1.SetBotView(message, wpfHost);
                }
                finally
                {
                    isUpdating = false;
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (isUpdating) return;

            sfaiAssistView1.SuspendLayout();

            foreach(var kvp in botControlMapping)
            {
                UpdateHostControlSize(kvp.Key, kvp.Value);
            }

            sfaiAssistView1.ResumeLayout(true);
        }

        private void OnSuggestionSelected(object sender, SuggestionSelectedEventArgs e)
        {
            if (e.Item is string plainString)
            {
                viewModel.SendUserMessage(plainString);
            }
        }

        private void BannerTemplate()
        {
            BannerStyle customStyle = new BannerStyle
            {
                TitleFont = new Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold),
                SubTitleFont = new Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular),
                ImageSize = AvatarSize.Medium,
            };

            string title = "AI Assist ";
            string subTitle = "Your best AI Companion";
            sfaiAssistView1.SetBannerView(title, subTitle, Image.FromFile(@"Asset\AI_Assist.png"), customStyle);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowTypingIndicator")
            {
                sfaiAssistView1.ShowTypingIndicator = viewModel.ShowTypingIndicator;
            }
        }
    }
}
