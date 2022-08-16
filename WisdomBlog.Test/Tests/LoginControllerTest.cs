using WisdomBlog.API.Controllers;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using WisdomBlog.API.Services;
using AutoMapper;
using WisdomBlog.Models.RequestModels;
using Microsoft.AspNetCore.Http;

namespace WisdomBlog.Test.Tests
{
    public class LoginControllerTest
    {
        [Fact]
        public void LoginController_AdminLoginSuccess_Test()
        {
            var mockUserService = new Mock<IUserService>();

            var controller = new LoginController(mockUserService.Object);

            var authenticateRequest = new LoginRequest
            {
                Username = "AdminUser",
                Password = "123123"
            };
            var result = controller.UserAuthenticate(authenticateRequest);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public void LoginController_AdminLogin_MissingUserneme_Test()
        {
            var mockUserService = new Mock<IUserService>();

            var controller = new LoginController(mockUserService.Object);

            var authenticateRequest = new LoginRequest
            {
                Password = "123123"
            };
            var result = controller.UserAuthenticate(authenticateRequest);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }
        [Fact]
        public void LoginController_AdminLogin_MissingPassword_Test()
        {
            var mockUserService = new Mock<IUserService>();

            var controller = new LoginController(mockUserService.Object);

            var authenticateRequest = new LoginRequest
            {
                Username = "AdminBookStore1",
            };
            var result = controller.UserAuthenticate(authenticateRequest);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }
        [Fact]
        public void LoginController_AdminLogin_EmptyUsernamePassword_Test()
        {
            var mockUserService = new Mock<IUserService>();

            var controller = new LoginController(mockUserService.Object);

            var authenticateRequest = new LoginRequest();

            var result = controller.UserAuthenticate(authenticateRequest);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }
        [Fact]
        public void LoginController_AdminLogin_InvalidCredentials_Test()
        {
            var mockUserService = new Mock<IUserService>();

            var controller = new LoginController(mockUserService.Object);

            var authenticateRequest = new LoginRequest
            {
                Username = "test",
                Password = "test"
            };
            var result = controller.UserAuthenticate(authenticateRequest);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

    }
}
