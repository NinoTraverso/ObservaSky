namespace Observasky.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Articles
    {
        [Key]
        public int IdArticle { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public string Introduction { get; set; }

        [StringLength(255)]
        public string Photo { get; set; }

        public string Main { get; set; }

        public string Conclusions { get; set; }

        [StringLength(100)]
        public string Author { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }
    }
}
