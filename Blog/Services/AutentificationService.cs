using DBModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Utilities;
using Utilities.Extensions;

namespace Blog.Services
{
    public class AutentificationService
    {
        readonly BlogContext _db;

        public AutentificationService(BlogContext db)
        {
            _db = db;
        }


        public async Task<User> GetCurrentUserAsync(HttpContext httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                return await _db.Users.FirstAsync(u => u.Nickname == httpContext.User.Identity.Name);
            }
            else
            {
                throw new InvalidOperationException("User does not exist");
            }
        }

        public async Task<bool> TryAuthenticateAsync(HttpContext httpContext, string userName, string password)
        {
            //https://metanit.com/sharp/aspnet5/15.2.php

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Nickname == userName);
            if (user == null)
            {
                return false;
            }
            else
            {
                if (GetHash(password) == user.PasswordHash)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, Role.USER.GetRoleName())
                    };
                    var id = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    await httpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(id));

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task LogoutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public string GetHash(string password)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
