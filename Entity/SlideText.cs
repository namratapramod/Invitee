using System.ComponentModel.DataAnnotations;

namespace Invitee.Entity
{
    public class SlideText : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Text { get; set; }
        [Required]
        public int MediaTemplateId { get; set; }
        public virtual MediaTemplate MediaTemplate { get; set; }
    }
}