namespace Observasky.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NoLimitsToLectureDescriptionLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Lectures", "Description", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Lectures", "Description", c => c.String(maxLength: 200));
        }
    }
}
