namespace System.Net.Http
{
    public class StringContent : HttpContent<string>
    {
        public StringContent(string stringsContent, string contentType = "text/plain")
        {
            Content = stringsContent;
            ContentType = contentType;
        }
    }
}