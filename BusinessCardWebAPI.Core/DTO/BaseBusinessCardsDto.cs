using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardWebAPI.Core.DTO
{
    public class BaseBusinessCardsDto
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Photo { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
        
}

