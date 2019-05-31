namespace AvibaWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CollectionPaymentType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Collections", "PaymentType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Collections", "PaymentType");
        }
    }
}
