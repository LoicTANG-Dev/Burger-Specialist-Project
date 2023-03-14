using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace BugerSpecialistAPI.Models
{
    public class ResponseContent
    {
        // 0 => failed; 1 => succeed
        public int Status { get; private set; }
        public string Info { get; private set; }
        public HttpStatusCode Code { get; private set; }
        public IList<object> Data { get; private set; }


        public ResponseContent()
        {
            Status = 0;
            Info = "";
            Data = new List<Object>();
            Code = 0;
        }

        public void SetContent(int status, string info, HttpStatusCode code, IList<Object> data = null)
        {
            Status = status;
            Info = info;
            Code = code;
            if (data == null)
                Data = new List<Object>();
            else
                Data = data;
        }
    }
}