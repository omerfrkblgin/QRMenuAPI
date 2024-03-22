using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRMenuAPI.Models
{
    public class State
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] //veritabanında identity olmadığını belirler
        public byte Id { get; set; } //kullanıcının başlangıçta boşluk girmesini onler 1 karakter girildiğinde devredışı kalır
        [StringLength(10)] //maximum uzunluğu belirler
        [Column(TypeName = "nvarchar(10)")]
        public string Name { get; set; } = "";

        
    }
}

