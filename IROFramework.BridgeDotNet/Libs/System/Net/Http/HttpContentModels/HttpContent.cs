using System.Threading.Tasks;

namespace System.Net.Http
{
    public abstract class HttpContent
    {
        public string ContentType { get; set; }

        public async Task<string> ReadAsStringAsync()
        {
            return ReadAsString();
        }

        public string ReadAsString()
        {
            var content = GetContent();
            if (content is string strContent)
            {
                return strContent;
            }
            else
            {
                throw new Exception("HttpContent can't be read as string.");
            }
        }


        public abstract object GetContent();
    }

    public class HttpContent<TContent>: HttpContent
    {
        public TContent Content { get; set; }

        public override object GetContent()
        {
            return Content;
        }
    }
}