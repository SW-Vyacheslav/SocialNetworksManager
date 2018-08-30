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
        //MEF components
        private DirectoryCatalog directory_catalog = null;
        private CompositionContainer composition_container = null;
        private ImportManager import_manager = null;

        //Lists
        private List<SocialNetworksListItem> social_networks_list_items = new List<SocialNetworksListItem>();
        private List<FriendsListItem> friends_list_items = new List<FriendsListItem>();
        private List<PhotosListItem> photos_list_items = new List<PhotosListItem>();
        private List<SendMessageStatus> messages_statuses = new List<SendMessageStatus>();

        //Other fields
        private UserInfo photos_user_info = null;
        private Int32 old_photos_list_items_count = 0;
        private Thread check_connection_thread;
        private SpecialWindow special_window;
        private Boolean IsContainerInitialized = false;

        //Delegates
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
            check_connection_thread = new Thread(CheckConnectionThreadProc);
            check_connection_thread.IsBackground = true;

            check_connection_thread.Start();
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

            directory_catalog = new DirectoryCatalog(dirPath);
            composition_container = new CompositionContainer(directory_catalog);
            import_manager = new ImportManager();

            try
            {
                composition_container.ComposeParts(this, import_manager);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
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

            if (directory_catalog == null) return;
            directory_catalog.Refresh();
            socialNetworksHolder.ItemsSource = null;

            social_networks_list_items = new List<SocialNetworksListItem>();

            foreach (Lazy<ISocialNetworksManagerExtension> extension in import_manager.extensionsCollection)
            {
                SocialNetworksListItem methodsItem = new SocialNetworksListItem();
                methodsItem.Name = extension.Value.getSocialNetworkName();
                methodsItem.AuthorizedUsers = extension.Value.getAuthorizedUsers();

                social_networks_list_items.Add(methodsItem);
            }

            socialNetworksHolder.ItemsSource = social_networks_list_items;
        }

        private ISocialNetworksManagerExtension findSocialNetworkExtensionByName(String name)
        {
            foreach (Lazy<ISocialNetworksManagerExtension> extension in import_manager.extensionsCollection)
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
            friends_list_items.Clear();

            foreach (Lazy<ISocialNetworksManagerExtension> item in import_manager.extensionsCollection)
            {
                item.Value.GetFriends();
            }

            friendsList.ItemsSource = friends_list_items;
        }

        private void UpdatePhotos()
        {
            myphotos_socialnetworks_buttons.Items.Clear();
            friendsphotos_socialnetworks_buttons.Items.Clear();

            foreach (Lazy<ISocialNetworksManagerExtension> item in import_manager.extensionsCollection)
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

            foreach (Lazy<ISocialNetworksManagerExtension> item in import_manager.extensionsCollection)
            {
                String socNetName = item.Value.getSocialNetworkName();

                friends_list_items.Clear();
                item.Value.GetFriends();

                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.Header = socNetName;

                foreach (FriendsListItem listItem in friends_list_items)
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
            friends_list_items.AddRange(items);
        }

        public void AddItemsToPhotosList(List<PhotosListItem> items)
        {
            photos_list_items.AddRange(items);
        }

        public void OpenSpecialWindow(Uri uri, Uri redirect_uri, Dictionary<String, String> parameters)
        {
            special_window = new SpecialWindow(uri, redirect_uri, parameters);
            try
            {
                special_window.ShowDialog();
            }
            catch (Exception ex)
            {
                OpenSpecialWindow(ex.Message);
            }
        }

        public void OpenSpecialWindow(UserControl userControl)
        {
            special_window = new SpecialWindow(userControl);
            special_window.ShowDialog();
        }

        public void OpenSpecialWindow(String text)
        {
            special_window = new SpecialWindow(text);
            special_window.ShowDialog();
        }

        public void CloseSpecialWindow()
        {
            special_window.Close();
        }

        public List<FriendsListItem> GetFriendsListItems()
        {
            return friends_list_items;
        }

        public string GetMessage()
        {
            return message_text_box.Text;
        }

        public void AddSendMessageStatuses(List<SendMessageStatus> statuses)
        {
            messages_statuses.AddRange(statuses);
        }

        public void SetPhotosListSatusData(String data)
        {
            photoslist_satus_data.Content = data;
        }

        public string GetUserID()
        {
            return photos_user_info.ID;
        }

        public ulong GetPhotosCount()
        {
            return (ulong)photos_list_items.Count;
        }

        public void DisableNextPhotosButton()
        {
            next_photos_button.IsEnabled = false;
        }
        #endregion

        #region EventMethods
        private void Button_RefreshPhotos_Click(object sender, RoutedEventArgs e)
        {
            if (photos_user_info == null) return;

            next_photos_button.IsEnabled = true;

            photos_list_items.Clear();
            photos_holder.Children.Clear();

            findSocialNetworkExtensionByName(photos_user_info.SocialNetworkName).GetPhotos();

            old_photos_list_items_count = photos_list_items.Count;

            for (int i = 0; i < photos_list_items.Count; i++)
            {
                Image photo = new Image();
                photo.Source = photos_list_items[i].Photo;
                photo.Style = FindResource("PhotoStyle") as Style;

                photos_holder.Children.Add(photo);
            }
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            if (photos_user_info == null) return;

            findSocialNetworkExtensionByName(photos_user_info.SocialNetworkName).GetPhotos();

            for (int i = old_photos_list_items_count; i < photos_list_items.Count; i++)
            {
                Image photo = new Image();
                photo.Source = photos_list_items[i].Photo;
                photo.Style = FindResource("PhotoStyle") as Style;

                photos_holder.Children.Add(photo);
            }
        }

        private void ButtonWithUserInfo_Click(object sender, RoutedEventArgs e)
        {
            ButtonWithUserInfo buttonWithUserInfo = sender as ButtonWithUserInfo;

            if (buttonWithUserInfo.User == null) return;

            next_photos_button.IsEnabled = true;

            photos_user_info = new UserInfo();
            photos_user_info.ID = buttonWithUserInfo.User.ID;
            photos_user_info.Name = buttonWithUserInfo.User.Name;
            photos_user_info.SocialNetworkName = buttonWithUserInfo.User.SocialNetworkName;
            username_textbox.Text = buttonWithUserInfo.User.Name;
            photo_size_slider.Value = photo_size_slider.Minimum;
            old_photos_list_items_count = photos_list_items.Count;

            photos_list_items.Clear();
            photos_holder.Children.Clear();

            findSocialNetworkExtensionByName(buttonWithUserInfo.User.SocialNetworkName).GetPhotos();

            for (int i = 0; i < photos_list_items.Count; i++)
            {
                Image photo = new Image();
                photo.Source = photos_list_items[i].Photo;
                photo.Style = FindResource("PhotoStyle") as Style;

                photos_holder.Children.Add(photo);
            }
        }

        private void Button_RefreshExtensions_Click(object sender, RoutedEventArgs e)
        {
            RefreshExtensions();
        }

        private void Button_SendMessage_Click(object sender, RoutedEventArgs e)
        {
            messages_statuses.Clear();

            foreach (Lazy<ISocialNetworksManagerExtension> item in import_manager.extensionsCollection)
            {
                item.Value.SendMessageToSelectedFriends();
            }

            if (messages_statuses.Count == 0) return;

            StringBuilder messagesStatusesString = new StringBuilder();

            foreach (SendMessageStatus status in messages_statuses)
            {
                messagesStatusesString.AppendFormat("{0}: Message from {1} to {2} {3}.\n", status.SocialNetworkName, status.UserNameFrom, status.UserNameTo, status.IsMessageSended == true ? "Sended" : "Not Sended");
            }

            OpenSpecialWindow(messagesStatusesString.ToString());
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
            foreach (FriendsListItem item in friends_list_items)
            {
                item.IsChecked = true;
            }
        }

        private void Button_DeselectAllFriends_Click(object sender, RoutedEventArgs e)
        {
            foreach (FriendsListItem item in friends_list_items)
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

        private void Slider_PhotoSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UIElementCollection element_collection = photos_holder?.Children;

            if (element_collection == null) return;

            foreach (UIElement item in element_collection)
            {
                Image img = item as Image;

                img.Width = e.NewValue;
                img.Height = e.NewValue;
            }
        }
        #endregion
    }
}