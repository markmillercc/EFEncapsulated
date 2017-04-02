namespace EFEncapsulated.DAL
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using EFEncapsulated.Extensions;

    public class Model1Context : DbContext
    {
        public Model1Context()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasMany<Order, OrderLineItem>("_LineItems");

            base.OnModelCreating(modelBuilder);
        }
    }
}