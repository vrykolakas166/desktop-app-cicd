﻿#nullable disable

namespace NETX.Core.Models
{
    public class Setting
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
