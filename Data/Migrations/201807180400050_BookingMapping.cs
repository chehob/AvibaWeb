namespace AvibaWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookingMapping : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "BookingMappingId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "BookingMappingId");
        }
    }
}
