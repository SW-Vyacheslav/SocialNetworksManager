using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Converters.Attributes;

namespace VKExtension.Enums
{
    [Flags]
    public enum VkMethod
    {
        [StringValue("friends.get")]
        GetFriends = 1,

        [StringValue("photos.get")]
        GetPhotos = 2,

        [StringValue("newsfeed.get")]
        GetNewsFeed = 4
    }
}
