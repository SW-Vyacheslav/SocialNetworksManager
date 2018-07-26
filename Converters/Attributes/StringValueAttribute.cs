using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converters.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StringValueAttribute : Attribute
    {
        public String Value { get; private set; }

        public StringValueAttribute(String value)
        {
            Value = value;
        }
    }
}
