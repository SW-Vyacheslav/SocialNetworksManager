using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;

using SocialNetworksManager.Contracts;
using SocialNetworksManager.DataPresentation;

using MahApps.Metro.Controls;

namespace SocialNetworksManager
{
    [Export(typeof(IApplicationContract))]
    public partial class MainWindow : MetroWindow, IApplicationContract
    {
        private DirectoryCatalog     directoryCatalog     = null;
        private CompositionContainer compositionContainer = null;
        private ImportManager        importManager        = null;

        private List<SocialNetworksListItem> socialNetworksListItems = new List<SocialNetworksListItem>();
        private List<FriendsListItem>        friendsListItems        = new List<FriendsListItem>();
        private List<PhotosListItem>         photosListItems         = new List<PhotosListItem>();
        private List<SendMessageStatus>      messagesStatuses        = new List<SendMessageStatus>();

        private Thread checkConnectionThread;
        private SpecialWindow specialWindow;

        private Boolean IsContainerInitialized = false;

        private delegate void SetNoConnectionPageVisibilityDelegate(Boolean isVisible);

        public MainWindow()
        {
            InitializeComponent();
            InitializeContainer();
            if (!IsContainerInitialized) pages.IsEnabled = false;
            else RefreshExtensions();
            InitializeThreads();
        }

        #region UsualMethods
        private void SetNoConnectionPageVisibility(Boolean isVisible)
        {
            if (isVisible == true && noConnetionPage.Visibility != Visibility.Visible)
            {
                pages.Visibility = Visibility.Collapsed;
                noConnetionPage.Visibility = Visibility.Visible;
                button_refresh_extensions.IsEnabled = false;
            }
            else if (isVisible == false && noConnetionPage.Visibility != Visibility.Collapsed)
            {
                pages.Visibility = Visibility.Visible;
                noConnetionPage.Visibility = Visibility.Collapsed;
                button_refresh_extensions.IsEnabled = true;
            }
        }

        private void CheckConnectionThreadProc()
        {
            int description;
            SetNoConnectionPageVisibilityDelegate @delegate = new SetNoConnectionPageVisibilityDelegate(SetNoConnectionPageVisibility);

            while (true)
            {
                if (Helpers.NetHelper.InternetGetConnectedState(out description, 0)) Dispatcher.Invoke(@delegate, false);
                else Dispatcher.Invoke(@delegate, true);
            }
        }

        private void InitializeThreads()
        {
            checkConnectionThread = new Thread(CheckConnectionThreadProc);
            checkConnectionThread.IsBackground = true;

            checkConnectionThread.Start();
        }

        private void InitializeContainer()
        {
            String dirPath = Environment.CurrentDirectory + "\\bin";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                MessageBox.Show("Extensions directory does not exists.\nExtensions did not loaded.");
                IsContainerInitialized = false;
                return;
            }
            if (Directory.GetFiles(dirPath).Length == 0)
            {
                MessageBox.Show("There are no extensions.");
                IsContainerInitialized = false;
                return;
            }

            directoryCatalog = new DirectoryCatalog(dirPath);
            compositionContainer = new CompositionContainer(directoryCatalog);
            importManager = new ImportManager();

            try
            {
                compositionContainer.ComposeParts(this, importManager);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
                Environment.Exit(1);
            }

            IsContainerInitialized = true;
        }

        private void RefreshExtensions()
        {
            if (!IsContainerInitialized)
            {
                InitializeContainer();
                if (!IsContainerInitialized) return;
                else pages.IsEnabled = true;
            }

            if (directoryCatalog == null) return;
            directoryCatalog.Refresh();
            socialNetworksHolder.ItemsSource = null;

            socialNetworksListItems = new List<SocialNetworksListItem>();
            
            foreach (Lazy<ISocialNetworksManagerExtension> extension in importManager.extensionsCollection)
            {
                SocialNetworksListItem methodsItem = new SocialNetworksListItem();
                methodsItem.Name = extension.Value.getSocialNetworkName();
                methodsItem.AuthorizedUsers = extension.Value.getAuthorizedUsers();
                
                socialNetworksListItems.Add(methodsItem);
            }

            socialNetworksHolder.ItemsSource = socialNetworksListItems;
        }

        private ISocialNetworksManagerExtension findSocialNetworkExtensionByName(String name)
        {
            foreach (Lazy<ISocialNetworksManagerExtension> extension in importManager.extensionsCollection)
            {
                if (extension.Value.getSocialNetworkName().Equals(name))
                {
                    return extension.Value;
                }
            }

            return null;
        }

        private void UpdateFriends()
        {
            friendsList.ItemsSource = null;
            friendsListItems.Clear();

            foreach (Lazy<ISocialNetworksManagerExtension> item in importManager.extensionsCollection)
            {
                item.Value.GetFriends();
            }

            friendsList.ItemsSource = friendsListItems;
        }

