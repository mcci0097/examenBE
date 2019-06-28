using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using templateExamen;
using templateExamen.Constants;
using templateExamen.Models;
using templateExamen.Services;
using templateExamen.ViewModels;

namespace Tests
{
    public class UsersServiceTest
    {
        private IOptions<AppSettings> config;

        [SetUp]
        public void Setup()
        {
            config = Options.Create(new AppSettings
            {
                Secret = "dsadhjcghduihdfhdifd8ihadandwqdqfefefqwfq"
            });
        }


        [Test]
        public void Register()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(Register))
              .Options;

            using (var context = new UsersDbContext(options))
            {
                var userService = new UserService(context, config);
                var addedUser = new RegisterPostModel
                {
                    Email = "pop@yahoo.com",
                    FirstName = "Pop",
                    LastName = "Mihai",
                    Password = "pop123456",
                    Username = "popmihai01",
                    DateRegister = DateTime.Now,
                };

                var result = userService.Register(addedUser);

                Assert.IsNotNull(result);
                Assert.AreEqual(addedUser.Username, result.Username);
            }
        }

        [Test]
        public void Login()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(Login))
              .Options;

            using (var context = new UsersDbContext(options))
            {
                var userService = new UserService(context, config);

                var added = new RegisterPostModel
                {
                    Email = "lucian@yahoo.com",
                    FirstName = "Gavrilut",
                    LastName = "Lucian",
                    Password = "12345678",
                    Username = "glucian"
                };
                userService.Register(added);
                var loggedIn = new LoginPostModel
                {
                    Username = "glucian",
                    Password = "12345678"
                };
                var result = userService.Authenticate(added.Username, added.Password);

                Assert.IsNotNull(result);
                //Assert.AreEqual(7, result.Id);
                Assert.AreEqual(loggedIn.Username, result.Username);
            }
        }

        [Test]
        public void GetAll()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetAll))
              .Options;

            using (var context = new UsersDbContext(options))
            {
                //context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                var userService = new UserService(context, config);

                var added = new RegisterPostModel
                {
                    Email = "lucian@yahoo.com",
                    FirstName = "Gavrilut",
                    LastName = "Lucian",
                    Password = "12345678",
                    Username = "glucian"
                };
                var added1After = userService.Register(added);
            }

            using (var context = new UsersDbContext(options))
            {
                var userService = new UserService(context, config);
                var added = new RegisterPostModel
                {
                    Email = "pop@yahoo.com",
                    FirstName = "Pop",
                    LastName = "Mihai",
                    Password = "pop123456",
                    Username = "pmihai"
                };
                var added2After = userService.Register(added);
            }

            using (var context = new UsersDbContext(options))
            {
                var userService = new UserService(context, config);
                var added = new RegisterPostModel
                {
                    Email = "aconstantinesei@yahoo.com",
                    FirstName = "Luminita",
                    LastName = "Aconstantinesei",
                    Password = "1234567",
                    Username = "laconstantinesei"
                };
                var added2After = userService.Register(added);
            }

            using (var context = new UsersDbContext(options))
            {
                var userService = new UserService(context, config);
                var result = userService.GetAll();
                Assert.AreEqual(3, result.Count());
            }
        }

        [Test]
        public void GetAllHistoryUserRole()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetAllHistoryUserRole))
              .Options;

            using (var context = new UsersDbContext(options))
            {
                var userService = new UserService(context, config);

                var added = new HistoryUserRole
                {
                    User = new User
                    {
                        Username = "Mircea",
                    },
                    UserRole = new UserRole
                    {
                        Name = RoleConstants.REGULAR
                    },
                    StartTime = DateTime.Now,
                    EndTime = null
                };

                var added2 = new HistoryUserRole
                {
                    User = new User
                    {
                        Username = "Claudiu",
                    },
                    UserRole = new UserRole
                    {
                        Name = RoleConstants.MODERATOR
                    },
                    StartTime = DateTime.Now.AddDays(10),
                    EndTime = null
                };

                context.HistoryUserRoles.Add(added);
                context.HistoryUserRoles.Add(added2);
                context.SaveChanges();

                var result = userService.GetAllHistory();

                Assert.AreEqual(2, result.Count());
                Assert.AreNotEqual(0, result.Count());
            }
        }

        [Test]
        public void Upsert()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(Upsert))
              .Options;

            using (var context = new UsersDbContext(options))
            {
                var usersService = new UserService(context, config);

                var defaultuserRole = new UserRole
                {
                    Name = RoleConstants.REGULAR
                };
                context.UserRoles.Add(defaultuserRole);
                context.Entry(defaultuserRole).State = EntityState.Detached;

                var defaultuserRole2 = new UserRole
                {
                    Name = RoleConstants.ADMIN
                };
                context.UserRoles.Add(defaultuserRole2);
                context.Entry(defaultuserRole2).State = EntityState.Detached;

                UserRole userRoleRegular = new UserRole
                {
                    Name = RoleConstants.REGULAR
                };
                HistoryUserRole historyRegular = new HistoryUserRole
                {
                    UserRole = userRoleRegular,
                };
                List<HistoryUserRole> listRegular = new List<HistoryUserRole>
                {
                    historyRegular
                };
                User radu69 = new User
                {
                    Username = "radu96",
                    HistoryUserRole = listRegular,
                    DateRegister = DateTime.Now,
                    Password = "radu123456",
                    Email = "radu@yahoo.com",
                    FirstName = "radu",
                    LastName = "radu"
                };
                context.Users.Add(radu69);
                context.SaveChanges();
            }

            using (var context = new UsersDbContext(options))
            {
                var usersService = new UserService(context, config);

                var defaultuserRole = new UserRole
                {
                    Name = RoleConstants.ADMIN
                };
                context.UserRoles.Add(defaultuserRole);
                context.SaveChanges();
                context.Entry(defaultuserRole).State = EntityState.Detached;

                UserRole userRole = new UserRole
                {
                    Name = RoleConstants.ADMIN
                };
                HistoryUserRole history = new HistoryUserRole
                {
                    UserRole = userRole,
                };
                List<HistoryUserRole> list = new List<HistoryUserRole>
                {
                    history
                };
                User user2 = new User
                {
                    Username = "cristina91",
                    HistoryUserRole = list,
                    DateRegister = DateTime.Now,
                    Password = "cristina123456",
                    Email = "cristina@yahoo.com",
                    FirstName = "cristina",
                    LastName = "cristina"
                };

                User firstAdded = new User
                {
                    Username = "radu96",
                    DateRegister = DateTime.Now,
                    Password = "radu123456",
                    Email = "radu@yahoo.com",
                    FirstName = "radu",
                    LastName = "radu"
                };

                var toUpdateWith = new UserPostModel
                {
                    FirstName = "Gimli",
                    LastName = "Axeman",
                    UserName = "gimli",
                    Email = "gimli@gmail.com",
                    Password = "gimli123456",
                    UserRole = RoleConstants.ADMIN
                };

                var result = context.ChangeTracker.Entries()
                    .Where(t => t.State == EntityState.Unchanged);

                UserGetModel lalala = usersService.Upsert(1, toUpdateWith, user2);

                Assert.AreNotEqual(lalala.Username, firstAdded.Username);
                Assert.AreNotEqual(lalala.Email, firstAdded.Email);
                Assert.AreNotEqual(lalala.UserRole, RoleConstants.ADMIN);
            }
        }

        [Test]
        public void Delete()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(Delete))
              .Options;

            using (var context = new UsersDbContext(options))
            {
                var usersService = new UserService(context, config);
                UserRole userRole = new UserRole
                {
                    Id = 3,
                    Name = RoleConstants.ADMIN
                };

                HistoryUserRole history = new HistoryUserRole
                {
                    StartTime = DateTime.Now,
                    UserRoleId = 3,
                    UserRole = userRole,
                };
                List<HistoryUserRole> list = new List<HistoryUserRole>
                {
                    history
                };

                User user1 = new User
                {
                    Username = "alex69",
                    HistoryUserRole = list,
                    DateRegister = DateTime.Now

                };
                //aragorn.History.Add(history);

                var addedCosmin = new RegisterPostModel
                {
                    Email = "cosmin@yahoo.com",
                    FirstName = "Cosmin",
                    LastName = "Cosmin",
                    Password = "cosmin123456",
                    Username = "cosmin91"
                };
                usersService.Register(addedCosmin);
                User expected = context.Users.AsNoTracking()
                    .Include(x => x.HistoryUserRole)
                    .ThenInclude(x => x.UserRole)
                    .FirstOrDefault(x => x.FirstName.Equals(addedCosmin.FirstName));

                usersService.Delete(expected.Id, user1);

                Assert.IsNull(usersService.GetById(expected.Id));
            }
        }

        [Test]
        public void GetById()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetById))
              .Options;

            using (var context = new UsersDbContext(options))
            {
                var usersService = new UserService(context, config);
                var defaultuserRole = new UserRole
                {
                    Name = RoleConstants.ADMIN
                };
                context.UserRoles.Add(defaultuserRole);
                context.SaveChanges();
                context.Entry(defaultuserRole).State = EntityState.Detached;

                UserRole userRole = new UserRole
                {
                    Name = RoleConstants.ADMIN
                };
                HistoryUserRole history = new HistoryUserRole
                {
                    UserRole = userRole,
                };
                List<HistoryUserRole> list = new List<HistoryUserRole>
                {
                    history
                };
                User user = new User
                {
                    Username = "trulalalala",
                    HistoryUserRole = list,
                    DateRegister = DateTime.Now,
                    Password = "1234567",
                    Email = "trolalal@yahoo.com",
                    FirstName = "trolallaa",
                    LastName = "tralalalal"
                };
                context.Users.Add(user);
                context.SaveChanges();

                User expected = context.Users.AsNoTracking()                    
                    .FirstOrDefault(x => x.FirstName.Equals(user.FirstName));
                UserGetModel actual = usersService.GetById(expected.Id);

                Assert.AreEqual(expected.Username, actual.Username);
                Assert.AreEqual(expected.Id, actual.Id);
            }
        }
    }
}