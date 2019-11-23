using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace DBModels
{
    enum Interface
    {
        MODERATABLE_OBJECT
    }

    enum Implementation
    {
        COMMENTARY,
        PROFILE,
        POST
    }

    public class FinderToken
    {
        internal FinderToken(Interface interfaceType, Implementation implementationType, int id)
        {
            InterfaceType = interfaceType;
            ImplementationType = implementationType;
            Id = id;
        }

        internal Interface InterfaceType { get; }
        internal Implementation ImplementationType { get; }
        internal int Id { get; }

        public string SerializeToString()
        {
            return $"{InterfaceType.To<int>()}--{ImplementationType.To<int>()}--{Id}";
        }

        public static FinderToken DeserializeFromString(string serialized)
        {
            var parts = serialized.Split("--");
            return new FinderToken(
                EnumUtils.CastSafe<Interface>(parts[0].ParseToInt32Invariant()),
                EnumUtils.CastSafe<Implementation>(parts[1].ParseToInt32Invariant()),
                parts[2].ParseToInt32Invariant());
        }
    }

    public static class Extensions
    {
        public static FinderToken GetFinderToken(this IReportable @object)
        {
            var implementation = @object.Select(
                (v => v is Commentary, Implementation.COMMENTARY),
                (v => v is Profile, Implementation.PROFILE),
                (v => v is Post, Implementation.POST)).Single();
            
            return new FinderToken(Interface.MODERATABLE_OBJECT, implementation, @object.Id);
        }

        public static async Task<T> FindObjectByTokenOrNullAsync<T>(this BlogContext context, FinderToken token)
            where T : class
        {
            object result;
            switch (token.InterfaceType)
            {
                case Interface.MODERATABLE_OBJECT:
                    switch (token.ImplementationType)
                    {
                        case Implementation.COMMENTARY:
                            result = await context.Commentaries.FirstOrDefaultByIdAsync(token.Id).ThenDo(r => r.To<T>());
                            break;
                        case Implementation.PROFILE:
                            result = await context.ProfilesInfos.FirstOrDefaultByIdAsync(token.Id).ThenDo(r => r.To<T>());
                            break;
                        case Implementation.POST:
                            result = await context.Posts.FirstOrDefaultByIdAsync(token.Id).ThenDo(r => r.To<T>());
                            break;
                     
                        default:
                            throw new NotSupportedException();
                    }
                    break;
             
                default:
                    throw new NotSupportedException();
            }
            
            return (T)result;
        }

        public static async Task<T> FirstOrDefaultByIdAsync<T>(this DbSet<T> entities, int id)
            where T : class, IDbEntity
        {
            return await entities.FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
