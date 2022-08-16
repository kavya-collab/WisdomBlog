using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WisdomBlog.Models.DBEntities;
using WisdomBlog.Models.RequestModels;
using WisdomBlog.Models.ResponseModels;

namespace WisdomBlog.UI.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IConfiguration _Configure;
        private static string apiBaseUrl;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ILogger<AdminController> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _Configure = configuration;
            _webHostEnvironment = webHostEnvironment;
            apiBaseUrl = _Configure.GetValue<string>("WebAPIBaseUrl");
        }


        public async Task<IActionResult> Index()
        {
            var itemsList = await GetAllItems();
            return View(itemsList);
        }

        public IActionResult CreatePost()
        {
            return View();
        }

        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await GetItemById(id.HasValue ? id.Value : 0);
            var itemViewModel = new ItemRequest()
            {
                Id = item.Id,
                Name = item.Name,
                ExistingImage = item.Image,
                Description = item.Description
            };

            if (item == null)
            {
                return NotFound();
            }
            return View(itemViewModel);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await GetItemById(id.HasValue ? id.Value : 0);

            var itemViewModel = new ItemRequest()
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                ExistingImage = item.Image
            };
            if (item == null)
            {
                return NotFound();
            }

            return View(itemViewModel);
        }


        public async Task<IActionResult> SavePost(ItemRequest itemRequest)
        {
            using (HttpClient client = new HttpClient())
            {
                itemRequest.ExistingImage = ProcessUploadedFile(itemRequest);

                var multipartContent = new MultipartFormDataContent();

                multipartContent.Add(new StringContent(itemRequest.Name), "Name");
                multipartContent.Add(new StringContent(itemRequest.ExistingImage), "ExistingImage");
                multipartContent.Add(new StringContent(itemRequest.Description), "Description");
                multipartContent.Add(new StreamContent(itemRequest.Image.OpenReadStream()), "Image", itemRequest.Image.FileName);

                string endpoint = apiBaseUrl + "Admin/SaveItem";
                client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                using (var Response = await client.PostAsync(endpoint, multipartContent))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<ApiResponseMessage>(apiResponse);

                    if (responseMessage != null && responseMessage.IsSuccess && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ModelState.Clear();
                        TempData["Message"] = responseMessage.SuccessMessage;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, responseMessage.ErrorMessage);
                        return View("Create", itemRequest);
                    }
                }
            }
        }


        public async Task<IActionResult> EditItem(ItemRequest itemRequest)
        {
            var item = await GetItemById(itemRequest.Id);
            using (HttpClient client = new HttpClient())
            {
                if (item.Image != null)
                {
                    if (itemRequest.ExistingImage != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder, itemRequest.ExistingImage);
                        System.IO.File.Delete(filePath);
                    }
                    itemRequest.ExistingImage = ProcessUploadedFile(itemRequest);
                }
                var multipartContent = new MultipartFormDataContent();

                multipartContent.Add(new StringContent(JsonConvert.SerializeObject(itemRequest.Id)), "Id");
                multipartContent.Add(new StringContent(itemRequest.Name), "Name");
                multipartContent.Add(new StringContent(itemRequest.ExistingImage), "ExistingImage");
                multipartContent.Add(new StringContent(itemRequest.Description), "Description");
                multipartContent.Add(new StreamContent(itemRequest.Image.OpenReadStream()), "Image", itemRequest.Image.FileName);

                string endpoint = apiBaseUrl + "Admin/EditPost";
                client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                using (var Response = await client.PostAsync(endpoint, multipartContent))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<ApiResponseMessage>(apiResponse);

                    if (responseMessage != null && responseMessage.IsSuccess && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ModelState.Clear();
                        TempData["Message"] = responseMessage.SuccessMessage;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, responseMessage.ErrorMessage);
                        return View("EditPost", itemRequest);
                    }
                }
            }
        }

        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await GetItemById(id);
            using (HttpClient client = new HttpClient())
            {
                string endpoint = apiBaseUrl + "Admin/DeletePost/" + id;
                client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                using (var Response = await client.PostAsync(endpoint, null))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<ApiResponseMessage>(apiResponse);

                    if (responseMessage != null && responseMessage.IsSuccess && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        if (item.Image != null)
                        {
                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder, item.Image);
                            System.IO.File.Delete(filePath);
                        }
                        ModelState.Clear();
                        TempData["Message"] = responseMessage.SuccessMessage;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, "Delete Failed...");
                        return View("Index");
                    }
                }
            }
        }




        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private async Task<List<Item>> GetAllItems()
        {
            using (HttpClient client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("Token");
                string endpoint = apiBaseUrl + "Admin/GetAllItems";
                client.DefaultRequestHeaders.Add("Authorization", token);
                using (var Response = await client.GetAsync(endpoint))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<List<Item>>(apiResponse);

                    if (responseMessage != null && responseMessage.Count() != 0 && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return responseMessage;
                    }
                    else
                    {
                        return new List<Item>();
                    }
                }
            }
        }
        private async Task<Item> GetItemById(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("Token");
                string endpoint = apiBaseUrl + "Admin/GetItemById/" + id;
                client.DefaultRequestHeaders.Add("Authorization", token);
                using (var Response = await client.GetAsync(endpoint))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<Item>(apiResponse);

                    if (responseMessage != null && responseMessage != null && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return responseMessage;
                    }
                    else
                    {
                        return new Item();
                    }
                }
            }
        }


        private string ProcessUploadedFile(ItemRequest model)
        {
            string uniqueFileName = null;

            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
