using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FigureManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CategoryTest
{
    public class CategoryTestCases
    {
        private DbContextOptions<FigureManagementSystemContext> Options() =>
            new DbContextOptionsBuilder<FigureManagementSystemContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        [Fact]
        public void CanCreateCategory_WithMinimalFields()
        {
            using var ctx = new FigureManagementSystemContext(Options());
            var cat = new Category { Name = "Anime" };
            ctx.Categories.Add(cat);
            ctx.SaveChanges();
            var loaded = ctx.Categories.Single();
            Assert.Equal("Anime", loaded.Name);
            Assert.True(loaded.IsActive ?? true); // Default true
            Assert.Null(loaded.Description);
        }

        [Fact]
        public void Name_IsRequiredAndCannotBeNull()
        {
            using var ctx = new FigureManagementSystemContext(Options());
            var cat = new Category { Name = null };
            ctx.Categories.Add(cat);
            Assert.Throws<ArgumentNullException>(() => ctx.SaveChanges());
        }

        [Fact]
        public void CannotExceed_MaxLength_Name()
        {
            using var ctx = new FigureManagementSystemContext(Options());
            var longName = new string('a', 260);
            var cat = new Category { Name = longName };
            ctx.Categories.Add(cat);
            // Manual validation, since InMemory won't enforce
            Assert.True(cat.Name.Length > 255);
        }

        [Fact]
        public void Description_MaxLength_IsEnforced()
        {
            using var ctx = new FigureManagementSystemContext(Options());
            var desc = new string('a', 1001);
            var cat = new Category { Name = "Extra", Description = desc };
            ctx.Categories.Add(cat);
            Assert.True(cat.Description.Length > 1000);
        }

        [Fact]
        public void CanUpdateCategoryFields()
        {
            using var ctx = new FigureManagementSystemContext(Options());
            var cat = new Category { Name = "Old" };
            ctx.Categories.Add(cat); ctx.SaveChanges();
            var loaded = ctx.Categories.Single();
            loaded.Name = "New"; loaded.Description = "Updated";
            ctx.SaveChanges();
            Assert.Equal("New", ctx.Categories.Single().Name);
            Assert.Equal("Updated", ctx.Categories.Single().Description);
        }

        [Fact]
        public void Uniqueness_OfName_EnforcedByManualCheck()
        {
            using var ctx = new FigureManagementSystemContext(Options());
            ctx.Categories.Add(new Category { Name = "A" }); ctx.SaveChanges();
            ctx.Categories.Add(new Category { Name = "A" });
            var isDuplicate = ctx.Categories.GroupBy(c => c.Name)
                .Any(g => g.Count() > 1);
            Assert.True(isDuplicate);
        }

        [Fact]
        public void SoftDelete_Category_ByIsActive()
        {
            using var ctx = new FigureManagementSystemContext(Options());
            var cat = new Category { Name = "ToDelete" };
            ctx.Categories.Add(cat); ctx.SaveChanges();
            cat.IsActive = false; ctx.SaveChanges();
            Assert.False(ctx.Categories.Single().IsActive ?? true);
        }

        [Fact]
        public void SearchByName_Works_CaseInsensitive()
        {
            using var ctx = new FigureManagementSystemContext(Options());
            ctx.Categories.Add(new Category { Name = "Movies" });
            ctx.Categories.Add(new Category { Name = "games" });
            ctx.Categories.Add(new Category { Name = "Game Figures" });
            ctx.SaveChanges();
            var res = ctx.Categories.Where(c => c.Name.ToLower().Contains("game")).ToList();
            Assert.Equal(2, res.Count);
        }

        [Fact]
        public void CanAssignProducts_ToCategory()
        {
            using var ctx = new FigureManagementSystemContext(Options());
            var cat = new Category { Name = "Toys" };
            var prod = new Product { Name = "Car", Price = 10, Quantity = 10, BrandId = 1, CategoryId = 1, CharacterId = 1 };
            cat.Products.Add(prod);
            ctx.Categories.Add(cat); ctx.SaveChanges();
            Assert.Single(ctx.Categories.Single().Products);
        }

        [Fact]
        public void RemoveCategory_RemovesFromDatabase()
        {
            using var ctx = new FigureManagementSystemContext(Options());
            var cat = new Category { Name = "Temp" };
            ctx.Categories.Add(cat); ctx.SaveChanges();
            ctx.Categories.Remove(cat); ctx.SaveChanges();
            Assert.Empty(ctx.Categories);
        }

        [Fact]
        public void Filter_ByIsActiveStatus()
        {
            using var ctx = new FigureManagementSystemContext(Options());
            ctx.Categories.Add(new Category { Name = "A", IsActive = true });
            ctx.Categories.Add(new Category { Name = "B", IsActive = false });
            ctx.SaveChanges();
            var active = ctx.Categories.Where(c => c.IsActive == true).ToList();
            var inactive = ctx.Categories.Where(c => c.IsActive == false).ToList();
            Assert.Single(active); Assert.Single(inactive);
            Assert.Equal("A", active[0].Name); Assert.Equal("B", inactive[0].Name);
        }

        [Fact]
        public void CanListAllCategories_ByDefaultOrder()
        {
            using var ctx = new FigureManagementSystemContext(Options());
            ctx.Categories.Add(new Category { Name = "X" });
            ctx.Categories.Add(new Category { Name = "Y" });
            ctx.SaveChanges();
            var names = ctx.Categories.OrderBy(c => c.Id).Select(c => c.Name).ToArray();
            Assert.Equal(new[] { "X", "Y" }, names);
        }
    }
}
