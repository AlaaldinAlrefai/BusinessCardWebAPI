using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardWebAPI.Core.Data
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        
        [StringLength(50)]
        public string UserName { get; set; }


        public string PasswordHash { get; set; }

        
        [StringLength(20)]
        public string Role { get; set; }

        
        [StringLength(100)]
        public string Email { get; set; }

        [Column(TypeName ="datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;




    }
}
