//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Eburger.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class cart
    {
        public int cartID { get; set; }
        public int BurgerID { get; set; }
        public string userID { get; set; }
        public int quantity { get; set; }
        public decimal totamount { get; set; }
        public bool cartStatus { get; set; }
        public bool orderStatus { get; set; }
        public int orderNo { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual tbl_burger tbl_burger { get; set; }
    }
}