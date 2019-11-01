namespace MvcCF5.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Keywords = c.String(),
                        Description = c.String(),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TypeId = c.Int(nullable: false),
                        SuppId = c.Int(nullable: false),
                        CatId = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                        Price = c.Single(nullable: false),
                        SellPrice = c.Single(nullable: false),
                        Weight = c.Single(nullable: false),
                        Image = c.String(),
                        Detail = c.String(),
                        Date = c.DateTime(),
                        Status = c.Boolean(nullable: false),
                        Categories_Id = c.Int(),
                        Suppliers_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.Categories_Id)
                .ForeignKey("dbo.Supplier", t => t.Suppliers_Id)
                .Index(t => t.Categories_Id)
                .Index(t => t.Suppliers_Id);
            
            CreateTable(
                "dbo.Supplier",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ProdId = c.Int(nullable: false),
                        Location = c.Double(nullable: false),
                        Image = c.String(),
                        Detail = c.String(),
                        Date = c.DateTime(),
                        Status = c.Boolean(nullable: false),
                        Supplier_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Supplier", t => t.Supplier_Id)
                .Index(t => t.Supplier_Id);
            
            CreateTable(
                "dbo.Message",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        Subject = c.String(),
                        Messages = c.String(),
                        Comment = c.String(),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Setting",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Keywords = c.String(),
                        Description = c.String(),
                        Title = c.String(),
                        Company = c.String(),
                        Address = c.String(),
                        Email = c.String(),
                        AboutUs = c.String(),
                        Contact = c.String(),
                        Referances = c.String(),
                        Status = c.String(),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        ConfirmPasword = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Product", "Suppliers_Id", "dbo.Supplier");
            DropForeignKey("dbo.Supplier", "Supplier_Id", "dbo.Supplier");
            DropForeignKey("dbo.Product", "Categories_Id", "dbo.Category");
            DropIndex("dbo.Supplier", new[] { "Supplier_Id" });
            DropIndex("dbo.Product", new[] { "Suppliers_Id" });
            DropIndex("dbo.Product", new[] { "Categories_Id" });
            DropTable("dbo.UserAccount");
            DropTable("dbo.Setting");
            DropTable("dbo.Message");
            DropTable("dbo.Supplier");
            DropTable("dbo.Product");
            DropTable("dbo.Category");
        }
    }
}
