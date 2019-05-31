namespace AvibaWeb.Migrations
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cards",
                c => new
                    {
                        CardId = c.Int(nullable: false, identity: true),
                        Number = c.String(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CardId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Position = c.String(),
                        Photo = c.Binary(),
                        Balance = c.Decimal(nullable: false, storeType: "money"),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CollectionOperations",
                c => new
                    {
                        CollectionOperationId = c.Int(nullable: false, identity: true),
                        CollectionId = c.Int(nullable: false),
                        ProviderId = c.String(maxLength: 128),
                        CollectorId = c.String(maxLength: 128),
                        IsAccepted = c.Boolean(nullable: false),
                        OperationDateTime = c.DateTime(nullable: false),
                        AppUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CollectionOperationId)
                .ForeignKey("dbo.Collections", t => t.CollectionId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.CollectorId)
                .ForeignKey("dbo.AspNetUsers", t => t.ProviderId)
                .ForeignKey("dbo.AspNetUsers", t => t.AppUser_Id)
                .Index(t => t.CollectionId)
                .Index(t => t.ProviderId)
                .Index(t => t.CollectorId)
                .Index(t => t.AppUser_Id);
            
            CreateTable(
                "dbo.Collections",
                c => new
                    {
                        CollectionId = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, storeType: "money"),
                        DeskIssued = c.String(),
                    })
                .PrimaryKey(t => t.CollectionId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AcceptedCollectors",
                c => new
                    {
                        ProviderID = c.String(nullable: false, maxLength: 128),
                        CollectorID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ProviderID, t.CollectorID })
                .ForeignKey("dbo.AspNetUsers", t => t.ProviderID)
                .ForeignKey("dbo.AspNetUsers", t => t.CollectorID)
                .Index(t => t.ProviderID)
                .Index(t => t.CollectorID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CollectionOperations", "AppUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CollectionOperations", "ProviderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CollectionOperations", "CollectorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CollectionOperations", "CollectionId", "dbo.Collections");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Cards", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AcceptedCollectors", "CollectorID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AcceptedCollectors", "ProviderID", "dbo.AspNetUsers");
            DropIndex("dbo.AcceptedCollectors", new[] { "CollectorID" });
            DropIndex("dbo.AcceptedCollectors", new[] { "ProviderID" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.CollectionOperations", new[] { "AppUser_Id" });
            DropIndex("dbo.CollectionOperations", new[] { "CollectorId" });
            DropIndex("dbo.CollectionOperations", new[] { "ProviderId" });
            DropIndex("dbo.CollectionOperations", new[] { "CollectionId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Cards", new[] { "UserId" });
            DropTable("dbo.AcceptedCollectors");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Collections");
            DropTable("dbo.CollectionOperations");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Cards");
        }
    }
}
