using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QRMenuAPI.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = "";

        [Column(TypeName = "nvarchar(200)")]
        [StringLength(200)]
        public string? Description { get; set; }

        public byte StateId { get; set; }
        [ForeignKey("StateId")]
        public State? State { get; set; }

        public int RestaurantId { get; set; }
        [ForeignKey("RestaurantId")]
        [JsonIgnore]
        public Restaurant? Restaurant { get; set; }

        public virtual List<Food>? Foods { get; set; }
    }
}

