using AutoMapper;
using System.Net;
using WisdomBlog.API.Authorization;
using WisdomBlog.Models.Data;
using WisdomBlog.Models.DBEntities;
using WisdomBlog.Models.RequestModels;
using WisdomBlog.Models.ResponseModels;

namespace WisdomBlog.API.Services
{
    public interface IUserService
    {
        Users GetById(int id);
        LoginResponse UserAuthenticate(LoginRequest model);
    }
    public class UserService : IUserService
    {
        private WisdomBlogDbContext _context;
        private IJwtUtils _jwtUtils;

        public UserService(WisdomBlogDbContext context,
            IJwtUtils jwtUtils)
        {
            _context = context;
            _jwtUtils = jwtUtils;
        }

        public Users GetById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }

        public LoginResponse UserAuthenticate(LoginRequest model)
        {
            var user = _context.Users.Where(x => x.IsActive == true).SingleOrDefault(x => x.Username == model.Username);

            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                var authenticateResponse = new LoginResponse()
                {
                    ErrorMessage = "Username or password is incorrect",
                    ResponseMesssage = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                };
                return authenticateResponse;
            }

            // authentication successful
            //var response = _mapper.Map<LoginResponse>(user);
            var response = new LoginResponse
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                MobileNo = user.MobileNo,
                Username = user.Username
            };
            response.Token = _jwtUtils.GenerateToken(user);
            response.ResponseMesssage = new HttpResponseMessage(HttpStatusCode.OK);
            return response;
        }

    }
}
