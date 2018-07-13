using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using SocialNetworksManager.Contracts;
using System.Reflection;

namespace SocialNetworksManager
{
    delegate void lol();

    [Export(typeof(IApplicationContract))]
    public partial class MainWindow : Window, IApplicationContract
    {
        private DirectoryCatalog directoryCatalog;
        private CompositionContainer compositionContainer;
        private ImportManager importManager;

        public MainWindow()
        {
            InitializeComponent();
            InitializeContainer();
            RefreshExtensions();

            but_refreshExtensions.Click += But_refreshExtensions_Click;
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

            compositionContainer.ComposeParts(this,importManager);
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

        public void setTextBoxValue(string value)
        {
            text_output.Text = value;
        }

        private void But_refreshExtensions_Click(object sender, RoutedEventArgs e)
        {
            RefreshExtensions();
        }

        private ISocialNetworksManagerExtension findSocialNetworkExtensionByName(String name)
        {
            foreach (Lazy<ISocialNetworksManagerExtension> extension in importManager.extensionsCollection)
            {
                if(extension.Value.getSocialNetworkName().Equals(name))
                {
                    return extension.Value;
                }
            }

            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            methodsHolder.Children.Clear();

            ISocialNetworksManagerExtension sn = findSocialNetworkExtensionByName(((Button)sender).Content.ToString());
            sn.Authorization();

            MethodInfo[] info = sn.GetType().GetMethods();

            for (int i = 0; i < info.Length; i++)
            {
                Attribute attribute = info[i].GetCustomAttribute(typeof(AvailableInUIAttribute));

                if (attribute != null)
                {
                    Button button = new Button();
                    if (((AvailableInUIAttribute)attribute).UIName == null) button.Content = info[i].Name;
                    else button.Content = ((AvailableInUIAttribute)attribute).UIName;
                    //button.Click += каким-то образом получить ссылку на метод
                    methodsHolder.Children.Add(button);
                }
            }
        }

        public String openAuthWindow(String authpage)
        {
            AuthWindow authWindow = new AuthWindow(authpage);
            authWindow.ShowDialog();

            return authWindow.access_token;
        }
    }
}
