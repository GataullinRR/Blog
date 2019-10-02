using DBModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog
{
    class DbSampleData
    {
        public static void Initialize(BlogContext context, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            if (!roleManager.Roles.Any())
            {
                foreach (var roleName in Roles.AllRoles)
                {
                    roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
                }
            }

            if (!context.Users.Any())
            {
                var user1 = new User()
                {
                    Email = "QTU100@gmail.com",
                    UserName = "QTU100",
                    PasswordHash = "123",
                    RegistrationDate = new DateTime(2018, 1, 20)
                };
                userManager.AddToRoleAsync(user1, Roles.ADMIN);

                var user2 = new User()
                {
                    Email = "Sasha@yandex.ru",
                    UserName = "SashaKeny",
                    PasswordHash = "123",
                    RegistrationDate = new DateTime(2010, 12, 9)
                };
                userManager.AddToRoleAsync(user2, Roles.UNCONFIRMED);

                var user3 = new User()
                {
                    Email = "Ksy_chemist@mail.ru",
                    UserName = "_KSY_",
                    PasswordHash = "123",
                    RegistrationDate = new DateTime(2019, 3, 11)
                };
                userManager.AddToRoleAsync(user3, Roles.USER);

                var post1 = new Post()
                {
                    Author = user1,
                    Title = "Why Is C# Among The Most Popular Programming Languages in The World",
                    Body = @"C# is a modern object-oriented programming language developed in 2000 by Anders Hejlsberg at Microsoft as a rival to Java (which it is quite similar to). It was created because Sun, (later bought by Oracle) did not want Microsoft to make changes to Java, so Microsoft chose to create their own language instead. C# has grown quickly since it was first created, with extensive support from Microsoft helping it to gain a large following; it is now one of the most popular programming languages in the world.
What is C#?
It is a general-purpose language designed for developing apps on the Microsoft platform and requires the .NET framework on Windows to work. C# is often thought of as a hybrid that takes the best of C and C++ to create a truly modernized language. Although the .NET framework supports several other coding languages, C# has quickly become one of the most popular.
C# can be used to create almost anything but is particularly strong at building Windows desktop applications and games. C# can also be used to develop web applications and has become increasingly popular for mobile development too. Cross-platform tools such as Xamarin allow apps written in C# to be used on almost any mobile device.",
                    Date = new DateTime(2018, 2, 1)
                };
                var post2 = new Post()
                {
                    Author = user1,
                    Title = "ASP .NET MVC vs ASP .NET Core MVC",
                    Body = "While millions of web developers use ASP.NET MVC to build web applications, but the latest ASP.NET Core framework offers far more benefits than the ASP.NET MVC for web application development. ASP.NET Core is an open-source, cross-platform framework developed by both the Microsoft and its community.",
                    Date = new DateTime(2018, 2, 5, 12, 40, 23)
                };
                var post3 = new Post()
                {
                    Author = user2,
                    Title = "7 Habits for a Healthy Mind in a Healthy Body",
                    Body = @"To find clues for healthy living today, we must look to our past. The history of human evolution shows a definitive link between our physical health and psychological well-being. The Greeks understood the importance of a Sound Mind in a Sound Body. That credo became the foundation of their civilization. For clues on how we can best survive the 21st century we should look to the wisdom held in our ancestry and evolutionary biology.
In this entry I will explore ways in which modern living is causing our bodies and minds to short - circuit.I will recap the major periods of human evolution and offer a simple prescriptive that can insulate you from the ‘future shock’ that rapid advances in technology have created in our bodies, minds, and society. “Future shock” is a term for a certain psychological state of individuals and entire societies, introduced by Alvin Toffler in his book of the same name.Toffler's most basic definition of future shock is: “ too much change in too short a period of time.”  Do you feel future shocked? What ways are you coping with it?",
                    Date = new DateTime(2015, 5, 7, 22, 3, 10)
                };
                var post4 = new Post()
                {
                    Author = user3,
                    Title = "Why study Chemistry",
                    Body = @"Chemistry is an incredibly fascinating field of study. Because it is so fundamental to our world, chemistry plays a role in everyone's lives and touches almost every aspect of our existence in some way. Chemistry is essential for meeting our basic needs of food, clothing, shelter, health, energy, and clean air, water, and soil. Chemical technologies enrich our quality of life in numerous ways by providing new solutions to problems in health, materials, and energy usage. Thus, studying chemistry is useful in preparing us for the real world.
Chemistry is often referred to as the central science because it joins together physics and mathematics, biology and medicine, and the earth and environmental sciences. Knowledge of the nature of chemicals and chemical processes therefore provides insights into a variety of physical and biological phenomena. Knowing something about chemistry is worthwhile because it provides an excellent basis for understanding the physical universe we live in. For better or for worse, everything is chemical!",

                    Date = new DateTime(2019, 5, 17, 20, 3, 0)
                };

                var commentary1 = new Commentary()
                {
                    Author = user1,
                    Post = post1,
                    Body = "C# is something which form base for any other language so it you are looking to be a product engineer, you need to have clarity on C#. Then there is Java or ASP.Net that you can explore.",
                    Date = post1.Date.AddHours(78.9)
                };
                var commentary2 = new Commentary()
                {
                    Author = user3,
                    Post = post3,
                    Body = "Very interesting!",
                    Date = post3.Date.AddHours(178.1)
                };

                context.Users.AddRange(user1, user2, user3);
                context.Posts.AddRange(post1, post2, post3, post4);
                context.Commentaries.AddRange(commentary1, commentary2);

                context.SaveChanges();
            }
        }
    }
}
