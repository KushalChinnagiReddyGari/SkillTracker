using AutoMapper;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogicLayer
{
    public class UserService
    {
        private readonly IMapper mapper;
        public UserService()
        {
            var mapConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<User, UserDTO>();
               
            });
            mapper = mapConfig.CreateMapper();
        }

        DataAccessLayer.SkillTrackerDBEntities db = new SkillTrackerDBEntities();
        public List<UserDTO> GetListOfUsers()
        {          
            DbSet<User> userDb = db.Users;

            List<UserDTO> users = new List<UserDTO>();
            foreach (var user in userDb)
            {
                if(user.Is_Admin != true)
                {                 
                    users.Add(MapUserToUserDTO(user));
                }
            }
            return users;
        }
        public List<UserDTO> GetListOfAdmins()
        {
            DbSet<User> userDb = db.Users;

            List<UserDTO> users = new List<UserDTO>();
            foreach (var user in userDb)
            {
                if (user.Is_Admin == true)
                {
                    users.Add(MapUserToUserDTO(user));
                }
            }
            return users;
        }
        public List<UserDTO> GetUserByIdOrName(string searchUser)
        {
            DbSet<User> userDb = db.Users;
            List<UserDTO> matchingUsers = new List<UserDTO>();

            if (int.TryParse(searchUser, out int userId))
            {
                var userById = userDb.FirstOrDefault(u => u.ID == userId);
                if (userById != null)
                {
                    matchingUsers.Add(MapUserToUserDTO(userById));
                    return matchingUsers;
                }
            }
            var userByName = userDb.Where(u => u.FullName.StartsWith(searchUser)).ToList();
            if (userByName.Any())
            {
                matchingUsers.AddRange(userByName.Select(u => MapUserToUserDTO(u)));
            }
            return matchingUsers;
        }

        public List<UserDTO> GetUsersBySkill(string skillName)
        {
            DbSet<UserSkill> userSkillDb = db.UserSkills;

            var usersWithSkill = userSkillDb
                .Where(us => us.Skill.Name.Equals(skillName, StringComparison.OrdinalIgnoreCase))
                .Select(us => us.User).ToList();

            return usersWithSkill.Select(user => MapUserToUserDTO(user)).ToList();
        }
        public GetUserDetailsDTO GetUserDetails(int userId)
         {
            User user = db.Users.Find(userId);
            List<UserSkill> userSkills = db.UserSkills
                                        .Where(skill => skill.UserID == userId)
                                        .ToList();
            var userDetails = userSkills
           .Join(db.Skills, us => us.SkillID, skill => skill.ID, (us, skill) => new UpdateUserSkillsDTO
           {
            Name = skill.Name,
            Proficiency = us.Proficiency
           })
           .ToList();
            List<UpdateUserSkillsDTO> updateUserSkillsDTOs = new List<UpdateUserSkillsDTO>();
            foreach (var item in userDetails)
            {
                UpdateUserSkillsDTO updateUserSkillsDTO = new UpdateUserSkillsDTO
                {
                    Name = item.Name,
                    Proficiency = item.Proficiency
                };
                updateUserSkillsDTOs.Add(updateUserSkillsDTO);
            }

            GetUserDetailsDTO getUserDetailsDTO = new GetUserDetailsDTO();
            getUserDetailsDTO.ID = user.ID;
            getUserDetailsDTO.EmailID = user.EmailID;
            getUserDetailsDTO.Password = this.HashPassword(user.Password);
            getUserDetailsDTO.FullName = user.FullName;
            getUserDetailsDTO.DOB = user.DOB;
            Int64 contact_no = Convert.ToInt64(user.ContactNo);
            getUserDetailsDTO.ContactNo = contact_no;
            getUserDetailsDTO.Gender = user.Gender;
           
            getUserDetailsDTO.Skills = new List<UpdateUserSkillsDTO>();
            foreach (var skill in updateUserSkillsDTOs)
            {
                UpdateUserSkillsDTO skillDTO = new UpdateUserSkillsDTO
                {
                    Name = skill.Name,
                    Proficiency = skill.Proficiency
                };
                getUserDetailsDTO.Skills.Add(skillDTO);
            }
            return getUserDetailsDTO;
        }
        public bool AddUser(UserDTO newUser)
        {
            DbSet<User> userDb = db.Users;

            
            if (userDb.Any(u => u.EmailID == newUser.EmailID))
            {
                throw new ArgumentException("User with the same EmailId already exists.");
            }
            User user = new User();
            user.EmailID = newUser.EmailID;
            user.Password = newUser.Password;

            userDb.Add(user);
            db.SaveChanges();

            newUser.ID = user.ID;
            return true;
        }
        public void DeleteUser(int userId)
        {
            using (var db = new SkillTrackerDBEntities())
            {
                
                User user = db.Users.Find(userId);

                if (user != null)
                {
                    // Delete related UserSkills
                    var userSkills = db.UserSkills.Where(us => us.UserID == userId);
                    foreach (var userSkill in userSkills)
                    {
                        db.UserSkills.Remove(userSkill);
                    }

                    // Remove the user entity from the database
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception($"User with ID {userId} not found.");

                }
            }
        }
        public bool UpdateUserDetails(int id,UserDTO userDTO)
        {

            var existingUser = db.Users.FirstOrDefault(u => u.ID == id);
            if (existingUser != null)
            {
                existingUser.FullName = userDTO.FullName;
                existingUser.ContactNo = userDTO.ContactNo;
                existingUser.DOB = userDTO.DOB;
                existingUser.Gender = userDTO.Gender;
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public UserDTO AuthenticateUser(string email, string password)
        {
            DbSet<User> userDb = db.Users;
            var authenticatedUser = userDb.FirstOrDefault(u => u.EmailID == email && u.Password == password);

            if (authenticatedUser != null)
            {
                UserDTO userDTO = new UserDTO();
                userDTO.ID = authenticatedUser.ID;
                userDTO.FullName = authenticatedUser.FullName;
                userDTO.EmailID = authenticatedUser.EmailID;
                userDTO.Is_Admin = authenticatedUser.Is_Admin;
                return userDTO;
            }
            return null;
        }

        private UserDTO MapUserToUserDTO(User user)
        {
            return new UserDTO
            {
                ID = user.ID,
                FullName = user.FullName,
                EmailID = user.EmailID,
                Password = this.HashPassword(user.Password)
        };
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

        public bool VerifyUserEmailAndDOB(string email, DateTime dob)
        {
            DbSet<User> userDb = db.Users;
            return userDb.Any(u => u.EmailID == email && u.DOB == dob);
        }
        public bool UpdatePassword(string email, string newPassword)
        {
            DbSet<User> userDb = db.Users;
            var userToUpdate = userDb.FirstOrDefault(u => u.EmailID == email);

            if (userToUpdate != null)
            {
                userToUpdate.Password = newPassword;
                db.SaveChanges();
                return true;
            }
            return false;
        }
    }
}