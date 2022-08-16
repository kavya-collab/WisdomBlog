using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using WisdomBlog.Models.DBEntities;
using WisdomBlog.Models.RequestModels;
using WisdomBlog.Models.ResponseModels;
using WisdomBlog.UI.Models;

namespace WisdomBlog.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _Configure;
        private static string apiBaseUrl;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _Configure = configuration;
            apiBaseUrl = _Configure.GetValue<string>("WebAPIBaseUrl");
        }

        public async Task<IActionResult> Index()
        {
            var itemsList = await GetAllItems();
            return View(itemsList);
        }

        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> ReadContent(int id)
        {
            var item = await GetItemById(id);
            return View(item);
        }

        public async Task<IActionResult> userAuthenticate(LoginRequest user)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "Login/UserAuthenticate";

                using (var Response = await client.PostAsync(endpoint, content))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var authenticateResponse = JsonConvert.DeserializeObject<LoginResponse>(apiResponse);

                    if (authenticateResponse != null && authenticateResponse.ResponseMesssage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        HttpContext.Session.SetString("Token", authenticateResponse.Token);
                        HttpContext.Session.SetString("Username", authenticateResponse.Username);
                        HttpContext.Session.SetString("Name", authenticateResponse.FirstName + " " + authenticateResponse.LastName);
                        HttpContext.Session.SetInt32("UserId", authenticateResponse.Id);
                        TempData["Profile"] = JsonConvert.SerializeObject(apiResponse);
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
                        return View("Index");
                    }
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<List<Item>> GetAllItems()
        {
            using (HttpClient client = new HttpClient())
            {
                string endpoint = apiBaseUrl + "Admin/GetAllItems";
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
    }
}