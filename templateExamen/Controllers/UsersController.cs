using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using templateExamen.Models;
using templateExamen.Services;
using templateExamen.ViewModels;

namespace templateExamen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService userService;

        public UsersController(IUserService userservice)
        {
            userService = userservice;
        }
        /// <summary>
        /// Login for user 
        /// </summary>
        /// <remarks>
        ///            {
        ///            "username":"pop91",
        ///            "password":"pop123456"
        ///            }
        /// 
        /// 
        /// </remarks>
        /// <param name="loginModel">Enter username and password</param>
        /// <returns>Return username , email and token</returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] LoginPostModel loginModel)
        {
            var user = userService.Authenticate(loginModel.Username, loginModel.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
        /// <summary>
        /// Register a user in the database
        /// </summary>
        /// <remarks>
        ///     {
        ///         "firstName":"Pop",
        ///         "lastName":"Mihai",
        ///         "username":"pop91",
        ///         "email":"pop@yahoo.com",
        ///         "password":"pop123456"
        ///        }
        /// </remarks>
        /// <param name="registerModel">Introduce firstname, lastname,username,email and password</param>
        /// <returns>Inserted user in database</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterPostModel registerModel)
        {
            var user = userService.Register(registerModel);
            if (user == null)
            {
                return BadRequest(new { ErrorMessage = "Username already exists" });
            }
            // userService.GetCurentUser(HttpContext);
            return Ok(user);
        }

        [HttpGet]
       [Authorize(Roles = "Moderator,Admin")]
        public IActionResult GetAll()
        {
            var users = userService.GetAll();
            return Ok(users);
        }


        /// <summary>
        /// Find an user by the given id.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     Get /users
        ///     {  
        ///        id: 3,
        ///        firstName = "Pop",
        ///        lastName = "Andrei",
        ///        userName = "user123",
        ///        email = "Us1@yahoo.com",
        ///        userRole = "regular"
        ///     }
        /// </remarks>
        /// <param name="id">The id given as parameter</param>
        /// <returns>The user with the given id</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // GET: api/Users/5
        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult GetByIdHistory(int id)
        {
            var found = userService.GetHistoryById(id);
            if (found == null)
            {
                return NotFound();
            }
            return Ok(found);
        }



        /// <summary>
        /// Add an new User
        /// </summary>
        ///   /// <remarks>
        /// Sample response:
        ///
        ///     Post /users
        ///     {
        ///        firstName = "Pop",
        ///        lastName = "Andrei",
        ///        userName = "user123",
        ///        email = "Us1@yahoo.com",
        ///        userRole = "regular"
        ///     }
        /// </remarks>
        /// <param name="userPostModel">The input user to be added</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public void Post([FromBody] UserPostModel userPostModel)
        {
            userService.Create(userPostModel);
        }



        /// <summary>
        /// Modify an user if exists in dbSet , or add if not exist
        /// </summary>
        /// <param name="id">id-ul user to update</param>
        /// <param name="userPostModel">obiect userPostModel to update</param>
        /// Sample request:
        ///     <remarks>
        ///     Put /users/id
        ///     {
        ///        firstName = "Pop",
        ///        lastName = "Andrei",
        ///        userName = "user123",
        ///        email = "Us1@yahoo.com",
        ///        userRole = "regular"
        ///     }
        /// </remarks>
        /// <returns>Status 200 daca a fost modificat</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UserPostModel userPostModel)
        {
            User userlogat = userService.GetCurentUser(HttpContext);
            var result = userService.Upsert(id, userPostModel, userlogat);
            if (result == null)
            {
                return BadRequest(new { ErrorMessage = "No update was made" });
            }
            return Ok(result);
            //if (userlogat.UserRole.Equals(UserRole.User_Manager))
            //{
            //    //  https://www.aspforums.net/Threads/289493/Get-Number-of-months-between-two-dates-in-C/
            //    //   UserGetModel userUpdate = userService.GetById(id);
            //    var dateRegister = userlogat.DateRegister;
            //    var dateCurrent = DateTime.Now;
            //    int months = dateCurrent.Subtract(dateRegister).Days / 30;

            //    if (months >= 6)
            //    {
            //        var updateResult = userService.Upsert(id, userPostModel);
            //        return Ok(updateResult);
            //    }
            //    UserPostModel newUserPostModel = new UserPostModel
            //    {
            //        FirstName = userPostModel.FirstName,
            //        LastName = userPostModel.LastName,
            //        Email = userPostModel.Email,
            //        UserName = userPostModel.UserName,
            //        UserRole = userPostModel.UserRole
            //    };
            //    var result2 = userService.Upsert(id, newUserPostModel);
            //    return Ok(result2);
            //}
            //var result = userService.Upsert(id, userPostModel);
            //return Ok(result);
        }



        /// <summary>
        /// Delete an user
        /// </summary>
        /// <param name="id">User id to delete</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin,Moderator")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            User userlogat = userService.GetCurentUser(HttpContext);

            var result = userService.Delete(id, userlogat);
            if (result == null)
            {
                return BadRequest(new { ErrorMessage = "User with the given id not fount !" });
            }
            return Ok(result);
        }
    }
}
