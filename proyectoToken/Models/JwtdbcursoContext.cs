using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace proyectoToken.Models;

public partial class JwtdbcursoContext : DbContext
{
    public JwtdbcursoContext()
    {
    }

    public JwtdbcursoContext( DbContextOptions<JwtdbcursoContext> options )
        : base(options)
    {
    }

    public virtual DbSet<HistoryRefreshToken> HistoryRefreshTokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder ) { }

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<HistoryRefreshToken>(entity =>
        {
            entity.HasKey(e => e.IdHistoryToken).HasName("PK__HistoryR__07DD6EBDF77F7847");

            entity.ToTable("HistoryRefreshToken");

            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .HasComputedColumnSql("(case when [ExpirationDate]<getdate() then CONVERT([bit],(0)) else CONVERT([bit],(1)) end)", false)
                .HasColumnName("isActive");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.HistoryRefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__HistoryRe__UserI__398D8EEE");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC7C2CBC22");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Pass)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial( ModelBuilder modelBuilder );
}
