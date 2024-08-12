using System;
using System.Collections.Generic;
using Lab3_NhatNguyen_WebApp.Services;
using Microsoft.EntityFrameworkCore;

namespace Lab3_NhatNguyen_WebApp.Models;

public partial class MovieWebAppContext : DbContext
{
    public MovieWebAppContext()
    {

    }

    public MovieWebAppContext(DbContextOptions<MovieWebAppContext> options)
        : base(options)
    {

    }

    public virtual DbSet<User> Users { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer($"Server=.\\;Database=MovieWebApp;Data Source=mssqlserveruser.cxfhmnqbeabx.ca-central-1.rds.amazonaws.com,1433;User ID={DBUsername};Password={DBPassword};Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Email).HasName("PK__Users__AB6E6165FC34B378");

            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("date")
                .HasColumnName("dateOfBirth");
            entity.Property(e => e.Displayname)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("displayname");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phonenumber");
            entity.Property(e => e.Userid)
                .ValueGeneratedOnAdd()
                .HasColumnName("userid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
