using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Web.Providers.Entities;


namespace DeliveryService.Domain.Entities
{
    public class Delivery
    {

        [Key]
        public Guid DeliveryId { get; set; }

        [Required]
        public DeliveryStatus Status { get; set; }

        [Required]
        public DateTime ETA { get; set; }

        [Required]
        public Guid OrderID { get; set; }

        [Required]
        public Guid User_ID { get; set; }

        // Navigation properties
        [ForeignKey("OrderID")]
        public virtual StackExchange.Redis.Order Order { get; set; }

        [ForeignKey("User_ID")]
        public virtual User User { get; set; }
    }
}
