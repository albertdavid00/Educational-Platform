namespace PracticaV1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Professors",
                c => new
                    {
                        ProfessorId = c.Int(nullable: false, identity: true),
                        ProfessorLastName = c.String(nullable: false),
                        ProfessorFirstName = c.String(nullable: false),
                        DidacticDegree = c.String(),
                    })
                .PrimaryKey(t => t.ProfessorId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        StudentId = c.Int(nullable: false, identity: true),
                        StudentLastName = c.String(nullable: false),
                        StudentFirstName = c.String(nullable: false),
                        YearOfStudy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StudentId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Students");
            DropTable("dbo.Professors");
        }
    }
}
