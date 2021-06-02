using System.Collections.Generic;
using Bridge.Html5;

namespace System.Net.Http
{
    public class FormUrlEncodedContent : HttpContent<FormData>
    {
        public FormUrlEncodedContent(FormData formData)
        {
            Content = formData;
            ContentType = "application/x-www-form-urlencoded";
        }

        public FormUrlEncodedContent(IDictionary<string, string> formDataDict)
        {
            Content = new FormData();
            foreach (var item in formDataDict)
            {
                Content.Append(item.Key, item.Value);
            }
            ContentType = "application/x-www-form-urlencoded";
        }
    }
}