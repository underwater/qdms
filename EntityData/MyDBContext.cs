// -----------------------------------------------------------------------
// <copyright file="MyDBContext.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QDMS;

namespace EntityData
{
    public partial class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions options)
            : base(options)
        {
        }
 
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<Datasource> Datasources { get; set; }
        public DbSet<SessionTemplate> SessionTemplates { get; set; }
        public DbSet<ExchangeSession> ExchangeSessions { get; set; }
        public DbSet<InstrumentSession> InstrumentSessions { get; set; }
        public DbSet<TemplateSession> TemplateSessions { get; set; }
        public DbSet<UnderlyingSymbol> UnderlyingSymbols { get; set; }
        public DbSet<ContinuousFuture> ContinuousFutures { get; set; }
        public DbSet<DataUpdateJobDetails> DataUpdateJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelCreatingInterface(modelBuilder.Entity<Instrument>());
            ModelCreatingExchange(modelBuilder.Entity<Exchange>());
            ModelCreatingContinuousFuture(modelBuilder.Entity<ContinuousFuture>());
            ModelCreatingDataUpdateJobDetails(modelBuilder.Entity<DataUpdateJobDetails>());

            modelBuilder.Entity<SessionTemplate>()
                .HasMany(x => x.Sessions).WithOne()
                .HasForeignKey(x => x.TemplateID)
                .IsRequired()
                .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);

            modelBuilder.Entity<ExchangeSession>().ToTable("exchangesessions");
            modelBuilder.Entity<InstrumentSession>().ToTable("instrumentsessions");
            modelBuilder.Entity<TemplateSession>().ToTable("templatesessions");

            modelBuilder.Entity<TagInstrument>().HasOne(x => x.Instrument)
                .WithMany()
                .HasForeignKey(x => x.InstrumentID);
            modelBuilder.Entity<TagInstrument>().HasOne(x => x.Tag)
                .WithMany()
                .HasForeignKey(x => x.TagID);
            modelBuilder.Entity<TagInstrument>().ToTable("tag_map");
        }

        public void ModelCreatingInterface(EntityTypeBuilder<Instrument> builder)
        {
            builder.HasOne(p => p.Exchange)
                .WithMany()
                .HasForeignKey(x => x.ExchangeID)
                .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            
            builder.HasOne(x => x.PrimaryExchange)
                .WithMany()
                .HasForeignKey(x => x.PrimaryExchangeID)
                .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            builder.HasOne(x => x.Datasource)
                .WithMany()
                .HasForeignKey(x => x.DatasourceID)
                .IsRequired()
                .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);

            builder.HasMany(x => x.Sessions)
                .WithOne()
                .HasForeignKey(x => x.InstrumentID)
                .IsRequired()
                .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            
            builder.HasOne(x => x.ContinuousFuture)
                .WithMany()
                .HasForeignKey(x => x.ContinuousFutureID)
                .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
        }

        public void ModelCreatingExchange(EntityTypeBuilder<Exchange> builder)
        {
            builder.HasMany(x => x.Sessions)
                .WithOne()
                .HasForeignKey(x => x.ExchangeID)
                .IsRequired()
                .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
        }

        public void ModelCreatingContinuousFuture(EntityTypeBuilder<ContinuousFuture> builder)
        {
            builder.HasOne(x => x.UnderlyingSymbol)
                .WithMany()
                .HasForeignKey(x => x.UnderlyingSymbolID)
                .IsRequired()
                .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
        }

        public void ModelCreatingDataUpdateJobDetails(EntityTypeBuilder<DataUpdateJobDetails> builder)
        {
            builder.HasOne(t => t.Instrument)
                .WithMany()
                .HasForeignKey(x => x.InstrumentID)
                .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);

            builder.HasOne(t => t.Tag)
                .WithMany()
                .HasForeignKey(x => x.TagID)
                .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
        }
    }
}
