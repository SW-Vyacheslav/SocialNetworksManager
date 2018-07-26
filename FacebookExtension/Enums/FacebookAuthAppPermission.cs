using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using FacebookExtension.Attributes;

namespace FacebookExtension.Enums
{
    [Flags]
    public enum FacebookAuthAppPermission
    {
        [StringValue("email")]
        Email = 1,

        [StringValue("user_age_range")]
        AgeRange = 2,

        [StringValue("user_birthday")]
        BirthDay = 4,

        [StringValue("user_friends")]
        Friends = 8,

        [StringValue("user_gender")]
        Gender = 16,

        [StringValue("user_link")]
        Link = 32,

        [StringValue("user_hometown")]
        Hometown = 64,

        [StringValue("user_location")]
        Location = 128,

        [StringValue("user_likes")]
        Likes = 256,

        [StringValue("user_photos")]
        Photos = 512,

        [StringValue("user_posts")]
        Posts = 1024,

        [StringValue("user_videos")]
        Videos = 2048,

        [StringValue("user_tagged_places")]
        TaggetPlaces = 4096,

        [StringValue("user_events")]
        Events = 8192,

        [StringValue("user_managed_groups")]
        ManagedGroups = 16384,

        [StringValue("groups_access_member_info")]
        GroupsAccessMemberInfo = 32768,

        [StringValue("publish_to_groups")]
        PublishToGroups = 65536,

        [StringValue("email,user_age_range,user_birthday,user_friends,user_gender,user_hometown,user_link,user_photos,user_posts,user_location,user_likes,user_tagged_places,user_videos,groups_access_member_info,user_events,user_managed_groups,publish_to_groups")]
        All = 131072
    }
}
