using Microsoft.EntityFrameworkCore;

namespace AppSecuriAndContainer.Models
{
    public partial class UserDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            if (!optionsBuilder.IsConfigured)
            { optionsBuilder.UseNpgsql("Host=localhost;Database=MyDbLast;Username=example;Password=example").UseLazyLoadingProxies().LogTo(Console.WriteLine); }
        }

       public UserDbContext() { }

        /*        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
                    optionsBuilder.UseNpgsql("Host=localhost;Database=MyDbLast;Username=example;Password=example")
                    .UseLazyLoadingProxies()
                    .LogTo(Console.WriteLine);*/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(x => x.Id).HasName("users_pkey");
                e.Property(x => x.UserName).HasMaxLength(50);
                e.HasIndex(x => x.UserName).IsUnique();
                e.HasOne(e => e.Role).WithMany(e => e.Users).HasForeignKey(e => e.RoleId);
                e.Property(x => x.RoleId).HasConversion<int>();
            });
            modelBuilder.Entity<Role>(e =>
            {
                e.ToTable("roles");
                e.Property(x => x.RoleId).HasConversion<int>();
                e.HasData(Enum.GetValues(typeof(RoleId)).Cast<RoleId>().Select(x => new Role { RoleId = x, Name = x.ToString() }).ToList());


            });
        }


    }
}
