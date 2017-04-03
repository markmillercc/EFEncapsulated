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

            // Map _LineItems explicitly by providing the property name                       
            modelBuilder.Entity<Order>()
                .HasMany<Order, OrderLineItem>("_LineItems");


            // Map _LineItems implicitly by passing the public, readOnly IEnumerable to which it backs
            modelBuilder.Entity<Order>()
                .HasMany(a => a.LineItems);

            base.OnModelCreating(modelBuilder);
        }
    }
}