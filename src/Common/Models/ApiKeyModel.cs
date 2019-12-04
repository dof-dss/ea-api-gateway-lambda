using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class ApiKeyModel
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string CustomerId { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
    }
}
