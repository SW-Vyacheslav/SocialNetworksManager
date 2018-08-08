using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackExtension
{
    public class SlackGetCodeResponse
    {
        public String Code { get; set; }
        public String State { get; set; }
        public Boolean Error { get; set; }
    }
}
