using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using SocialNetworksManager.Contracts;

namespace VkExtension
{
    /// <summary>
    /// Interaction logic for AuthControl.xaml
    /// </summary>
    public partial class AuthControl : UserControl
    {
        private IApplicationContract applicationContract;

        public Boolean IsCanceled { get; set; }

        public AuthControl(IApplicationContract applicationContract)
        {
            InitializeComponent();

            GetUserEnteredData();
            this.applicationContract = applicationContract;
            IsCanceled = false;
        }

        private void Button_LogIn_Click(object sender, RoutedEventArgs e)
        {
            if (remember_check_box.IsChecked == true)
            {
                SaveUserEnteredData();
            }
            applicationContract.CloseSpecialWindow();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsCanceled = true;
            applicationContract.CloseSpecialWindow();
        }

        private void SaveUserEnteredData()
        {
            String userData = String.Format("{0}:{1}", GetLogin(), GetPassword());
            userData = Helpers.DataHelper.EncryptData(userData);

            using (FileStream fileStream = new FileStream(Environment.CurrentDirectory + "\\Extensions\\userdata", FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    writer.Write(userData);
                }
            }
        }

        private void GetUserEnteredData()
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\Extensions\\userdata")) return;

            using (FileStream fileStream = new FileStream(Environment.CurrentDirectory + "\\Extensions\\userdata", FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    String userData = reader.ReadToEnd();
                    userData = Helpers.DataHelper.DecryptData(userData);
                    login.Text = userData.Split(':')[0];
                    password.Password = userData.Split(':')[1];
                }
            }
        }

        public String GetLogin()
        {
            return login.Text;
        }

        public String GetPassword()
        {
            return password.Password;
        }
    }
}
