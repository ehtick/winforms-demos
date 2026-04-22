#region Copyright Syncfusion® Inc. 2001-2026.
// Copyright Syncfusion® Inc. 2001-2026. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;

namespace AssistViewDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WPFApplication : UserControl
    {
        public WPFApplication()
        {
            InitializeComponent();
        }

        public Syncfusion.UI.Xaml.Markdown.SfMarkdownViewer Viewer => this.MarkdownViewer;

        public string MarkDownText
        {
            get => MarkdownViewer.Source;
            set
            {
                MarkdownViewer.Source = value;
                MarkdownViewer.UpdateLayout();
            }
        }
    }
}