namespace Observasky.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEmailForUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Email", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Email");
        }
    }
}
