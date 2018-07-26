using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Converters.Attributes;
using System.Reflection;

namespace Converters
{
    public static class EnumConverter
    {
        public static String ConvertToString<T>(Enum permission)
        {
            String output = "";
            Type enumtype = typeof(T);
            Type underlyingType = Enum.GetUnderlyingType(enumtype);

            Array values = Enum.GetValues(enumtype);

            for (int i = 0; i < values.Length; i++)
            {
                object value = values.GetValue(i);
                object underlyingValue = Convert.ChangeType(value, underlyingType);

                if (permission.HasFlag((Enum)value))
                {
                    String memberName = Enum.GetName(enumtype, value);
                    MemberInfo[] member = enumtype.GetMember(memberName);
                    object[] attribute = member[0].GetCustomAttributes(typeof(StringValueAttribute), false);

                    if (output == "") output += ((StringValueAttribute)attribute[0]).Value;
                    else output += ("," + ((StringValueAttribute)attribute[0]).Value);
                }
            }

            return output;
        }
    }
}
