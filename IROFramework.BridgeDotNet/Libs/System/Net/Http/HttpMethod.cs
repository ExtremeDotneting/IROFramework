using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge;

namespace System.Net.Http
{
    [Reflectable]
    [Enum(Emit.StringName)]
    public enum HttpMethod
    {
        Get,
        Delete,
        Head,
        Options,
        Patch,
        Post,
        Put,
        Trace
    }
}
