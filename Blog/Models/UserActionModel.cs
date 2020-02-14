using DBModels;
using System;

namespace Blog.Models
{
    public class UserActionModel
    {
        public DateTime ActionDate { get; set; }
        public ActionType Type { get; set; }
        public object ActionObject { get; set; }
        public bool IsSelfAction { get; set; }
    }
}
