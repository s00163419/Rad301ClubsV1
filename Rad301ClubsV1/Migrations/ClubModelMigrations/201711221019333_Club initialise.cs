namespace Rad301ClubsV1.Migrations.ClubModelMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Clubinitialise : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClubEvent",
                c => new
                    {
                        EventID = c.Int(nullable: false, identity: true),
                        Venue = c.String(),
                        Location = c.String(),
                        ClubId = c.Int(nullable: false),
                        StartDateTime = c.DateTime(nullable: false),
                        EndDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EventID)
                .ForeignKey("dbo.Club", t => t.ClubId, cascadeDelete: true)
                .Index(t => t.ClubId);
            
            CreateTable(
                "dbo.Club",
                c => new
                    {
                        ClubId = c.Int(nullable: false, identity: true),
                        ClubName = c.String(),
                        CreationDate = c.DateTime(nullable: false, storeType: "date"),
                        adminID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClubId);
            
            CreateTable(
                "dbo.Member",
                c => new
                    {
                        memberID = c.Int(nullable: false, identity: true),
                        ClubId = c.Int(nullable: false),
                        StudentID = c.String(nullable: false, maxLength: 128),
                        approved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.memberID, t.ClubId, t.StudentID })
                .ForeignKey("dbo.Club", t => t.ClubId, cascadeDelete: true)
                .ForeignKey("dbo.Student", t => t.StudentID, cascadeDelete: true)
                .Index(t => t.ClubId)
                .Index(t => t.StudentID);
            
            CreateTable(
                "dbo.Student",
                c => new
                    {
                        StudentID = c.String(nullable: false, maxLength: 128),
                        Fname = c.String(),
                        Sname = c.String(),
                    })
                .PrimaryKey(t => t.StudentID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClubEvent", "ClubId", "dbo.Club");
            DropForeignKey("dbo.Member", "StudentID", "dbo.Student");
            DropForeignKey("dbo.Member", "ClubId", "dbo.Club");
            DropIndex("dbo.Member", new[] { "StudentID" });
            DropIndex("dbo.Member", new[] { "ClubId" });
            DropIndex("dbo.ClubEvent", new[] { "ClubId" });
            DropTable("dbo.Student");
            DropTable("dbo.Member");
            DropTable("dbo.Club");
            DropTable("dbo.ClubEvent");
        }
    }
}
