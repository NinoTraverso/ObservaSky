namespace Observasky.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPHotoForUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Photo", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Photo");
        }
    }
}
