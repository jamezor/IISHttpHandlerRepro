using System;
using System.Text;
using System.Web;
using System.IO;
using System.Globalization;

namespace HttpHandlerWebSite
{
    public class LocationScopedHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.StatusCode = 200;
            context.Response.Write("This is the location scoped handler");
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }

    public class DefaultHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.StatusCode = 200;
            context.Response.Write("This is the default handler");
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
