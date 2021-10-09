namespace Eburger.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class useerdata : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "fName", c => c.String());
            AddColumn("dbo.AspNetUsers", "lName", c => c.String());
            AddColumn("dbo.AspNetUsers", "uAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "uAddress");
            DropColumn("dbo.AspNetUsers", "lName");
            DropColumn("dbo.AspNetUsers", "fName");
        }
    }
}
