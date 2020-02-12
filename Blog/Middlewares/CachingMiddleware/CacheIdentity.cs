using System;
using System.Collections.Generic;
using System.Linq;

namespace Blog.Middlewares
{
    class CacheIdentity : IEquatable<CacheIdentity>
    {
        public static bool operator ==(CacheIdentity left, CacheIdentity right)
        {
            return EqualityComparer<CacheIdentity>.Default.Equals(left, right);
        }
        public static bool operator !=(CacheIdentity left, CacheIdentity right)
        {
            return !(left == right);
        }

        public string Path { get; }

        public CacheIdentity(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CacheIdentity);
        }

        public bool Equals(CacheIdentity other)
        {
            return other != null &&
                   Path == other.Path;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Path);
        }
    }
}
