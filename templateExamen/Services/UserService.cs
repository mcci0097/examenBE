using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using templateExamen.Models;
using templateExamen.ViewModels;

namespace templateExamen.Services
{
    public class UserService
    {
        public interface IUserService
        {
            UserGetModel Authenticate(string username, string password);
            UserGetModel Register(RegisterPostModel register);
            User GetCurentUser(HttpContext httpContext);
            IEnumerable<UserGetModel> GetAll();

            User Create(UserPostModel userModel);
            UserGetModel Upsert(int id, UserPostModel userPostModel, User userLogat);
            UserGetModel Delete(int id, User addedBy);
            IEnumerable<HistoryUserRoleGetModel> GetAllHistory();
            IEnumerable<HistoryUserRoleGetModel> GetHistoryById(int id);
        }

        public class UserService : IUserService
        {
            private TasksDbContext dbcontext;

            private readonly AppSettings appSettings;

            public UserService(TasksDbContext context, IOptions<AppSettings> appSettings)
            {
                this.dbcontext = context;
                this.appSettings = appSettings.Value;
            }

            public UserGetModel Authenticate(string username, string password)
            {
                var user = dbcontext
                    .Users
                    .Include(h => h.HistoryUserRole)
                    .ThenInclude(ur => ur.UserRole)
                    .SingleOrDefault(x => x.Username == username && x.Password == ComputeSha256Hash(password));

                // return null if user not found
                if (user == null)
                    return null;

                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                  //  new Claim(ClaimTypes.Role, user.UserRole.ToString()),
                    new Claim (ClaimTypes.Role, getLatestHistoryUserRole(user.HistoryUserRole).UserRole.Title),
                    new Claim(ClaimTypes.UserData, user.DateRegister.ToString())

                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var result = new UserGetModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Token = tokenHandler.WriteToken(token),
                    UserRole = user.HistoryUserRole.First().UserRole.Title


                };


                return result;
            }

            private string ComputeSha256Hash(string rawData)
            {
                // Create a SHA256   
                // TODO: also use salt
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    // ComputeHash - returns byte array  
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                    // Convert byte array to a string   
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
            public UserGetModel Register(RegisterPostModel register)
            {
                User existing = dbcontext.Users.FirstOrDefault(u => u.Username == register.Username);
                if (existing != null)
                {
                    return null;
                }

                User toAdd = new User
                {
                    Email = register.Email,
                    LastName = register.LastName,
                    FirstName = register.FirstName,
                    Password = ComputeSha256Hash(register.Password),
                    Username = register.Username,
                    DateRegister = DateTime.Now,
                    HistoryUserRole = new List<HistoryUserRole>()
                };

                dbcontext.Users.Add(toAdd);
                dbcontext.SaveChanges();
                dbcontext.Users.Attach(toAdd);
                UserRole userRole = new UserRole
                {
                    Id = 1,
                    Title = RoleConstants.REGULAR,
                };

                //var defaultRole = dbcontext
                //   .UserRoles
                //   .AsNoTracking()
                //   .FirstOrDefault(uRole => uRole.Title == RoleConstants.REGULAR);

                HistoryUserRole history = new HistoryUserRole
                {
                    User = toAdd,
                    UserRole = userRole,
                    StartTime = DateTime.Now,
                    EndTime = null
                };
                List<HistoryUserRole> list = new List<HistoryUserRole>
            {
                history
            };
                //dbcontext.HistoryUserRoles.Add(new HistoryUserRole
                //{
                //    User = toAdd,
                //    UserRole = RoleConstants.REGULAR,
                //    StartTime = DateTime.Now,
                //    EndTime = null

                //});         
                dbcontext.UserRoles.Add(userRole);
                dbcontext.UserRoles.Attach(userRole);
                toAdd.HistoryUserRole = list;
                dbcontext.SaveChanges();
                return Authenticate(register.Username, register.Password);
            }

            public User GetCurentUser(HttpContext httpContext)
            {
                string username = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
                //string accountType = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationMethod).Value;
                //return _context.Users.FirstOrDefault(u => u.Username == username && u.AccountType.ToString() == accountType);

                return dbcontext
                    .Users
                    .Include(u => u.HistoryUserRole)
                    .ThenInclude(ur => ur.UserRole)
                    .FirstOrDefault(u => u.Username == username);
            }


            public IEnumerable<UserGetModel> GetAll()
            {
                // return users without passwords
                return dbcontext.Users.Select(user => new UserGetModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    UserRole = user.HistoryUserRole.First().UserRole.Title,
                    Token = null
                });
            }

            public IEnumerable<HistoryUserRoleGetModel> GetAllHistory()
            {
                return dbcontext
                    .HistoryUserRoles
                    .OrderBy(x => x.StartTime)
                    .Select(history => new HistoryUserRoleGetModel
                    {
                        Username = history.User.Username,
                        UserRoleTitle = history.UserRole.Title,
                        StartTime = history.StartTime,
                        EndTime = history.EndTime
                    });
            }

