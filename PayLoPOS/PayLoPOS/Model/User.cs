﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLoPOS.Model
{
    class UserData
    {
        public string msg { get; set; }
        public string token { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string key { get; set; }
        public long merchant_id { get; set; }
        public List<Outlet> outlet { get; set; }
    }

    class User
    {
        public int status { get; set; } = 0;
        public string error { get; set; }
        public UserData data { get; set; }
    }
}