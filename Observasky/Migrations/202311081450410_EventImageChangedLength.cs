namespace Observasky.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventImageChangedLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Events", "Photo", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Events", "Photo", c => c.String(maxLength: 255));
        }
    }
}
