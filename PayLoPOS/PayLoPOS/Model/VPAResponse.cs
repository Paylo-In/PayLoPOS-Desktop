using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLoPOS.Model
{
    class VPAResponse
    {
        public int status { get; set; }
        public List<string> data { get; set; }
    }
}
