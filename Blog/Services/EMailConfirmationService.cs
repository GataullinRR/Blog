using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Services
{
    public class EMailConfirmationService
    {
        public string GetConfirmationToken(User user)
        {
            var randomSeed = user.Nickname.Select(c => (int)c).Sum();
            var rnd = new Random(randomSeed);
            var shaked = user.EMail.Shake(rnd).Aggregate();
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(shaked)).ToBase64();
            }
        }
    }
}
