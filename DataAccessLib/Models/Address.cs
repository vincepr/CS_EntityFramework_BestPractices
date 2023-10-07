using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLib.Models;
public class Address
{
    public int Id { get; set; }
    
    [Required, MaxLength(200)]
    public string StreetAddress { get; set; }
    
    [Required, MaxLength(100)]
    public string City { get; set; }
    
    [Required, MaxLength(50)]
    public string State { get; set; }
   
    // We can put them in same section or stack them:
    [Required]
    [MaxLength]
    [Column(TypeName = "varchar(10)")]  // we can basically write raw sql like this
    public string ZipCode { get; set; }
}