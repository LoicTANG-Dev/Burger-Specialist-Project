using BugerSpecialistAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BugerSpecialistAPI.Controllers
{
    public class Controller : ApiController
    {
        [HttpPost]
        public HttpResponseMessage AddComment()
        {
            var content = new ResponseContent();
            var id = HttpContext.Current.Request.Form["uuid"];
            try
            {
                HttpPostedFile file = HttpContext.Current.Request.Files["file"];

                string userId = HttpContext.Current.Request.Form["userID"];
                string fileName = file.FileName;
                string fileSaveLocation = $"/{userId}/{fileName}";
                byte[] filecontent = new byte[file.InputStream.Length];
                file.InputStream.Read(filecontent, 0, filecontent.Length);
                file.InputStream.Seek(0, SeekOrigin.Begin);

                var workQueue = new Queue<string>();
                workQueue.Enqueue("SaveFile");
                WorkItemManager.Instance.DoWork(new Tuple<Queue<string>, object[]>(workQueue, new object[] { fileSaveLocation, filecontent }));

                content.SetContent(1, string.Empty, HttpStatusCode.OK);
            }
            catch (System.Exception ex)
            {
                content.SetContent(0, ex.Message, HttpStatusCode.InternalServerError);
            }
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(content), System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}