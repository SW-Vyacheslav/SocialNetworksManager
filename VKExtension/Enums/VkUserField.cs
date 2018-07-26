using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Converters.Attributes;
using Converters;

namespace VKExtension.Enums
{
    [Flags]
    public enum VkUserField
    {
        [StringValue("id")]
        ID = 1,

        [StringValue("first_name")]
        FirstName = 2,

        [StringValue("last_name")]
        LastName = 4,

        [StringValue("online")]
        Online = 8,

        [StringValue("deactivated")]
        Deactivated = 16
    }
}