            public IEnumerable<HistoryUserRoleGetModel> GetHistoryById(int id)
            {
                List<HistoryUserRole> histories = dbcontext.Users
                    .Include(x => x.HistoryUserRole)
                    .ThenInclude(x => x.UserRole)
                    .FirstOrDefault(u => u.Id == id).HistoryUserRole;
                List<HistoryUserRoleGetModel> returnList = new List<HistoryUserRoleGetModel>();
                foreach (HistoryUserRole history in histories)
                {
                    returnList.Add(HistoryUserRoleGetModel.FromHistoryUserRole(history));
                }
                var list = returnList.OrderBy(x => x.StartTime);
                return list;
            }

            public User Create(UserPostModel userModel)
            {
                User toAdd = UserPostModel.ToUser(userModel);

                dbcontext.Users.Add(toAdd);
                dbcontext.SaveChanges();
                dbcontext.Users.Attach(toAdd);

                UserRole userRole = new UserRole
                {
                    Id = 1,
                    Title = RoleConstants.REGULAR
                };
                HistoryUserRole history = new HistoryUserRole
                {
                    UserRole = userRole,
                    StartTime = DateTime.Now
                };
                List<HistoryUserRole> list = new List<HistoryUserRole>
            {
                history
            };

                dbcontext.UserRoles.Add(userRole);
                dbcontext.UserRoles.Attach(userRole);

                toAdd.HistoryUserRole = list;

                dbcontext.SaveChanges();

                //dbcontext.Users.Add(toAdd);
                //dbcontext.SaveChanges();
                return toAdd;



            }


