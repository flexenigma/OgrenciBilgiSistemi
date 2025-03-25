using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Models;

namespace OgrenciBilgiSistemi.Data
{
    public class VeriTabaniContext : DbContext
    {
        public VeriTabaniContext(DbContextOptions<VeriTabaniContext> options)
            : base(options)
        {
        }

        public DbSet<Ogrenci> Ogrenciler { get; set; }
        public DbSet<Ders> Dersler { get; set; }
        public DbSet<OgrenciDersKayit> OgrenciDersKayitlari { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Rol> Roller { get; set; }
        public DbSet<LogKaydi> LogKayitlari { get; set; }

        // İlişkileri ve tabloları yapılandırma
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Öğrenci-Ders ilişkisi (Çoka-çok)
            modelBuilder.Entity<OgrenciDersKayit>()
                .ToTable("OgrenciDersKayitlari")
                .HasKey(ok => ok.KayitId);

            modelBuilder.Entity<OgrenciDersKayit>()
                .HasOne(ok => ok.Ogrenci)
                .WithMany(o => o.Kayitlar)
                .HasForeignKey(ok => ok.OgrenciId);

            modelBuilder.Entity<OgrenciDersKayit>()
                .HasOne(ok => ok.Ders)
                .WithMany(d => d.Kayitlar)
                .HasForeignKey(ok => ok.DersId);

            // Kullanıcı-Rol ilişkisi (Bire-çok)
            modelBuilder.Entity<Kullanici>()
                .HasOne(k => k.Rol)
                .WithMany(r => r.Kullanicilar)
                .HasForeignKey(k => k.RolId);

            // Benzersiz alanları belirleme
            modelBuilder.Entity<Ogrenci>()
                .HasIndex(o => o.OgrenciNo)
                .IsUnique();

            modelBuilder.Entity<Kullanici>()
                .HasIndex(k => k.KullaniciAdi)
                .IsUnique();

            modelBuilder.Entity<Kullanici>()
                .HasIndex(k => k.Email)
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "server=localhost;port=3306;database=OgrenciBilgiSistemi;user=root;password=Ss123123;CharSet=utf8mb4;";
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
            
            base.OnConfiguring(optionsBuilder);
        }
    }
}