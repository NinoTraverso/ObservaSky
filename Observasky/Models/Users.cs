namespace Observasky.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;

    public partial class Users
    {
        [Key]
        public int IdUser { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "A password is required")]
        [StringLength(50)]
        public string Password { get; set; }

        [StringLength(20)]
        public string Role { get; set; }

        [Required(ErrorMessage = "An email is required")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please choose an image")]
        [NotMapped]
        public HttpPostedFileBase Image { get; set; }

        [StringLength(255)]
        public string Photo { get; set; }
    }
}
