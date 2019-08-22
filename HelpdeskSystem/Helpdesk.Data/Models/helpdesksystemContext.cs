using System;
using Helpdesk.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Helpdesk.Data.Models
{
    public partial class helpdesksystemContext : DbContext
    {
        public helpdesksystemContext()
        {
        }

        public helpdesksystemContext(DbContextOptions<helpdesksystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Checkinhistory> Checkinhistory { get; set; }
        public virtual DbSet<Checkinqueueitem> Checkinqueueitem { get; set; }
        public virtual DbSet<Helpdesksettings> Helpdesksettings { get; set; }
        public virtual DbSet<Helpdeskunit> Helpdeskunit { get; set; }
        public virtual DbSet<Nicknames> Nicknames { get; set; }
        public virtual DbSet<Queueitem> Queueitem { get; set; }
        public virtual DbSet<Timespans> Timespans { get; set; }
        public virtual DbSet<Topic> Topic { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                AppSettings appSettings = new AppSettings();

                optionsBuilder.UseMySQL(appSettings.DefaultConnection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Checkinhistory>(entity =>
            {
                entity.HasKey(e => e.CheckInId);

                entity.ToTable("checkinhistory", "helpdesksystem");

                entity.HasIndex(e => e.UnitId)
                    .HasName("UnitID");

                entity.Property(e => e.CheckInId)
                    .HasColumnName("CheckInID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ForcedCheckout).HasColumnType("tinyint(1)");

                entity.Property(e => e.UnitId)
                    .HasColumnName("UnitID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Checkinhistory)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("checkinhistory_ibfk_1");
            });

            modelBuilder.Entity<Checkinqueueitem>(entity =>
            {
                entity.ToTable("checkinqueueitem", "helpdesksystem");

                entity.HasIndex(e => e.CheckInId)
                    .HasName("CheckInID");

                entity.HasIndex(e => e.QueueItemId)
                    .HasName("QueueItemID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CheckInId)
                    .HasColumnName("CheckInID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.QueueItemId)
                    .HasColumnName("QueueItemID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.CheckIn)
                    .WithMany(p => p.Checkinqueueitem)
                    .HasForeignKey(d => d.CheckInId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("checkinqueueitem_ibfk_2");

                entity.HasOne(d => d.QueueItem)
                    .WithMany(p => p.Checkinqueueitem)
                    .HasForeignKey(d => d.QueueItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("checkinqueueitem_ibfk_1");
            });

            modelBuilder.Entity<Helpdesksettings>(entity =>
            {
                entity.HasKey(e => e.HelpdeskId);

                entity.ToTable("helpdesksettings", "helpdesksystem");

                entity.Property(e => e.HelpdeskId)
                    .HasColumnName("HelpdeskID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IsDeleted).HasColumnType("tinyint(1)");

                entity.Property(e => e.HasCheckIn).HasColumnType("tinyint(1)");

                entity.Property(e => e.HasQueue).HasColumnType("tinyint(1)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Helpdeskunit>(entity =>
            {
                entity.ToTable("helpdeskunit", "helpdesksystem");

                entity.HasIndex(e => e.HelpdeskId)
                    .HasName("HelpdeskID");

                entity.HasIndex(e => e.UnitId)
                    .HasName("UnitID");

                entity.Property(e => e.HelpdeskUnitId)
                    .HasColumnName("HelpdeskUnitID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HelpdeskId)
                    .HasColumnName("HelpdeskID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UnitId)
                    .HasColumnName("UnitID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Helpdesk)
                    .WithMany(p => p.Helpdeskunit)
                    .HasForeignKey(d => d.HelpdeskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("helpdeskunit_ibfk_1");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Helpdeskunit)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("helpdeskunit_ibfk_2");
            });

            modelBuilder.Entity<Nicknames>(entity =>
            {
                entity.HasKey(e => e.StudentId);

                entity.ToTable("nicknames", "helpdesksystem");

                entity.HasIndex(e => e.NickName)
                    .HasName("NickName")
                    .IsUnique();

                entity.Property(e => e.StudentId)
                    .HasColumnName("StudentID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.NickName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Sid)
                    .IsRequired()
                    .HasColumnName("SID")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Queueitem>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.ToTable("queueitem", "helpdesksystem");

                entity.HasIndex(e => e.StudentId)
                    .HasName("StudentID");

                entity.HasIndex(e => e.TopicId)
                    .HasName("TopicID");

                entity.Property(e => e.ItemId)
                    .HasColumnName("ItemID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NickName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.StudentId)
                    .HasColumnName("StudentID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TimeHelped).HasColumnType("tinyint(1)");

                entity.Property(e => e.TimeRemoved).HasColumnType("tinyint(1)");

                entity.Property(e => e.TopicId)
                    .HasColumnName("TopicID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Queueitem)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("queueitem_ibfk_1");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.Queueitem)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("queueitem_ibfk_2");
            });

            modelBuilder.Entity<Timespans>(entity =>
            {
                entity.HasKey(e => e.SpanId);

                entity.ToTable("timespans", "helpdesksystem");

                entity.Property(e => e.SpanId)
                    .HasColumnName("SpanID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HelpdeskId)
                    .HasColumnName("HelpdeskID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.ToTable("topic", "helpdesksystem");

                entity.HasIndex(e => e.UnitId)
                    .HasName("UnitID");

                entity.Property(e => e.TopicId)
                    .HasColumnName("TopicID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IsDeleted).HasColumnType("tinyint(1)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UnitId)
                    .HasColumnName("UnitID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Topic)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("topic_ibfk_1");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("unit", "helpdesksystem");

                entity.Property(e => e.UnitId)
                    .HasColumnName("UnitID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IsDeleted).HasColumnType("tinyint(1)");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user", "helpdesksystem");

                entity.HasIndex(e => e.Username)
                    .HasName("Username")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });
        }
    }
}
