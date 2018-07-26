using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Converters.Attributes;

namespace VKExtension.Enums
{
    [Flags]
    public enum VkApiVersion
    {
        [StringValue("5.80")]
        V5_80 = 1
    }
}
