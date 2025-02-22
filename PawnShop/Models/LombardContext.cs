using System.Data.Entity;

namespace PawnShop.Models
{
    public class LombardContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Pledge> Pledges { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Employees> Employees { get; set; }


        public LombardContext() : base("Data Source=USER-2RABC32TPE\\SQLEXPRESS;Initial Catalog=Pawnshop;Integrated Security=True")
        {
            Configuration.LazyLoadingEnabled = false; // Отключение ленивой загрузки
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи Transactions -> Pledge
            modelBuilder.Entity<Transactions>()
                .HasRequired(t => t.Pledge)         // Связь обязательная
                .WithMany()                         // У Pledge может быть много Transactions
                .HasForeignKey(t => t.PledgeID);    // Внешний ключ

            // Конфигурация для Pledge
            modelBuilder.Entity<Pledge>()
                .Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}

