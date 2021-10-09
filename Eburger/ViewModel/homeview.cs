using Eburger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eburger.ViewModel
{
    public class homeview
    {
        public IEnumerable<tbl_burger> burgers { get; set; }
        public IEnumerable<burger_type> burgerTypes { get; set; }
        public IEnumerable<burger_type> menu { get; set; }
        public IEnumerable<cart> carts { get; set; }
        public IEnumerable<order> ord { get; set; }
        public IEnumerable<order> ordlastthree { get; set; }
        public IEnumerable<order> ordYear { get; set; }
    }
}