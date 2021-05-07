namespace MHS_FINAL_PROJECT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class database : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.drug_active_name",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50),
                        description = c.String(nullable: false),
                        Add_By_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.User", t => t.Add_By_Id)
                .Index(t => t.name, unique: true)
                .Index(t => t.Add_By_Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Gender = c.String(nullable: false),
                        age = c.Int(nullable: false),
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
                "dbo.UserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserLogin",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.drug_dosage",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        how_to_take = c.String(nullable: false, maxLength: 50),
                        number_of_dosage = c.String(nullable: false, maxLength: 100),
                        dosage = c.String(nullable: false),
                        beginning_of_effectiveness = c.String(nullable: false, maxLength: 50),
                        duration_of_effectiveness = c.String(nullable: false, maxLength: 50),
                        feed = c.String(nullable: false, maxLength: 50),
                        Storage_and_preservation = c.String(nullable: false, maxLength: 100),
                        Forget_a_dose = c.String(nullable: false),
                        overdose = c.String(nullable: false),
                        drug_active_name_id = c.Int(nullable: false),
                        drug_active_name_id1 = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.drug_active_name", t => t.drug_active_name_id1)
                .Index(t => t.drug_active_name_id1);
            
            CreateTable(
                "dbo.drug_trade_name",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        trade_name = c.String(nullable: false, maxLength: 50),
                        Manufacturer_company = c.String(nullable: false, maxLength: 50),
                        drug_active_nameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.drug_active_name", t => t.drug_active_nameId, cascadeDelete: true)
                .Index(t => t.drug_active_nameId);
            
            CreateTable(
                "dbo.side_effect",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 50),
                        prevalence_effect = c.String(nullable: false, maxLength: 50),
                        inform_doctor = c.String(nullable: false, maxLength: 50),
                        drug_active_nameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.drug_active_name", t => t.drug_active_nameId, cascadeDelete: true)
                .Index(t => t.drug_active_nameId);
            
            CreateTable(
                "dbo.warnings",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        During_pregnancy = c.String(nullable: false, maxLength: 200),
                        Breast_feeding = c.String(nullable: false, maxLength: 200),
                        Children_and_infants = c.String(nullable: false, maxLength: 200),
                        Elderly = c.String(nullable: false, maxLength: 100),
                        Driving = c.String(nullable: false, maxLength: 100),
                        Surgery_and_anesthesia = c.String(nullable: false, maxLength: 100),
                        drug_active_nameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.drug_active_name", t => t.drug_active_nameId, cascadeDelete: true)
                .Index(t => t.drug_active_nameId);
            
            CreateTable(
                "dbo.Drug_Interaction",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Degree = c.String(nullable: false, maxLength: 25),
                        add_by = c.String(),
                        drug_id = c.Int(nullable: false),
                        with_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.drug_active_name", t => t.drug_id, cascadeDelete: false)
                .ForeignKey("dbo.drug_active_name", t => t.with_id, cascadeDelete: true)
                .Index(t => t.drug_id)
                .Index(t => t.with_id);
            
            CreateTable(
                "dbo.health_drugs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        dosage = c.String(nullable: false),
                        user_healthId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.user_health", t => t.user_healthId, cascadeDelete: true)
                .Index(t => t.user_healthId);
            
            CreateTable(
                "dbo.user_health",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        health_condition_name = c.String(nullable: false),
                        description = c.String(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.pharmacists",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        University_Name = c.String(nullable: false),
                        Degree = c.String(nullable: false),
                        certification_img = c.String(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.pharmacists", "User_Id", "dbo.User");
            DropForeignKey("dbo.user_health", "User_Id", "dbo.User");
            DropForeignKey("dbo.health_drugs", "user_healthId", "dbo.user_health");
            DropForeignKey("dbo.Drug_Interaction", "with_id", "dbo.drug_active_name");
            DropForeignKey("dbo.Drug_Interaction", "drug_id", "dbo.drug_active_name");
            DropForeignKey("dbo.warnings", "drug_active_nameId", "dbo.drug_active_name");
            DropForeignKey("dbo.side_effect", "drug_active_nameId", "dbo.drug_active_name");
            DropForeignKey("dbo.drug_trade_name", "drug_active_nameId", "dbo.drug_active_name");
            DropForeignKey("dbo.drug_dosage", "drug_active_name_id1", "dbo.drug_active_name");
            DropForeignKey("dbo.drug_active_name", "Add_By_Id", "dbo.User");
            DropForeignKey("dbo.UserRole", "UserId", "dbo.User");
            DropForeignKey("dbo.UserLogin", "UserId", "dbo.User");
            DropForeignKey("dbo.UserClaim", "UserId", "dbo.User");
            DropIndex("dbo.Role", "RoleNameIndex");
            DropIndex("dbo.pharmacists", new[] { "User_Id" });
            DropIndex("dbo.user_health", new[] { "User_Id" });
            DropIndex("dbo.health_drugs", new[] { "user_healthId" });
            DropIndex("dbo.Drug_Interaction", new[] { "with_id" });
            DropIndex("dbo.Drug_Interaction", new[] { "drug_id" });
            DropIndex("dbo.warnings", new[] { "drug_active_nameId" });
            DropIndex("dbo.side_effect", new[] { "drug_active_nameId" });
            DropIndex("dbo.drug_trade_name", new[] { "drug_active_nameId" });
            DropIndex("dbo.drug_dosage", new[] { "drug_active_name_id1" });
            DropIndex("dbo.UserRole", new[] { "RoleId" });
            DropIndex("dbo.UserRole", new[] { "UserId" });
            DropIndex("dbo.UserLogin", new[] { "UserId" });
            DropIndex("dbo.UserClaim", new[] { "UserId" });
            DropIndex("dbo.User", "UserNameIndex");
            DropIndex("dbo.drug_active_name", new[] { "Add_By_Id" });
            DropIndex("dbo.drug_active_name", new[] { "name" });
            DropTable("dbo.Role");
            DropTable("dbo.pharmacists");
            DropTable("dbo.user_health");
            DropTable("dbo.health_drugs");
            DropTable("dbo.Drug_Interaction");
            DropTable("dbo.warnings");
            DropTable("dbo.side_effect");
            DropTable("dbo.drug_trade_name");
            DropTable("dbo.drug_dosage");
            DropTable("dbo.UserRole");
            DropTable("dbo.UserLogin");
            DropTable("dbo.UserClaim");
            DropTable("dbo.User");
            DropTable("dbo.drug_active_name");
        }
    }
}
