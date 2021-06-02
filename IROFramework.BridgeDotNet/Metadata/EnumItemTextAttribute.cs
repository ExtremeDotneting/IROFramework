using System;

namespace Libs.Metadata
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumItemTextAttribute : Attribute
    {
        public EnumItemTextAttribute(string text)
        {
            Text = text;
        }

        public string Text { get; }
        
    }
}
