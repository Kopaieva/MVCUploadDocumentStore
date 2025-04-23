using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MVCUploadDocumentStore.Models;

public partial class NetDbContext : DbContext
{
    public NetDbContext()
    {
    }

    public NetDbContext(DbContextOptions<NetDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DocStore> DocStore { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=NetDB;Trusted_Connection=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocStore>(entity =>
        {
            entity.HasKey(e => e.DocId)
                .HasName("PK_DOC_STORE")
                .IsClustered(false);

            entity.HasIndex(e => e.DocName, "IDX_NAME").IsClustered();

            entity.Property(e => e.DocId).HasColumnName("DocID");
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.DocName)
                .HasMaxLength(512)
                .IsUnicode(false);
            entity.Property(e => e.InsertionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
