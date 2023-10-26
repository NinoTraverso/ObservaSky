namespace Observasky.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Lectures
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lectures()
        {
            Guests = new HashSet<Guests>();
        }

        [Key]
        public int IdLecture { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public DateTime? DateTime { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [StringLength(255)]
        public string Photo { get; set; }

        public int? Seats { get; set; }

        [StringLength(100)]
        public string Speakers { get; set; }

        [StringLength(50)]
        public string Topic { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Guests> Guests { get; set; }
    }
}
