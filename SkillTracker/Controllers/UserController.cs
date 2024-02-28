using AutoMapper;
using BusinessLogicLayer;
using SkillTracker.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System;

namespace SkillTracker.Controllers
{
    public class UserController : ApiController
    {
        private readonly IMapper mapper;
        public UserController()
        {
            var mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserDTO, UserModel>();
                cfg.CreateMap<GetUserDetailsDTO, GetUserDetailsModel>()
            .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills));
                cfg.CreateMap<UpdateUserSkillsDTO, UpdateUserSkillsModel>();
                cfg.CreateMap<UserModel, DisplayModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.EmailID, opt => opt.MapFrom(src => src.EmailID));
            });
            mapper = mapConfig.CreateMapper();
        }

        UserService userBusiness = new UserService();
        public List<DisplayModel> GetAllUsers()        //View -> Admin
        {
            List<UserDTO> users = userBusiness.GetListOfUsers();
            List<UserModel> userModelList = mapper.Map<List<UserModel>>(users);
            List<DisplayModel> displayModels = mapper.Map<List<DisplayModel>>(userModelList);
            return displayModels;
        }

        [Route("api/user/getalladmins")]
        public List<DisplayModel> GetAllAdmins()       //View -> Admin
        {
            List<UserDTO> admins = userBusiness.GetListOfAdmins();
            List<UserModel> adminModelList = mapper.Map<List<UserModel>>(admins);
            List<DisplayModel> displayModels = mapper.Map<List<DisplayModel>>(adminModelList);
            return displayModels;
        }

        [Route("api/user/getuserbyidorname")]
        public IHttpActionResult GetUserByIdOrName(string searchUser)       //View -> Admin
        {
            List<UserDTO> foundUsers = userBusiness.GetUserByIdOrName(searchUser);


            if (foundUsers.Any())
            {
                List<UserModel> users = mapper.Map<List<UserModel>>(foundUsers);
                List<DisplayModel> displayModel = mapper.Map<List<DisplayModel>>(users);

                return Ok(displayModel);
            }
            else
            {
                return Ok<string>("User not found");
            }
        }

        [HttpGet]
        [Route("api/user/getusersbyskill")]
        public IHttpActionResult GetUsersBySkill(string skillName)
        {
            List<UserDTO> users = userBusiness.GetUsersBySkill(skillName);

            if (users.Any())
            {
                List<UserModel> userModelList = mapper.Map<List<UserModel>>(users);
                List<DisplayModel> displayUsers = mapper.Map<List<DisplayModel>>(userModelList);

                return Ok(displayUsers);
            }
            else
            {
                return Ok("No users found with the specified skill.");
            }
        }
        [Route("api/user/{id}")]
        public IHttpActionResult GetUserDetails(int id)         //View -> Admin & User(Id)
        {
            GetUserDetailsDTO getUserDetailsDTOs = userBusiness.GetUserDetails(id);
            GetUserDetailsModel getUserDetailsModel = mapper.Map<GetUserDetailsModel>(getUserDetailsDTOs);
            return Ok(getUserDetailsModel);
        }

        [HttpPost]
        [Route("api/user/AddNewUser")]
        public IHttpActionResult PostNewUser([FromBody] UserModel newUser)      //View -> Admin
        {
            try
            {
                UserDTO userDTO = new UserDTO
                {
                    EmailID = newUser.EmailID,
                    Password = newUser.Password,
                };
                bool result = userBusiness.AddUser(userDTO);
                if (result)
                {
                    return Ok($"User having User ID: {userDTO.ID} added successfully.");
                }
                else
                {
                    return Ok<string>("Error occured during adding Try Again..");
                }

            }
            catch (ArgumentException ex)
            {

                return BadRequest(ex.Message);
            }
          
        }

        [HttpDelete]
        [Route("api/user/{id}/Delete")]
        public IHttpActionResult DeleteUser(int id)         //View -> Admin
        {
            try
            {
                UserService userBusiness = new UserService();
                userBusiness.DeleteUser(id);

                return Ok($"User with ID {id} is deleted.");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        [HttpPut]
        [Route("api/user/{id}/Edit")]
        public IHttpActionResult PutUpdatedUserDetails(int id, UserModel userModel)         //View -> Admin & User(Id)
        {
            UserDTO userDTO = new UserDTO();
            userDTO.FullName = userModel.FullName;
            userDTO.ContactNo = userModel.ContactNo;
            userDTO.DOB = userModel.DOB;
            userDTO.Gender = userModel.Gender;

            bool result = userBusiness.UpdateUserDetails(id, userDTO);
            if (result)
            {
                return Ok<string>("Details Updated Successfully!");
            }
            else
            {
                return Ok<string>("Error occured during updating Try Again..");
            }
        }


        [HttpPost]
        [Route("api/user/postloginuser")]
        public IHttpActionResult PostUserLogin(UserModel loginUser)         //View -> Admin & User
        {
            string email = loginUser.EmailID;
            string password = loginUser.Password;

            UserDTO authenticatedUser = userBusiness.AuthenticateUser(email, password);

            if (authenticatedUser != null)
            {
                if (authenticatedUser.Is_Admin)
                {
                    List<UserDTO> users = userBusiness.GetListOfUsers();
                    List<UserModel> userModelList = mapper.Map<List<UserModel>>(users);
                    List<DisplayModel> displayModels = mapper.Map<List<DisplayModel>>(userModelList);
                    return Ok(displayModels);
                }
                else
                {
                    UserModel userModel = mapper.Map<UserModel>(authenticatedUser);
                    DisplayModel displayModel = mapper.Map<DisplayModel>(userModel);
                    return Ok(displayModel);
                }
            }
            else
            {
                return BadRequest("Invalid Email or Password");
            }
        }


        public string HashPassword(string password) // methods that takes a plain text password 
        {
            using (SHA512 sha256Hash = SHA512.Create()) // using Create() method of SHA256 class to create an instance of the class.
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password)); // Computing hash using UTF8 encoding 

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder(); // using "StringBuilder" class, to build strings from the bytes obtained above.
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Converting byte to String using "x2" format specifier.
                }
                return builder.ToString();
            }
        }
        [HttpPost]
        [Route("api/user/forgotpassword")]
        public IHttpActionResult ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            string email = forgotPasswordModel.EmailID;
            DateTime dob = forgotPasswordModel.DOB;
            string newPassword = forgotPasswordModel.NewPassword;
            string confirmPassword = forgotPasswordModel.ConfirmPassword;

            if (newPassword != confirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            if (userBusiness.VerifyUserEmailAndDOB(email, dob))
            {
                if (userBusiness.UpdatePassword(email, newPassword))
                {
                    return Ok("Password updated successfully.");
                }
                else
                {
                    return InternalServerError();
                }
            }
            else
            {
                return BadRequest("Invalid email or date of birth.");
            }
        }
    }
    
}