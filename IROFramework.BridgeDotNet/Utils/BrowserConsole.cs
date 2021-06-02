using System;
using Bridge;
using Bridge.Html5;

namespace Libs.Utils
{
    public static class BrowserConsole
    {
        static string _lastGroup;
        static int _consoleObjectId;

        public static void WriteLine(object obj)
        {
            Log(obj, null);
        }

        public static void Log(object obj, string group = null)
        {
            Scope(group);
            Script.Call("console.log", obj);
            SetConsoleObject(obj);
        }

        public static void Error(object obj, string group = null)
        {
            Scope(group);
            Script.Call("console.error", obj);
            SetConsoleObject(obj);
        }

        public static void Warn(object obj, string group = null)
        {
            Scope(group);
            Script.Call("console.warn", obj);
            SetConsoleObject(obj);
        }

        public static void Info(object obj, string group = null)
        {
            Scope(group);
            Script.Call("console.info", obj);
            SetConsoleObject(obj);
        }

        public static void GroupEnd()
        {
            Script.Call("console.groupEnd");
        }

        public static void Group(string group)
        {
            Script.Call("console.group", group);
        }
        
        /// <summary>
        /// Crunch for fast testing.
        /// </summary>
        /// <param name="obj"></param>
        static void SetConsoleObject(object obj)
        {
#if DEBUG
            string typeName=obj.ToDynamic().CSharpObjectType;
            if (typeName == null)
            {
                typeName = obj
                    .GetType()
                    .FullName
                    .Replace(".", "_");
            }
            var name = "obj__"+ typeName;
            Window.Set(name , obj);
#endif
        }

        static void Scope(string group)
        {
            if (_lastGroup != group)
            {
                GroupEnd();
                _lastGroup = group;

                if (group != null)
                {
                    Group(group);
                }
            }

        }


    }
}
