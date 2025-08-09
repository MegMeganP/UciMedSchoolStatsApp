Imports System
Imports System.Data.Entity.Migrations
Imports Microsoft.VisualBasic

Namespace Migrations
    Public Partial Class InitialCreate
        Inherits DbMigration
    
        Public Overrides Sub Up()
            CreateTable(
                "dbo.Applications",
                Function(c) New With
                    {
                        .Id = c.Int(nullable := False, identity := True),
                        .SchoolID = c.Int(nullable := False),
                        .NumberApplications = c.Int(),
                        .InStateAppPercent = c.Decimal(precision := 18, scale := 2),
                        .OutOfStateAppPercent = c.Decimal(precision := 18, scale := 2)
                    }) _
                .PrimaryKey(Function(t) t.Id) _
                .ForeignKey("dbo.CaMedSchool", Function(t) t.SchoolID, cascadeDelete := True) _
                .Index(Function(t) t.SchoolID)
            
            CreateTable(
                "dbo.CaMedSchool",
                Function(c) New With
                    {
                        .ID = c.Int(nullable := False, identity := True),
                        .Name = c.String(nullable := False, maxLength := 255)
                    }) _
                .PrimaryKey(Function(t) t.ID)
            
            CreateTable(
                "dbo.Matriculants",
                Function(c) New With
                    {
                        .Id = c.Int(nullable := False, identity := True),
                        .SchoolID = c.Int(nullable := False),
                        .NumberMatriculants = c.Int(),
                        .InStateMatPercent = c.Decimal(precision := 18, scale := 2),
                        .OutOfStateMatPercent = c.Decimal(precision := 18, scale := 2)
                    }) _
                .PrimaryKey(Function(t) t.Id) _
                .ForeignKey("dbo.CaMedSchool", Function(t) t.SchoolID, cascadeDelete := True) _
                .Index(Function(t) t.SchoolID)
            
        End Sub
        
        Public Overrides Sub Down()
            DropForeignKey("dbo.Applications", "SchoolID", "dbo.CaMedSchool")
            DropForeignKey("dbo.Matriculants", "SchoolID", "dbo.CaMedSchool")
            DropIndex("dbo.Matriculants", New String() { "SchoolID" })
            DropIndex("dbo.Applications", New String() { "SchoolID" })
            DropTable("dbo.Matriculants")
            DropTable("dbo.CaMedSchool")
            DropTable("dbo.Applications")
        End Sub
    End Class
End Namespace
