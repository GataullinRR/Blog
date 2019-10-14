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
        public static void Initialize(
            BlogContext context, 
            RoleManager<IdentityRole> roleManager, 
            UserManager<User> userManager,
            SignInManager<User> signInManager)
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
                var radmir = new User()
                {
                    Email = "QTU100@yandex.ru",
                    UserName = "QTU100",
                    RegistrationDate = new DateTime(2018, 1, 20),
                    EmailConfirmed = true
                };
                userManager.CreateAsync(radmir, "QTU100@yandex.ru").Wait();
                userManager.AddToRoleAsync(radmir, Roles.ADMIN).Wait();

                var alex = new User()
                {
                    Email = "Sasha@yandex.ru",
                    UserName = "SashaKeny",
                    RegistrationDate = new DateTime(2010, 12, 9),
                    EmailConfirmed = true
                };
                userManager.CreateAsync(alex, "QTU100@yandex.ru").Wait();
                userManager.AddToRoleAsync(alex, Roles.USER).Wait();

                var ksenya = new User()
                {
                    Email = "Ksy_chemist@mail.ru",
                    UserName = "_KSY_",
                    RegistrationDate = new DateTime(2019, 3, 11),
                    EmailConfirmed = true
                };
                userManager.CreateAsync(ksenya, "QTU100@yandex.ru").Wait();
                userManager.AddToRoleAsync(ksenya, Roles.USER).Wait();

                var oleg = new User()
                {
                    Email = "Oleg@yota.ru",
                    UserName = "Oleg",
                    RegistrationDate = new DateTime(2012, 1, 5),
                    EmailConfirmed = true
                };
                userManager.CreateAsync(oleg, "QTU100@yandex.ru").Wait();
                userManager.AddToRoleAsync(oleg, Roles.MODERATOR).Wait();

                var radmirsPost1 = new Post()
                {
                    Author = radmir,
                    Title = "Why Is C# Among The Most Popular Programming Languages in The World",
                    Body = @"C# is a modern object-oriented programming language developed in 2000 by Anders Hejlsberg at Microsoft as a rival to Java (which it is quite similar to). It was created because Sun, (later bought by Oracle) did not want Microsoft to make changes to Java, so Microsoft chose to create their own language instead. C# has grown quickly since it was first created, with extensive support from Microsoft helping it to gain a large following; it is now one of the most popular programming languages in the world.
What is C#?
It is a general-purpose language designed for developing apps on the Microsoft platform and requires the .NET framework on Windows to work. C# is often thought of as a hybrid that takes the best of C and C++ to create a truly modernized language. Although the .NET framework supports several other coding languages, C# has quickly become one of the most popular.
C# can be used to create almost anything but is particularly strong at building Windows desktop applications and games. C# can also be used to develop web applications and has become increasingly popular for mobile development too. Cross-platform tools such as Xamarin allow apps written in C# to be used on almost any mobile device.",
                    Date = new DateTime(2018, 2, 1)
                };
                var post2 = new Post()
                {
                    Author = radmir,
                    Title = "ASP .NET MVC vs ASP .NET Core MVC",
                    Body = "While millions of web developers use ASP.NET MVC to build web applications, but the latest ASP.NET Core framework offers far more benefits than the ASP.NET MVC for web application development. ASP.NET Core is an open-source, cross-platform framework developed by both the Microsoft and its community.",
                    Date = new DateTime(2018, 2, 5, 12, 40, 23)
                };
                var post3 = new Post()
                {
                    Author = alex,
                    Title = "7 Habits for a Healthy Mind in a Healthy Body",
                    Body = @"To find clues for healthy living today, we must look to our past. The history of human evolution shows a definitive link between our physical health and psychological well-being. The Greeks understood the importance of a Sound Mind in a Sound Body. That credo became the foundation of their civilization. For clues on how we can best survive the 21st century we should look to the wisdom held in our ancestry and evolutionary biology.
In this entry I will explore ways in which modern living is causing our bodies and minds to short - circuit.I will recap the major periods of human evolution and offer a simple prescriptive that can insulate you from the ‘future shock’ that rapid advances in technology have created in our bodies, minds, and society. “Future shock” is a term for a certain psychological state of individuals and entire societies, introduced by Alvin Toffler in his book of the same name.Toffler's most basic definition of future shock is: “ too much change in too short a period of time.”  Do you feel future shocked? What ways are you coping with it?",
                    Date = new DateTime(2015, 5, 7, 22, 3, 10)
                };
                var post4 = new Post()
                {
                    Author = ksenya,
                    Title = "Why study Chemistry",
                    Body = @"Chemistry is an incredibly fascinating field of study. Because it is so fundamental to our world, chemistry plays a role in everyone's lives and touches almost every aspect of our existence in some way. Chemistry is essential for meeting our basic needs of food, clothing, shelter, health, energy, and clean air, water, and soil. Chemical technologies enrich our quality of life in numerous ways by providing new solutions to problems in health, materials, and energy usage. Thus, studying chemistry is useful in preparing us for the real world.
Chemistry is often referred to as the central science because it joins together physics and mathematics, biology and medicine, and the earth and environmental sciences. Knowledge of the nature of chemicals and chemical processes therefore provides insights into a variety of physical and biological phenomena. Knowing something about chemistry is worthwhile because it provides an excellent basis for understanding the physical universe we live in. For better or for worse, everything is chemical!",

                    Date = new DateTime(2019, 5, 17, 20, 3, 0)
                };
                var post5 = new Post()
                {
                    Author = ksenya,
                    Title = "17 Amazing Chemistry Facts that will Blow Your Mind",
                    Body = @"1. Lightning strikes produce Ozone, hence the characteristic smell after lightning storms

Ozone, the triple oxygen molecule that acts as a protective stratospheric blanket against ultraviolet rays, is created in nature by lightning. When it strikes, the lightning cracks oxygen molecules in the atmosphere into radicals which reform into ozone. The smell of ozone is very sharp, often described as similar to that of chlorine. This is why you get that “clean” smell sensation after a thunderstorm.

2. The only two non-silvery metals are gold and copper
Gold

A metal is an element that readily forms positive ions (cations) and has metallic bonds. These elements have electrons that are loosely held to the atoms, and will readily transfer them. This is why metals are great electrical and thermal conductors — because the electrons move energy.

Most metals’ electrons reflect colors equally, so the sun’s light is reflected as white. Gold and copper, however, happen to absorb blue and violet light, leaving yellow light. It’s worth noting here that copper is also the only metal that is naturally antibacterial.

3. Water expands when freezes, unlike other substances
water ice

Typically, when something is cold, it shrinks. That’s because temperature describes atomic vibration — the more vibration, the more space it takes, hence expansion. Water is an exception. Even though it vibrates less when it’s frozen, the ice occupies more volume. That’s due to the strange shape of the water molecule.

If you remember your Chemistry 101, the water molecule looks like Mickey Mouse, the oxygen atom sitting at the center (the face) and two hydrogen atoms each at an angle (Mickey’s ears). Because of how oxygen and hydrogen bond, the water molecule is an open structure with a lot of space. When water freezes it releases energy because a lot of extra strong bonds can be made.  But it does take up more space.  And so, ice expands when it freezes. Another interesting fact worth mentioning is that hot water freezes faster than cold water.

4. Glass is actually a liquid, it just flows very, very slowly
Mr. Freeze

It’s actually true, Mr. Freeze.

Being neither liquid, nor solid, explaining glass is a lot harder than some might think. In a glass, molecules still flow, but at a very low rate that it’s barely perceptible. As such, it’s not enough to class glasses as a liquid, but neither as a solid. Instead, chemists classify glasses as amorphous solids— a state somewhere between those two states of matter. There’s also a thing called metal glass – a class of materials that are three times stronger than titanium and have the elastic modulus of bone, all while being extremely lightweight

5. Every hydrogen atom in your body is likely 13.5 billion years old because they were created at the birth of the universe
big bang hydrogen

At ground zero, during the Universe’s singularity, the very first chemical element was hydrogen. All the other followed by fusing hydrogen into helium, which then fused into carbon and so on. Approximately 73% of the mass of the visible universe is in the form of hydrogen. Helium makes up about 25% of the mass, and everything else represents only 2%. By mass, hydrogen and helium combined make up less than 1% of the Earth.

6. Superfluid Helium defies gravity and climbs on walls
helium superfluid

A remarkable transition occurs in the properties of liquid helium at the temperature 2.17K (very close to absolute zero), called the “lambda point” for helium. Part of the liquid becomes a “superfluid”, a zero-viscosity fluid which will move rapidly through any pore in the apparatus.

7. If you pour a handful of salt into a glass of water, the water  level will go down
water and salt

When you step inside a bathtub, the water level will immediately go up, per Achimedes’ law. But when you add a volume of sodium chloride (salt) to a volume of water, the overall volume actually decreases by up to 2%. What gives? The net reduction in observed volume is due to solvent molecules which become more ordered in the vicinity of dissolved ions.

8. Diamond and graphite are both entirely made of carbon and nothing else
diamond

Though made of the same stuff, the difference between a crown jewel and pencil lead is given by form. Namely, diamond and graphite are arranged differently in space making them allotropes of carbon.

9. The rarest naturally-occurring element in the Earth’s crust is astatine
Astatine

Named after the Greek word for unstable (astatos), Astatine is a naturally occurring semi-metal produced from the decay of uranium and thorium. In its most stable form, the element has a half-time of only 8.1 hours. The entire crust appears to contain about 28 g of the element. If scientists ever have to use it, they basically have to make it from scratch. Only 0.00000005 grams of astatine have been made so far.

10. These buckyballs sell for $167 million per gram. The only thing more expensive in the world is antimatter
buckyballs
Credit: Oxford University
An Oxford startup recently sold endohedral fullerenes for $167 million per gram. According to Designer Carbon Materials – the only company in the world that manufactures this exotic material – it sold 200 micrograms of pure endohedral fullerenes for $33,400.

11. DNA is a flame retardant
DNA fire

DNA, also known as the blueprint for life, contains all the biological instructions that make each species unique. The molecule of life is also surprisingly sturdy, being considered a natural flame retardant and suppressant. Its flame retardant properties are due to DNA’s chemical structure — when heated, the phosphate-containing backbone produces phosphoric acid, which chemically removes water, leaving behind a flame-resistant, carbon-rich residue. Other bases, such as nitrogen, react to produce ammonia which inhibits combustion. In the future, researchers plan on coating fabric with DNA to make inflammable clothing.

12. One inch of rain is equal to 10 inches of snow
rain and snow

When the temperature is around 30 degrees F (0 degrees C), one inch of liquid precipitation would fall as 10 inches of snow — assuming the rainfall is all snow.

13. A rubber tire is technically one single, giant, polymerized molecule
rubber tire

Some molecules can be very big, but most are still microscopic. Not the vulcanized tire, though — it’s all one, big, freakin’ molecule! Basically, the vulcanized tire is all made of large polymers chains that have been crosslinked together with covalent bonds. 

14. Your car’s airbags are packed with salt sodium azide, which is very toxic
airbag

When a collision takes place, the car’s sensors trigger an electrical impulse which in the fraction of a second dramatically raises the temperature of the salts. These then decompose into harmless nitrogen gas, rapidly expanding the airbag.

15. Famed chemist Glenn Seaborg was the only person who could write his address in chemical elements
atom 

He would write Sg, Lr, Bk, Cf, Am. That’s  Seaborgium (Sg), named after Seaborg himself; Lawrencium (Lr), named after the Lawrence Berkeley National Laboratory;  Berkelium (Bk), named after the city of Berkeley, the home of UC Berkeley;  Californium (Cf), named after the state of California; Americium (Am), named after America.

16. Air becomes liquid at -190°C

Commonly, matter appears in one of the four states: solid, liquid, gas and plasma. The air we all breathe is gaseous but like any kind of matter, it can change its state when subjected to certain temperature and pressure. Air is a mixture of nitrogen, oxygen, and other gases. The gas can be liquefied by compression and cooling to extremely low temperatures — under normal atmospheric pressure, air has to be cooled to -200°C and under high pressure (typically 200 atmospheres) to -141°C to convert into liquid. Liquid air is used commercially for freezing other substances and especially as an intermediate step in the production of nitrogen, oxygen, and argon and the other inert gases.

17. Mars is red because of iron oxide

Credit: Pixabay.
While Earth is sometimes referred to as the ‘blue marble’ because it’s mostly covered in oceans and has a thick atmosphere, giving it a blue appearance, Mars is covered in a lot of iron oxide — these are the same compounds that give blood and rust their distinct color. In light of this, it’s no coincidence that Mars, which occasionally appears as a bright red ‘star’, was named after the Greek god of war.",
                    Date = new DateTime(2019, 5, 17, 20, 3, 0)
                };

                var commentary1 = new Commentary()
                {
                    Author = radmir,
                    Post = radmirsPost1,
                    Body = "C# is something which form base for any other language so it you are looking to be a product engineer, you need to have clarity on C#. Then there is Java or ASP.Net that you can explore.",
                    Date = radmirsPost1.Date.AddHours(78.9)
                };
                var commentary2 = new Commentary()
                {
                    Author = ksenya,
                    Post = post3,
                    Body = "Very interesting!",
                    Date = post3.Date.AddHours(178.1)
                };
                var commentary3 = new Commentary()
                {
                    Author = oleg,
                    Post = radmirsPost1,
                    Body = "Technology is cool!",
                    Date = radmirsPost1.Date.AddHours(2)
                };

                context.Posts.AddRange(radmirsPost1, post2, post3, post4, post5);
                context.Commentaries.AddRange(commentary1, commentary2, commentary3);
                post3.Edits = new List<PostEditInfo>()
                {
                    new PostEditInfo()
                    {
                        Author = radmir,
                        EditTime = post3.Date.AddHours(4),
                        Reason = "Removed typo"
                    } 
                };

                context.SaveChanges();
            }
        }
    }
}
