namespace Observasky.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        IdArticle = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 50),
                        Introduction = c.String(),
                        Photo = c.String(maxLength: 255),
                        Main = c.String(),
                        Conclusions = c.String(),
                        Author = c.String(maxLength: 100),
                        Date = c.DateTime(storeType: "date"),
                    })
                .PrimaryKey(t => t.IdArticle);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        IdEvent = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Photo = c.String(maxLength: 255),
                        Date = c.DateTime(storeType: "date"),
                    })
                .PrimaryKey(t => t.IdEvent);
            
            CreateTable(
                "dbo.Glossary",
                c => new
                    {
                        IdGlossary = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.IdGlossary);
            
            CreateTable(
                "dbo.Guests",
                c => new
                    {
                        IdBooking = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Surname = c.String(maxLength: 50),
                        NumberOfGuests = c.Int(),
                        LectureID = c.Int(),
                    })
                .PrimaryKey(t => t.IdBooking)
                .ForeignKey("dbo.Lectures", t => t.LectureID)
                .Index(t => t.LectureID);
            
            CreateTable(
                "dbo.Lectures",
                c => new
                    {
                        IdLecture = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        DateTime = c.DateTime(),
                        Description = c.String(maxLength: 200),
                        Photo = c.String(maxLength: 255),
                        Seats = c.Int(),
                        Speakers = c.String(maxLength: 100),
                        Topic = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.IdLecture);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        IdUser = c.Int(nullable: false, identity: true),
                        Username = c.String(maxLength: 50),
                        Password = c.String(maxLength: 50),
                        Role = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.IdUser);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Guests", "LectureID", "dbo.Lectures");
            DropIndex("dbo.Guests", new[] { "LectureID" });
            DropTable("dbo.Users");
            DropTable("dbo.Lectures");
            DropTable("dbo.Guests");
            DropTable("dbo.Glossary");
            DropTable("dbo.Events");
            DropTable("dbo.Articles");
        }
    }
}