            public UserGetModel Upsert(int id, UserPostModel user, User userLogat)
            {
                var existing = dbcontext
                    .Users
                    .Include(x => x.HistoryUserRole)
                    .ThenInclude(x => x.UserRole)
                    .AsNoTracking().FirstOrDefault(u => u.Id == id);
                if (existing == null)
                {
                    return null;
                }
                if (existing.Username.Equals(userLogat.Username))
                {
                    return null;
                }

                string existingCurrentRole = getLatestHistoryUserRole(existing.HistoryUserRole).UserRole.Title;
                string addedByCurrentRole = getLatestHistoryUserRole(userLogat.HistoryUserRole).UserRole.Title;

                HistoryUserRole currentHistory = getLatestHistoryUserRole(existing.HistoryUserRole);

                if (existing == null)
                {
                    //dbcontext.Users.(UserPostModel.ToUser(user));
                    //dbcontext.SaveChanges();
                    //return UserGetModel.FromUser(UserPostModel.ToUser(user));
                    User toReturn = Create(user);
                    return UserGetModel.FromUser(toReturn);
                }

                User toUpdate = UserPostModel.ToUser(user);

                toUpdate.Password = existing.Password;
                toUpdate.DateRegister = existing.DateRegister;
                toUpdate.Id = id;
                if (addedByCurrentRole.Equals(RoleConstants.USER_MANAGER))
                {
                    //  https://www.aspforums.net/Threads/289493/Get-Number-of-months-between-two-dates-in-C/
                    var dateRegister = userLogat.DateRegister;
                    var dateCurrent = DateTime.Now;
                    int months = dateCurrent.Subtract(dateRegister).Days / 30;
                    //  toAdd.Id = id;

                    if (months >= 6)
                    {
                        //dbcontext.Users.Update(toAdd);
                        //dbcontext.SaveChanges();
                        //return UserGetModel.FromUser(toAdd);
                        toUpdate.HistoryUserRole = existing.HistoryUserRole;
                        dbcontext.Users.Update(toUpdate);
                        dbcontext.SaveChanges();
                        dbcontext.Users.Attach(toUpdate);

                        if (existingCurrentRole != user.UserRole)
                        {

                            IEnumerable<UserRole> allRoles = dbcontext.UserRoles;
                            List<String> list = new List<string>();
                            foreach (UserRole userRole in allRoles)
                            {
                                list.Add(userRole.Title);
                            }
                            if (list.Contains(user.UserRole))
                            {
                                UserRole userRole = searchForRoleByTitle(user.UserRole);
                                HistoryUserRole history = new HistoryUserRole
                                {
                                    UserRole = userRole,
                                    StartTime = DateTime.Now
                                };
                                currentHistory.EndTime = DateTime.Now;

                                dbcontext.UserRoles.Attach(userRole);
                                toUpdate.HistoryUserRole.Add(history);
                                dbcontext.SaveChanges();
                                return UserGetModel.FromUser(toUpdate);
                            }
                            return null;
                        }
                        return null;
                        //  dbcontext.Users.Update(toAdd);
                        //dbcontext.SaveChanges();
                        //return toAdd;
                    }
                    return null;
                }
                if (addedByCurrentRole.Equals(RoleConstants.ADMIN))
                {
                    //toAdd.Id = id;
                    //dbcontext.Users.Update(toAdd);
                    //dbcontext.SaveChanges();
                    //return UserGetModel.FromUser(toAdd);

                    toUpdate.HistoryUserRole = existing.HistoryUserRole;
                    dbcontext.Users.Update(toUpdate);
                    dbcontext.SaveChanges();
                    dbcontext.Users.Attach(toUpdate);

                    if (existingCurrentRole != user.UserRole)
                    {

                        IEnumerable<UserRole> allRoles = dbcontext.UserRoles;
                        List<String> list = new List<string>();
                        foreach (UserRole userRole in allRoles)
                        {
                            list.Add(userRole.Title);
                        }
                        if (list.Contains(user.UserRole))
                        {

                            UserRole userRole = searchForRoleByTitle(user.UserRole);
                            HistoryUserRole history = new HistoryUserRole
                            {
                                UserRole = userRole,
                                StartTime = DateTime.Now
                            };
                            currentHistory.EndTime = DateTime.Now;

                            dbcontext.UserRoles.Attach(userRole);
                            toUpdate.HistoryUserRole.Add(history);
                            dbcontext.SaveChanges();
                            return UserGetModel.FromUser(toUpdate);
                        }
                        return null;
                    }
                    return null;
                }
                return null;
            }
            public UserGetModel Delete(int id, User addedBy)
            {
                var existing = dbcontext
                    .Users
                    .Include(x => x.HistoryUserRole)
                    .ThenInclude(x => x.UserRole)
                    .FirstOrDefault(u => u.Id == id);
                string addedByCurrentRole = getLatestHistoryUserRole(addedBy.HistoryUserRole).UserRole.Title;
                string existingCurrentRole = getLatestHistoryUserRole(existing.HistoryUserRole).UserRole.Title;
                if (existing == null)
                {
                    return null;
                }
                if (addedByCurrentRole.Equals(RoleConstants.USER_MANAGER))
                {
                    //  https://www.aspforums.net/Threads/289493/Get-Number-of-months-between-two-dates-in-C/
                    var dateRegister = addedBy.DateRegister;
                    var dateCurrent = DateTime.Now;
                    int months = dateCurrent.Subtract(dateRegister).Days / 30;


                    if (months >= 6)
                    {
                        dbcontext.Comments.RemoveRange(dbcontext.Comments.Where(u => u.Owner.Id == existing.Id));
                        dbcontext.SaveChanges();
                        dbcontext.Tasks.RemoveRange(dbcontext.Tasks.Where(u => u.Owner.Id == existing.Id));
                        dbcontext.SaveChanges();
                        dbcontext.HistoryUserRoles.RemoveRange(dbcontext.HistoryUserRoles.Where(u => u.User.Id == existing.Id));
                        dbcontext.SaveChanges();

                        dbcontext.Users.Remove(existing);
                        dbcontext.SaveChanges();
                        return UserGetModel.FromUser(existing);
                    }
                    return null;
                    //  dbcontext.Users.Update(toAdd);
                    //dbcontext.SaveChanges();
                    //return toAdd;
                }
                if (addedByCurrentRole.Equals(RoleConstants.ADMIN))
                {
                    dbcontext.Comments.RemoveRange(dbcontext.Comments.Where(u => u.Owner.Id == existing.Id));
                    dbcontext.SaveChanges();
                    dbcontext.Tasks.RemoveRange(dbcontext.Tasks.Where(u => u.Owner.Id == existing.Id));
                    dbcontext.SaveChanges();
                    dbcontext.HistoryUserRoles.RemoveRange(dbcontext.HistoryUserRoles.Where(u => u.User.Id == existing.Id));
                    dbcontext.SaveChanges();

                    dbcontext.Users.Remove(existing);
                    dbcontext.SaveChanges();
                    return UserGetModel.FromUser(existing);
                }
                return null;
            }

            private HistoryUserRole getLatestHistoryUserRole(IEnumerable<HistoryUserRole> allHistoryRole)
            {
                var latestHistoryUserRole = allHistoryRole.OrderByDescending(x => x.StartTime).FirstOrDefault();
                if (latestHistoryUserRole.EndTime == null)
                {
                    return latestHistoryUserRole;
                }
                return null;
            }


            private UserRole searchForRoleByTitle(string title)
            {
                IEnumerable<UserRole> roles = dbcontext.UserRoles;
                foreach (UserRole userRole in roles)
                {
                    if (userRole.Title.Equals(title))
                    {
                        return userRole;
                    }
                }
                return null;
            }


        }
    }
}
