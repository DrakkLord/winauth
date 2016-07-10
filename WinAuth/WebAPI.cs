using SimpleWebServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinAuth
{
    class WebAPI
    {
        private WebServer ws;
        private WinAuthForm wform;

        public WebAPI(WinAuthForm wf) {
            wform = wf;
        }

        public void Start() {

            ws = new WebServer(SendResponse, "http://localhost:8080/getcode/");
            ws.Run();
        }

        public void Stop() {
            ws.Stop();
        }

        public string SendResponse(HttpListenerRequest request)
        {
            if (request.HttpMethod != "POST" || !request.IsLocal || request.ContentType != "application/x-www-form-urlencoded")
            {
                return null;
            }

            string documentContents;
            using (Stream receiveStream = request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            string[] words = documentContents.Split('=');
            if (words.Length < 2)
            {
                return null;
            }

            string requestUserName = null;
            if (words[0] == "username")
            {
                requestUserName = words[1];
            }
            if (requestUserName == null)
            {
                return null;
            }

            string code = null;
            wform.OnWebGetCode(requestUserName, (resultCode) => {
                code = resultCode;
            });
            if (code == null) {
                return null;
            }

            return string.Format("{{\"code\":\"{0}\"}}", code);
        }
    }
}
