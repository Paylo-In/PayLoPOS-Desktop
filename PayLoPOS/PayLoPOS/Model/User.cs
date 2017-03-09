using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLoPOS.Model
{
    class Wallet
    {
        public string name { get; set; }
        public string display_name { get; set; }
        public string icon { get; set; }
        public string info { get; set; }
        public double cashback { get; set; }
    }

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
        public List<Wallet> wallets { get; set; }

        public string getSelectedOutletName()
        {
            foreach(Outlet o in outlet)
            {
                if(o.id == Properties.Settings.Default.outletId)
                {
                    return o.outlet_name;
                }
            }

            return outlet[0].outlet_name;
        }

    }

    class User
    {
        public int status { get; set; } = 0;
        public string error { get; set; }
        public UserData data { get; set; }
    }
}
