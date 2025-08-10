Imports System
Imports System.Data.Entity.Migrations
Imports Microsoft.VisualBasic

Namespace Migrations
    Public Partial Class AddFeatureREquests
        Inherits DbMigration
    
        Public Overrides Sub Up()
            CreateTable(
                "dbo.FeatureRequests",
                Function(c) New With
                    {
                        .Id = c.Int(nullable := False, identity := True),
                        .Description = c.String(nullable := False, maxLength := 2000),
                        .RequesterName = c.String(nullable := False, maxLength := 200),
                        .RequestedAt = c.DateTime(nullable := False),
                        .IsCompleted = c.Boolean(nullable := False)
                    }) _
                .PrimaryKey(Function(t) t.Id)
            
        End Sub
        
        Public Overrides Sub Down()
            DropTable("dbo.FeatureRequests")
        End Sub
    End Class
End Namespace
