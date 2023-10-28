namespace Observasky.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Glossary")]
    public partial class Glossary
    {
        [Key]
        public int IdGlossary { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
