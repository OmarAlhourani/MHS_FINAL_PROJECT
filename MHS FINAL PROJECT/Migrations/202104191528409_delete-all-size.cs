namespace MHS_FINAL_PROJECT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteallsize : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.drug_trade_name", "trade_name", c => c.String(nullable: false));
            AlterColumn("dbo.drug_trade_name", "Manufacturer_company", c => c.String(nullable: false));
            AlterColumn("dbo.side_effect", "name", c => c.String(nullable: false));
            AlterColumn("dbo.side_effect", "prevalence_effect", c => c.String(nullable: false));
            AlterColumn("dbo.side_effect", "inform_doctor", c => c.String(nullable: false));
            AlterColumn("dbo.warnings", "During_pregnancy", c => c.String(nullable: false));
            AlterColumn("dbo.warnings", "Breast_feeding", c => c.String(nullable: false));
            AlterColumn("dbo.warnings", "Children_and_infants", c => c.String(nullable: false));
            AlterColumn("dbo.warnings", "Elderly", c => c.String(nullable: false));
            AlterColumn("dbo.warnings", "Driving", c => c.String(nullable: false));
            AlterColumn("dbo.warnings", "Surgery_and_anesthesia", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.warnings", "Surgery_and_anesthesia", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.warnings", "Driving", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.warnings", "Elderly", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.warnings", "Children_and_infants", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.warnings", "Breast_feeding", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.warnings", "During_pregnancy", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.side_effect", "inform_doctor", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.side_effect", "prevalence_effect", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.side_effect", "name", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.drug_trade_name", "Manufacturer_company", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.drug_trade_name", "trade_name", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
