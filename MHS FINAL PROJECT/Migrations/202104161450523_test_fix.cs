namespace MHS_FINAL_PROJECT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test_fix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.drug_dosage", "drug_active_name_id1", "dbo.drug_active_name");
            DropIndex("dbo.drug_dosage", new[] { "drug_active_name_id1" });
            DropColumn("dbo.drug_dosage", "drug_active_name_id");
            RenameColumn(table: "dbo.drug_dosage", name: "drug_active_name_id1", newName: "drug_active_name_id");
            AlterColumn("dbo.drug_dosage", "drug_active_name_id", c => c.Int(nullable: false));
            CreateIndex("dbo.drug_dosage", "drug_active_name_id");
            AddForeignKey("dbo.drug_dosage", "drug_active_name_id", "dbo.drug_active_name", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.drug_dosage", "drug_active_name_id", "dbo.drug_active_name");
            DropIndex("dbo.drug_dosage", new[] { "drug_active_name_id" });
            AlterColumn("dbo.drug_dosage", "drug_active_name_id", c => c.Int());
            RenameColumn(table: "dbo.drug_dosage", name: "drug_active_name_id", newName: "drug_active_name_id1");
            AddColumn("dbo.drug_dosage", "drug_active_name_id", c => c.Int(nullable: false));
            CreateIndex("dbo.drug_dosage", "drug_active_name_id1");
            AddForeignKey("dbo.drug_dosage", "drug_active_name_id1", "dbo.drug_active_name", "id");
        }
    }
}
