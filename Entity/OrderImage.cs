using System.ComponentModel.DataAnnotations;

namespace Invitee.Entity
{
    public class OrderImage
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        [Required]
        public string ImagePath { get; set; }
        public long ImageSize { get; set; }

    }
}