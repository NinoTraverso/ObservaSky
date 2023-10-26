using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Observasky.Models
{
    public partial class ModelDbContext : DbContext
    {
        public ModelDbContext()
            : base("name=ModelDbContext")
        {
        }

        public virtual DbSet<Articles> Articles { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<Glossary> Glossary { get; set; }
        public virtual DbSet<Guests> Guests { get; set; }
        public virtual DbSet<Lectures> Lectures { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lectures>()
                .HasMany(e => e.Guests)
                .WithOptional(e => e.Lectures)
                .HasForeignKey(e => e.LectureID);
        }
    }
}
