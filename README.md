===================================================================

These are the Nuget Packages. YOU MUST INSTALL "Microsoft.AspNetCore.Mvc.ViewFeatures".

Microsoft.AspNetCore.Mvc.ViewFeatures


Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore


Microsoft.AspNetCore.Identity.EntityFrameworkCore


Microsoft.AspNetCore.Identity.UI


Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation


Microsoft.EntityFrameworkCore.SqlServer


Microsoft.EntityFrameworkCore.Tools


Microsoft.VisualStudio.Web.CodeGeneration.Design

=================================================================

INSERT SOME DATA INTO "Categories"

Use MMLHandicraftsDB;
Go

-- Inserting 5 sample handicraft items into the Categories table
INSERT INTO [MMLHandicraftsDB].[dbo].[Categories] ([Name]) VALUES
('Handwoven Baskets'), ('Wooden Sculptures'), ('Pottery and Ceramics'), ('Metal Jewelry'), ('Textile Art')
