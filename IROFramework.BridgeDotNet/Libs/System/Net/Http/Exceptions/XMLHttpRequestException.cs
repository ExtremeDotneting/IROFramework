using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge.Html5;
using Libs.Utils;

namespace Libs.Libs.System.Net.Http.Exceptions
{
    public class XMLHttpRequestException : Exception
    {
        public XMLHttpRequest XMLHttpRequest { get; set; }

        public XMLHttpRequestException(string msg, XMLHttpRequest req)
        {
            XMLHttpRequest = req;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
