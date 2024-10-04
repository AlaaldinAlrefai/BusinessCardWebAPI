using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardWebAPI.Core.Data
{
    public class ExportLogs
    {
        [Key]
        public int Id { get; set; }

        public DateTime ExportDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string FileType { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }

        public Users Users { get; set; }
    }
}
