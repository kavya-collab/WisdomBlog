using Microsoft.EntityFrameworkCore;
using System.Net;
using WisdomBlog.Models.Data;
using WisdomBlog.Models.DBEntities;
using WisdomBlog.Models.RequestModels;
using WisdomBlog.Models.ResponseModels;

namespace WisdomBlog.API.Services
{
    public interface IItemsService
    {
        List<Item> GetAllItems();
        Item GetItemById(int id);
        Task<ApiResponseMessage> CreatePost(ItemRequest itemRequest);
        Task<ApiResponseMessage> EditPost(ItemRequest itemRequest);
        Task<ApiResponseMessage> DeleteConfirmed(int id);
    }

    public class ItemsService : IItemsService
    {
        private WisdomBlogDbContext _context;

        public ItemsService(WisdomBlogDbContext context)
        {
            _context = context;
        }

        public List<Item> GetAllItems()
        {
            return _context.Items.ToList();
        }

        public Item GetItemById(int id)
        {
            return _context.Items.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public async Task<ApiResponseMessage> CreatePost(ItemRequest itemRequest)
        {
            ApiResponseMessage apiResponseMessage = new ApiResponseMessage
            {
                IsSuccess = false
            };
            if (itemRequest != null)
            {
                Item category = new Item
                {
                    Name = itemRequest.Name,
                    Image = itemRequest.ExistingImage,
                    Description = itemRequest.Description
                };

                _context.Items.Add(category);
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "Item Saved Successfully...";
                return apiResponseMessage;
            }
            return apiResponseMessage;
        }

        public async Task<ApiResponseMessage> EditPost(ItemRequest itemRequest)
        {
            ApiResponseMessage apiResponseMessage = new ApiResponseMessage
            {
                IsSuccess = false
            };
            if (itemRequest != null)
            {
                var itemObject = GetItemById(itemRequest.Id);
                Item item = new Item
                {
                    Id = itemObject.Id,
                    Name = itemRequest.Name,
                    Description = itemRequest.Description,
                    Image = itemRequest.ExistingImage,
                };

                _context.Items.Update(item);
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "Item Updated Successfully...";
                return apiResponseMessage;
            }
            return apiResponseMessage;
        }


        public async Task<ApiResponseMessage> DeleteConfirmed(int id)
        {
            ApiResponseMessage apiResponseMessage = new ApiResponseMessage
            {
                IsSuccess = false
            };
            if (id != 0)
            {
                var item = GetItemById(id);
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "Item Deleted Successfully...";
            }
            return apiResponseMessage;
        }

    }
}
