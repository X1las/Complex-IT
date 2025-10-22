using System;
using System.Linq;
using Xunit;

namespace DataServiceLayer.Tests
{
    public class DataServiceTests
    {
        /* Categories */

        [Fact]
        public void UserObject_HasDefaultValues()
        {
            var user = new Users();
            user.Username = "testuser";
            user.Pswd = "password123";
            Assert.Equal("testuser", user.Username);
            Assert.Equal("password123", user.Pswd);
        }
    }

    
}