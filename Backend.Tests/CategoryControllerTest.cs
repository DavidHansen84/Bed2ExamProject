using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using OnlineClinicBooking.Data;
using OnlineClinicBooking.Models;
using OnlineClinicBooking.Controllers;
using Xunit;

namespace Backend.Test
{
    public class CategoryControllerTest
    {
        [Fact]
        public async Task GetCategoriesTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new DataContext(options);

            // Seed test data
            context.Categories.AddRange(
                new Category { Id = 1, Name = "Emergency" },
                new Category { Id = 2, Name = "Check up" },
                new Category { Id = 3, Name = "Follow up" },
                new Category { Id = 4, Name = "Diagnosis" }
            );
            await context.SaveChangesAsync();

            var controller = new CategoryController(context);

            // Act
            var result = await controller.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var categories = Assert.IsType<List<Category>>(okResult.Value);
            Assert.Equal(4, categories.Count);
        }


    }
}
