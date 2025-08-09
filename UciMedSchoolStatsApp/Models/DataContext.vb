Imports System.Data.Entity

Public Class DataContext
    Inherits DbContext

    Public Sub New()
        MyBase.New("name=UciMedSchoolStatsDb") ' use the Web.config connection string
    End Sub

    Public Property Schools As DbSet(Of CaMedSchool)
    Public Property Applications As DbSet(Of ApplicationStat)
    Public Property Matriculants As DbSet(Of MatriculantStat)

    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
        MyBase.OnModelCreating(modelBuilder)
        ' map the models to the tables
        modelBuilder.Entity(Of CaMedSchool)().ToTable("CaMedSchool")
        modelBuilder.Entity(Of ApplicationStat)().ToTable("Applications")
        modelBuilder.Entity(Of MatriculantStat)().ToTable("Matriculants")
    End Sub
End Class

