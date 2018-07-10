using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using SocialNetworksManager.Contracts;

namespace SocialNetworksManager
{
    public partial class MainWindow : Window
    {
        private DirectoryCatalog directoryCatalog;
        private CompositionContainer compositionContainer;
        private ImportManager importManager;

        public MainWindow()
        {
            InitializeComponent();
            InitializeContainer();
            RefreshExtensions();
        }

        private void InitializeContainer()
        {
            String dirPath = Environment.CurrentDirectory + "\\Extensions";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                MessageBox.Show("Extensions directory does not exists.\nExtensions did not loaded.");
                return;
            }
            else
            {
                String[] files = Directory.GetFiles(dirPath);
                if (files.Length == 0)
                {
                    MessageBox.Show("There are no extensions.");
                    return;
                }
            }

            directoryCatalog = new DirectoryCatalog(dirPath);
            compositionContainer = new CompositionContainer(directoryCatalog);
            importManager = new ImportManager();

            importManager.ImportSatisfied += (sender, e) => 
            {
                MessageBox.Show(e.Status,"Import");
            };

            compositionContainer.ComposeParts(importManager);
        }

        private void RefreshExtensions()
        {
            if (directoryCatalog == null) return;
            directoryCatalog.Refresh();

            socialNetworksHolder.Children.Clear();

            foreach(Lazy<ISocialNetworksManagerExtension> extension in importManager.extensionsCollection)
            {
                Button button = new Button();
                button.Content = extension.Value.getSocialNetworkName();
                button.BorderBrush = new SolidColorBrush(Colors.Black);
                button.Click += Button_Click;
                socialNetworksHolder.Children.Add(button);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (Lazy<ISocialNetworksManagerExtension> extension in importManager.extensionsCollection)
            {
                if (extension.Value.getSocialNetworkName().Equals(((Button)sender).Content))
                {
                    MessageBox.Show(extension.Value.getExtensionName());
                    break;
                }
            }
        }
    }
}
