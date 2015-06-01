using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lithnet.Logging;
using System.Reflection;
using Microsoft.Win32;
using System.IO;
using Lithnet.Fim;

namespace Lithnet.Acma.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            MmsAssemblyResolver.RegisterResolver();

            // Select the text in a TextBox when it receives focus.
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.PreviewMouseLeftButtonDownEvent,
                new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotKeyboardFocusEvent,
                new RoutedEventHandler(SelectAllText));
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.MouseDoubleClickEvent,
                new RoutedEventHandler(SelectAllText));
            base.OnStartup(e);
        }

        //private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    if (args.Name.StartsWith("Microsoft.MetadirectoryServicesEx", StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\FIMSynchronizationService\Parameters", false);

        //        if (key != null)
        //        {
        //            string path = key.GetValue("Path", null) as string;

        //            if (path != null)
        //            {
        //                Assembly mmsAssemby = Assembly.LoadFrom(Path.Combine(path, "bin\\assemblies\\Microsoft.MetadirectoryServicesEx.dll"));
        //                if (mmsAssemby != null)
        //                {
        //                    return mmsAssemby;
        //                }
        //            }

        //        }

        //        MessageBox.Show(@"The Microsoft.MetadirectoryServicesEx.dll file could not be found on this system. Please copy the file from the bin\Assemblies folder from the FIM Synchronization Server, and place it in the ACMA Editor application directory");
        //        Environment.Exit(9000);
        //    }

        //    return null;
        //}

        void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focused, give it the focus and
                    // stop further processing of this click event.
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null && e.ExceptionObject is Exception)
            {
                Exception ex = e.ExceptionObject as Exception;
                MessageBox.Show("An unexpected error occurred in the editor\n\n" + ex.Message);
                Logger.WriteException(ex);
            }
        }

    }
}
