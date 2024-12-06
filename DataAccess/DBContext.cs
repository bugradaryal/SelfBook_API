using System;
using DataAccess.SeedData;
using Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



namespace DataAccess
{
    public class DBContext : IdentityDbContext<User>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=.\\SQLExpress; Database=SelfBook; Trusted_Connection=True; MultipleActiveResultSets=true; TrustServerCertificate=True;");
        }

        public DbSet<User> Users { get; set; }

        public DbSet<UserLibary> UserLibaries { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Category> Categories { get; set; }

        //FLUENT API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().Property(x => x.FirstName).HasColumnType("nvarchar(120)").IsRequired();
            modelBuilder.Entity<User>().Property(x => x.LastName).HasColumnType("nvarchar(120)").IsRequired();
            modelBuilder.Entity<User>().Property(x => x.Gender).HasColumnType("nvarchar(15)").HasDefaultValue("Undefined");

            modelBuilder.Entity<UserLibary>().HasKey(x => x.id);
            modelBuilder.Entity<UserLibary>().Property(x => x.id).UseIdentityColumn();
            modelBuilder.Entity<UserLibary>().Property(x => x.user_id).HasColumnType("nvarchar(450)").IsRequired();
            modelBuilder.Entity<UserLibary>().Property(x => x.book_id).IsRequired();
            modelBuilder.Entity<UserLibary>().Property(x => x.current_page).HasDefaultValue(0);
            modelBuilder.Entity<UserLibary>().Property(x => x.add_date).HasColumnType("date").HasDefaultValue(DateOnly.FromDateTime(DateTime.Now));

            //////////////////////////////////////////////

            modelBuilder.Entity<Category>().HasKey(x => x.id);
            modelBuilder.Entity<Category>().Property(x => x.id).UseIdentityColumn();
            modelBuilder.Entity<Category>().Property(x => x.category).HasColumnType("varchar(80)").IsRequired().HasMaxLength(80);

            //////////////////////////////////////////////

            modelBuilder.Entity<Book>().HasKey(x => x.id);
            modelBuilder.Entity<Book>().Property(x => x.id).UseIdentityColumn();

            modelBuilder.Entity<Book>().Property(x => x.name).HasColumnType("varchar(160)").IsRequired();

            modelBuilder.Entity<Book>().Property(x => x.author).HasColumnType("varchar(160)").HasDefaultValue("Anonymous");

            modelBuilder.Entity<Book>().Property(x => x.release_date).HasColumnType("date").HasDefaultValue(DateOnly.FromDateTime(DateTime.Now));

            modelBuilder.Entity<Book>().Property(x => x.category_id).IsRequired();

            modelBuilder.Entity<Book>().Property(x => x.page).HasDefaultValue(0);

            modelBuilder.Entity<Book>().Property(x => x.image).HasColumnType("varbinary(max)");

            modelBuilder.Entity<Book>().Property(x => x.pdf).HasColumnType("varbinary(max)");

            //////////////////////////////////////////////

            modelBuilder.Entity<User>().HasMany(x => x.userlibary).WithOne(x => x.user).HasForeignKey(x => x.user_id);

            modelBuilder.Entity<Category>().HasOne(x => x.book).WithOne(x => x.b_category).HasForeignKey<Book>(x => x.category_id);

            modelBuilder.Entity<Book>().HasMany(x => x.userlibary).WithOne(x => x.book).HasForeignKey(x => x.book_id);  

            modelBuilder.ApplyConfiguration(new SeedData.InsertCategories());
            modelBuilder.ApplyConfiguration(new SeedData.InsertIdentity());

        }
    }
}
