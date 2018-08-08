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

using SocialNetworksManager.Contracts;

namespace VkExtension
{
    /// <summary>
    /// Interaction logic for AuthControl.xaml
    /// </summary>
    public partial class AuthControl : UserControl
    {
        private IApplicationContract applicationContract;

        public AuthControl(IApplicationContract applicationContract)
        {
            InitializeComponent();

            this.applicationContract = applicationContract;
        }

        private void Button_LogIn_Click(object sender, RoutedEventArgs e)
        {
            applicationContract.CloseSpecialWindow();
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
