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
    using System.Web;

    public partial class tbl_burger
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_burger()
        {
            this.carts = new HashSet<cart>();
        }
    
        public int BurgerID { get; set; }
        public string BurgerName { get; set; }
        public Nullable<int> typeID { get; set; }
        public decimal UnitPrice { get; set; }
        public string BurgerWeight { get; set; }
        public string ImagePath { get; set; }
        public string Descriptions { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }

        public virtual burger_type burger_type { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cart> carts { get; set; }
    }
}
