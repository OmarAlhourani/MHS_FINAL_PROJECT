namespace MHS_FINAL_PROJECT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix_size : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.drug_dosage", "beginning_of_effectiveness", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.drug_dosage", "Storage_and_preservation", c => c.String(nullable: false, maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.drug_dosage", "Storage_and_preservation", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.drug_dosage", "beginning_of_effectiveness", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
