using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRO.MessageBus.Metadata
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BusRequestAttribute : Attribute
    {
        /// <summary>
        /// If null - whill use name of request type.
        /// </summary>
        public string RequestName { get; set; }
    }
}
