using Launcher.WinUI.Logic;
using Launcher.WinUI.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.EnterpriseData;
using CommunityToolkit.Mvvm.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Launcher.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private bool _initialized = false;
        UILogic uiLogic = new();

        public MainWindow()
        {
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;
            
            if (Content is FrameworkElement root)
            {
                root.Loaded += (_, _) =>
                {
                    OnLaunched();
                };
            }
        }

        private void OnLaunched()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            InitializeLauncher();
        }

        private void InitializeLauncher()
        {
            Debug.WriteLine("[Info] MainWindow: Initializing Launcher");
            uiLogic.StartUILogic();
            Debug.WriteLine("[Info] MainWindow: UILogic started");
            this.SetDefaultPage("dashboard");
            Debug.WriteLine("[Info] MainWindow: DefaultStartPage set");
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                this.SetActivePage(item.Tag.ToString());
            }
        }

        private void SetActivePage(string? tag)
        {
            switch (tag)
            {
                case "dashboard":
                    ContentFrame.Navigate(typeof(DashboardPage), uiLogic);
                    break;

                case "assets":
                    ContentFrame.Navigate(typeof(AssetsPage), uiLogic);
                    break;

                case "Settings":
                    ContentFrame.Navigate(typeof(SettingsPage), uiLogic);
                    break;
            }
        }

        private void SetDefaultPage(string? tag)
        {
            var navViewMenuItem = NavView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(x => x.Tag?.ToString() == tag);
            NavView.SelectedItem = navViewMenuItem;
        }
    }
}
