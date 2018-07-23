using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKExtension.Enums
{
    public enum VkAuthAppAccessRule
    {
        All = 140491999,
        Notify = 1,
        Friends = 2,
        Photos = 4,
        Audio = 8,
        Video = 16,
        Stories = 64,
        Pages = 128,
        Status = 1024,
        Notes = 2048,
        Messages = 4096,
        Wall = 8192,
        Ads = 32768,
        Offine = 65536,
        Docs = 131072,
        Groups = 262144,
        Notifications = 524288,
        Stats = 1048576,
        Email = 4194304,
        Market = 134217728
    }
}
