using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using DBModels;

namespace BlogTests
{
    public class BasicTests
        : IClassFixture<WebApplicationFactory<Blog.Startup>>
    {
        private readonly WebApplicationFactory<Blog.Startup> _factory;

        public BasicTests(WebApplicationFactory<Blog.Startup> factory)
        {
            _factory = factory;

            var db = getInMemoryDataContext();
            var testUser = new User(new Profile(new DateTime(2019, 10, 1)), new ProfileStatus(ProfileState.ACTIVE));
            db.Users.Add(testUser);
            var testPost = new Post(new DateTime(2019, 10, 3), testUser, "Just a test post", "Post body");
            db.Posts.Add(testPost);
            db.SaveChanges();
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Index")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/Profile?=0")]
        [InlineData("/Post?=0")]
        public async Task Get_PublicResources(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        BlogContext getInMemoryDataContext()
        {
            DbContextOptions<BlogContext> options;
            var builder = new DbContextOptionsBuilder<BlogContext>();
            builder.UseInMemoryDatabase("BlogDB");
            options = builder.Options;
            BlogContext personDataContext = new BlogContext(options);
            personDataContext.Database.EnsureDeleted();
            personDataContext.Database.EnsureCreated();

            return personDataContext;
        }
    }
}
