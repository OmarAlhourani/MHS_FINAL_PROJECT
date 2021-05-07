namespace MHS_FINAL_PROJECT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixsize : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.drug_dosage", "how_to_take", c => c.String(nullable: false));
            AlterColumn("dbo.drug_dosage", "number_of_dosage", c => c.String(nullable: false));
            AlterColumn("dbo.drug_dosage", "beginning_of_effectiveness", c => c.String(nullable: false));
            AlterColumn("dbo.drug_dosage", "duration_of_effectiveness", c => c.String(nullable: false));
            AlterColumn("dbo.drug_dosage", "feed", c => c.String(nullable: false));
            AlterColumn("dbo.drug_dosage", "Storage_and_preservation", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.drug_dosage", "Storage_and_preservation", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.drug_dosage", "feed", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.drug_dosage", "duration_of_effectiveness", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.drug_dosage", "beginning_of_effectiveness", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.drug_dosage", "number_of_dosage", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.drug_dosage", "how_to_take", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
