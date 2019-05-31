namespace AvibaWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CollectionOperations", "AppUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.CollectionOperations", new[] { "AppUser_Id" });
            AddColumn("dbo.AspNetUsers", "IsActive", c => c.Boolean(nullable: false, defaultValue:true));
            DropColumn("dbo.CollectionOperations", "AppUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CollectionOperations", "AppUser_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.AspNetUsers", "IsActive");
            CreateIndex("dbo.CollectionOperations", "AppUser_Id");
            AddForeignKey("dbo.CollectionOperations", "AppUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
