using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackExtension.Exceptions
{
    public class SlackAuthException : Exception
    {
        public SlackAuthException(String message) : base( message) { }
    }
}
