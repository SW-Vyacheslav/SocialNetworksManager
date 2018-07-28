using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Converters.Attributes;

namespace VKExtension.Enums
{
    [Flags]
    public enum VkPhotoAlbumId
    {
        [StringValue("profile")]
        Profile = 1,

        [StringValue("wall")]
        Wall = 2,

        [StringValue("saved")]
        Saved = 4
    }
}
