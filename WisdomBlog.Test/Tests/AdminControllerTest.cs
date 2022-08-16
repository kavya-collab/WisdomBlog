using WisdomBlog.API.Controllers;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using WisdomBlog.API.Services;
using AutoMapper;
using WisdomBlog.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.IO;

namespace WisdomBlog.Test.Tests
{
    public class AdminControllerTest
    {
        [Fact]
        public async void AdminController_DeletePost_Success_Test()
        {
            var mockItemService = new Mock<IItemsService>();

            var controller = new AdminController(mockItemService.Object);
            int id = 1;
            var result = await controller.DeletePost(id);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async void AdminController_DeletePost_Failure_Test()
        {
            var mockItemService = new Mock<IItemsService>();

            var controller = new AdminController(mockItemService.Object);
            int id = 0;
            var result = await controller.DeletePost(id);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public void AdminController_GetItemById_Success_Test()
        {
            var mockItemService = new Mock<IItemsService>();

            var controller = new AdminController(mockItemService.Object);
            int id = 1;
            var result = controller.GetItemById(id);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public void AdminController_GetItemById_0_Test()
        {
            var mockItemService = new Mock<IItemsService>();

            var controller = new AdminController(mockItemService.Object);
            int id = 0;
            var result = controller.GetItemById(id);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async void AdminController_SaveItem_Success_Test()
        {
            var mockItemService = new Mock<IItemsService>();

            var controller = new AdminController(mockItemService.Object);
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt");
            ItemRequest itemRequest = new ItemRequest
            {
                Id = 0,
                Name = "test book",
                Description = "test",
                Image = file
            };
            var result = await controller.SavePost(itemRequest);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async void AdminController_SaveItem_EmptyObject_Test()
        {
            var mockItemService = new Mock<IItemsService>();

            var controller = new AdminController(mockItemService.Object);
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt");
            ItemRequest itemRequest = new ItemRequest();
            var result = await controller.SavePost(itemRequest);
            var okResult = result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async void AdminController_SavePost_MissedProps_Test()
        {
            var mockItemService = new Mock<IItemsService>();

            var controller = new AdminController(mockItemService.Object);
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt");
            ItemRequest itemRequest = new ItemRequest
            {
                Id = 0,
                Image = file
            };
            var result = controller.SavePost(itemRequest);
            var okResult = await result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }
    }
}