        private void UpdatePhotos()
        {
            myphotos_socialnetworks_buttons.Items.Clear();
            friendsphotos_socialnetworks_buttons.Items.Clear();

            foreach (Lazy<ISocialNetworksManagerExtension> item in importManager.extensionsCollection)
            {
                String socNetName = item.Value.getSocialNetworkName();
                List<UserInfo> socNetUsers = item.Value.getAuthorizedUsers();

                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.Header = socNetName;

                foreach (UserInfo userInfo in socNetUsers)
                {
                    ButtonWithUserInfo buttonWithUserInfo = new ButtonWithUserInfo();
                    buttonWithUserInfo.User = userInfo;
                    buttonWithUserInfo.Content = userInfo.Name;
                    buttonWithUserInfo.Click += ButtonWithUserInfo_Click;
                    
                    treeViewItem.Items.Add(buttonWithUserInfo);
                }

                myphotos_socialnetworks_buttons.Items.Add(treeViewItem);
            }

            foreach (Lazy<ISocialNetworksManagerExtension> item in importManager.extensionsCollection)
            {
                String socNetName = item.Value.getSocialNetworkName();

                friendsListItems.Clear();
                item.Value.GetFriends();

                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.Header = socNetName;

                foreach (FriendsListItem listItem in friendsListItems)
                {
                    ButtonWithUserInfo buttonWithUserInfo = new ButtonWithUserInfo();
                    buttonWithUserInfo.User = listItem.Friend;
                    buttonWithUserInfo.Content = listItem.Friend.Name;
                    buttonWithUserInfo.User.SocialNetworkName = item.Value.getSocialNetworkName();
                    buttonWithUserInfo.Click += ButtonWithUserInfo_Click;

                    treeViewItem.Items.Add(buttonWithUserInfo);
                }

                friendsphotos_socialnetworks_buttons.Items.Add(treeViewItem);
            }
        }
        #endregion

        #region ContractMethods
        public void AddItemsToFriendsList(List<FriendsListItem> items)
        {
            friendsListItems.AddRange(items);
        }

        public void AddItemsToPhotosList(List<PhotosListItem> items)
        {
            photosListItems.AddRange(items);
        }

        public void OpenSpecialWindow(Uri uri, Uri redirect_uri, Dictionary<String, String> parameters)
        {
            specialWindow = new SpecialWindow(uri,redirect_uri,parameters);
            try
            {
                specialWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                OpenSpecialWindow(ex.Message);
            }
        }

        public void OpenSpecialWindow(UserControl userControl)
        {
            specialWindow = new SpecialWindow(userControl);
            specialWindow.ShowDialog();
        }

        public void OpenSpecialWindow(String text)
        {
            SpecialWindow specialWindow = new SpecialWindow(text);
            specialWindow.ShowDialog();
        }

        public void CloseSpecialWindow()
        {
            specialWindow.Close();
        }

        public List<FriendsListItem> GetFriendsListItems()
        {
            return friendsListItems;
        }

        public string GetMessage()
        {
            return message_text_box.Text;
        }

        public void AddSendMessageStatuses(List<SendMessageStatus> statuses)
        {
            messagesStatuses.AddRange(statuses);
        }
        #endregion

        #region EventMethods
        private void ButtonWithUserInfo_Click(object sender, RoutedEventArgs e)
        {
            ButtonWithUserInfo buttonWithUserInfo = sender as ButtonWithUserInfo;

            photosListItems.Clear();
            photos_holder.Children.Clear();

            username_textbox.Text = buttonWithUserInfo.User.Name;
            findSocialNetworkExtensionByName(buttonWithUserInfo.User.SocialNetworkName).GetPhotos(buttonWithUserInfo.User.ID);
            
            for (int i = 0; i < photosListItems.Count; i++)
            {
                Image photo = new Image();
                photo.Source = photosListItems[i].Photo;
                photo.Width = 200;
                photo.Height = 200;
                photo.Margin = new Thickness(5);
                
                photos_holder.Children.Add(photo);
            }
        }

        private void Button_RefreshExtensions_Click(object sender, RoutedEventArgs e)
        {
            RefreshExtensions();
        }

        private void Button_SendMessage_Click(object sender, RoutedEventArgs e)
        {
            messagesStatuses.Clear();

            foreach (Lazy<ISocialNetworksManagerExtension> item in importManager.extensionsCollection)
            {
                item.Value.SendMessageToSelectedFriends();
            }

            StringBuilder messagesStatusesString = new StringBuilder();

            foreach (SendMessageStatus status in messagesStatuses)
            {
                messagesStatusesString.AppendFormat("{0}: Message from {1} to {2} {3}.\n",status.SocialNetworkName,status.UserNameFrom,status.UserNameTo,status.IsMessageSended == true ? "Sended" : "Not Sended");
            }

            SpecialWindow specialWindow = new SpecialWindow(messagesStatusesString.ToString());
            specialWindow.ShowDialog();
        }

        private void Button_Auth_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            SocialNetworksListItem item = (button).DataContext as SocialNetworksListItem;
            ISocialNetworksManagerExtension extension = findSocialNetworkExtensionByName(item.Name);
            extension.Authorization();

            item.AuthorizedUsers = extension.getAuthorizedUsers();
        }

        private void Button_SelectAllFriends_Click(object sender, RoutedEventArgs e)
        {
            foreach (FriendsListItem item in friendsListItems)
            {
                item.IsChecked = true;
            }
        }

        private void Button_DeselectAllFriends_Click(object sender, RoutedEventArgs e)
        {
            foreach (FriendsListItem item in friendsListItems)
            {
                item.IsChecked = false;
            }
        }

        private void pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is MetroAnimatedTabControl)
            {
                MetroAnimatedTabControl tabControl = sender as MetroAnimatedTabControl;
                MetroTabItem tabItem = tabControl.SelectedItem as MetroTabItem;

                switch (tabItem.Header)
                {
                    case "Friends":
                        UpdateFriends();
                        break;
                    case "Photos":
                        UpdatePhotos();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}