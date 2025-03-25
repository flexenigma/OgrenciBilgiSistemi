﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OgrenciBilgiSistemi.Data;

#nullable disable

namespace OgrenciBilgiSistemi.Migrations
{
    [DbContext(typeof(VeriTabaniContext))]
    [Migration("20250325163835_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("OgrenciBilgiSistemi.Models.Ders", b =>
                {
                    b.Property<int>("DersId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("DersId"));

                    b.Property<bool>("AktifMi")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("DersAdi")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("DersKodu")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<int>("Kredi")
                        .HasColumnType("int");

                    b.HasKey("DersId");

                    b.ToTable("Dersler");
                });

            modelBuilder.Entity("OgrenciBilgiSistemi.Models.Kullanici", b =>
                {
                    b.Property<int>("KullaniciId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("KullaniciId"));

                    b.Property<string>("AdSoyad")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<bool>("AktifMi")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime?>("GuncellemeTarihi")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("KayitTarihi")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("KullaniciAdi")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("OlusturmaTarihi")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("RolId")
                        .HasColumnType("int");

                    b.Property<string>("Sifre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("KullaniciId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("KullaniciAdi")
                        .IsUnique();

                    b.HasIndex("RolId");

                    b.ToTable("Kullanicilar");
                });

            modelBuilder.Entity("OgrenciBilgiSistemi.Models.LogKaydi", b =>
                {
                    b.Property<int>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("LogId"));

                    b.Property<string>("Aciklama")
                        .HasColumnType("longtext");

                    b.Property<string>("IPAdresi")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("IslemTarihi")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("IslemTuru")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("KullaniciId")
                        .HasColumnType("int");

                    b.HasKey("LogId");

                    b.HasIndex("KullaniciId");

                    b.ToTable("LogKayitlari");
                });

            modelBuilder.Entity("OgrenciBilgiSistemi.Models.Ogrenci", b =>
                {
                    b.Property<int>("OgrenciId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("OgrenciId"));

                    b.Property<string>("AdSoyad")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<bool>("AktifMi")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Bolum")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("GirisTarihi")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("OgrenciNo")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Telefon")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.HasKey("OgrenciId");

                    b.HasIndex("OgrenciNo")
                        .IsUnique();

                    b.ToTable("Ogrenciler");
                });

            modelBuilder.Entity("OgrenciBilgiSistemi.Models.OgrenciDersKayit", b =>
                {
                    b.Property<int>("KayitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("KayitId"));

                    b.Property<int>("DersId")
                        .HasColumnType("int");

                    b.Property<string>("Donem")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime?>("GuncellemeTarihi")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("KayitTarihi")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("Not")
                        .HasColumnType("int")
                        .HasColumnName("Puan");

                    b.Property<int?>("NotDurumu")
                        .HasColumnType("int");

                    b.Property<int>("OgrenciId")
                        .HasColumnType("int");

                    b.HasKey("KayitId");

                    b.HasIndex("DersId");

                    b.HasIndex("OgrenciId");

                    b.ToTable("OgrenciDersKayitlari", (string)null);
                });

            modelBuilder.Entity("OgrenciBilgiSistemi.Models.Rol", b =>
                {
                    b.Property<int>("RolId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("RolId"));

                    b.Property<string>("RolAdi")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("RolId");

                    b.ToTable("Roller");
                });

            modelBuilder.Entity("OgrenciBilgiSistemi.Models.Kullanici", b =>
                {
                    b.HasOne("OgrenciBilgiSistemi.Models.Rol", "Rol")
                        .WithMany("Kullanicilar")
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Rol");
                });

            modelBuilder.Entity("OgrenciBilgiSistemi.Models.LogKaydi", b =>
                {
                    b.HasOne("OgrenciBilgiSistemi.Models.Kullanici", "Kullanici")
                        .WithMany()
                        .HasForeignKey("KullaniciId");

                    b.Navigation("Kullanici");
                });

            modelBuilder.Entity("OgrenciBilgiSistemi.Models.OgrenciDersKayit", b =>
                {
                    b.HasOne("OgrenciBilgiSistemi.Models.Ders", "Ders")
                        .WithMany("Kayitlar")
                        .HasForeignKey("DersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OgrenciBilgiSistemi.Models.Ogrenci", "Ogrenci")
                        .WithMany("Kayitlar")
                        .HasForeignKey("OgrenciId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ders");

                    b.Navigation("Ogrenci");
                });

            modelBuilder.Entity("OgrenciBilgiSistemi.Models.Ders", b =>
                {
                    b.Navigation("Kayitlar");
                });

            modelBuilder.Entity("OgrenciBilgiSistemi.Models.Ogrenci", b =>
                {
                    b.Navigation("Kayitlar");
                });

            modelBuilder.Entity("OgrenciBilgiSistemi.Models.Rol", b =>
                {
                    b.Navigation("Kullanicilar");
                });
#pragma warning restore 612, 618
        }
    }
}
