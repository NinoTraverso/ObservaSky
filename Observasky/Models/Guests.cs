namespace Observasky.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Guests
    {
        [Key]
        public int IdBooking { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }

        public int? NumberOfGuests { get; set; }

        public int? LectureID { get; set; }

        public virtual Lectures Lectures { get; set; }
    }
}
