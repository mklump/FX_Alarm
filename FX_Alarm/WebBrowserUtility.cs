// -----------------------------------------------------------------------
// <copyright file="StringAddressToUri.cs" company="PIPs for Heaven, LLC">
//     Copyright (c) PIPs for Heaven, LLC.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FX_Alarm
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// This class credited from http://stackoverflow.com/questions/263551/databind-the-source-property-of-the-webbrowser-in-wpf
    /// is meant to repair the non-bindable property WebBrowser.Source insinuing problem by instead using the Xaml
    /// ns:WebBrowserUtility.BindableSource="{Binding WebAddress}" that will replace WebBrowser.Source="{Binding WebAddress}"
    /// </summary>
    public static class WebBrowserUtility
    {
        public static readonly DependencyProperty BindableSourceProperty = null;

        static WebBrowserUtility()
        {
            BindableSourceProperty = DependencyProperty.RegisterAttached("BindableSource", typeof(object),
                typeof(WebBrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));
        }

        public static object GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, object value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = o as WebBrowser;
            if (browser == null) return;
            Uri uri = null;
            if (e.NewValue is string)
            {
                var uriString = e.NewValue as string;
                uri = string.IsNullOrWhiteSpace(uriString) ? null : new Uri(uriString);
            }
            else if (e.NewValue is Uri)
            {
                uri = e.NewValue as Uri;
            }
            browser.Source = uri;
        }
    } // End of public static class WebBrowserUtility
} // End of namespace FX_Alarm