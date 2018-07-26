using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Converters;

namespace VKExtension.Abilities
{
    public static class VkApiLinkCreator
    {
        public static String CreateLink(Enums.VkMethod method, IDictionary<String,String> parameters)
        {
            StringBuilder link = new StringBuilder();

            link.Append("https://api.vk.com/method/");
            link.Append(EnumConverter.ConvertToString<Enums.VkMethod>(method));
            link.Append('?');

            for (int i = 0; i < parameters.Count; i++)
            {
                if (i != 0) link.Append('&');
                link.AppendFormat("{0}={1}", parameters.Keys.ElementAt(i), parameters.Values.ElementAt(i));
            }

            return link.ToString();
        }
    }
}
