using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WisdomBlog.API.Authorization;
using WisdomBlog.API.Services;
using WisdomBlog.Models.RequestModels;

namespace WisdomBlog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private IItemsService _itemsService;

        public AdminController(
            IItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        [AllowAnonymous]
        [HttpGet("GetAllItems")]
        public IActionResult GetAllItems()
        {
            var response = _itemsService.GetAllItems();
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("GetItemById/{id}")]
        public IActionResult GetItemById(int id)
        {
            var response = _itemsService.GetItemById(id);
            return Ok(response);
        }

        [HttpPost("SaveItem")]
        public async Task<IActionResult> SavePost([FromForm] ItemRequest itemRequest)
        {
            var response = await _itemsService.CreatePost(itemRequest);
            return Ok(response);
        }

        [HttpPost("EditPost")]
        public async Task<IActionResult> EditPost([FromForm] ItemRequest itemRequest)
        {
            var response = await _itemsService.EditPost(itemRequest);
            return Ok(response);
        }

        [HttpPost("DeletePost/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var response = await _itemsService.DeleteConfirmed(id);
            return Ok(response);
        }
    }
}
