using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]

    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepository;

        public UserController(IConfiguration configuration, IRepository<User> userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<User> users = _userRepository.GetAll();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public IActionResult Get(int userId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Post(User newUser)
        {
            bool added = _userRepository.Add(newUser);
            if (!added)
            {
                return BadRequest("Failed to add user");
            }

            return Ok();
        }

[HttpPut("{userId}")]
public IActionResult Put(int userId, [FromBody] User updatedUser)
{
    if (updatedUser == null)
    {
        return BadRequest("Invalid user data.");
    }

    // Retrieve the existing user
    var existingUser = _userRepository.GetById(userId);
    if (existingUser == null)
    {
        return NotFound("User not found.");
    }

    // Create a new User object with the updated information
    var userToUpdate = new User(
        userId, 
        updatedUser.FirstName, 
        updatedUser.LastName, 
        updatedUser.Email, 
        updatedUser.Phone, 
        updatedUser.Password, 
        updatedUser.Address, 
        updatedUser.City, 
        updatedUser.PostalCode
    );

    // Update the user in the repository
    bool updated = _userRepository.Update(userToUpdate);

    return updated ? Ok() : StatusCode(500, "Failed to update the user.");
}



        [HttpDelete("{userId}")]
        public IActionResult Delete(int userId)
        {
            bool deleted = _userRepository.Delete(userId);
            if (deleted)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

            [HttpPost("login")]
            public IActionResult Login(UserLoginRequest loginRequest)
            {
                 // Admin login check
                if (loginRequest.Email == "admin@example.com" && loginRequest.Password == "admin")
            {
                 return Ok(new { userType = "admin", message = "Login successful" });
            }
                var user = _userRepository.GetAll().FirstOrDefault(u => u.Email == loginRequest.Email);

                if (user == null || !user.CheckPassword(loginRequest.Password))
                {
                    return Unauthorized("Invalid username or password");
                }

                var jwtService = new JwtService(_configuration);
                var token = jwtService.GenerateJwtToken(user);

                return Ok(new { Token = token });
            }

        }
}
    