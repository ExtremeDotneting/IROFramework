using System;

namespace Libs
{
    public static class ExceptionExt
    {
        public static string ToDetailedString(this Exception ex)
        {
            var msg = "";
            var prefix = "";
            while (true)
            {
                var currentMsg = ex
                    .ToString()
                    .Replace("\n", "\n" + prefix);
                msg += "\n" + prefix + currentMsg;

                prefix += "    ";
                if (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                else
                {
                    break;
                }
            }
            return msg;
        }
    }
}