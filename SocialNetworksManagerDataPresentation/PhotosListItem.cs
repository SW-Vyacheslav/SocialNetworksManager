using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SocialNetworksManager.DataPresentation
{
    public class PhotosListItem
    {
        public BitmapImage Photo { get; private set; }

        public PhotosListItem(Uri photo_path)
        {
            Photo = new BitmapImage(photo_path);
        }
    }
}
