using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;

namespace DBModels
{
    public interface IDbEntity
    {
        int Id { get; set; }
    }
}
