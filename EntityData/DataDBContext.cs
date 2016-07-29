// -----------------------------------------------------------------------
// <copyright file="DataDBContext.cs" company="">
// Copyright 2014 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using QDMS;

namespace EntityData
{
    public class DataDBContext : DbContext
    {
        public DataDBContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<OHLCBar> Data { get; set; }
        public DbSet<StoredDataInfo> StoredDataInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OHLCBar>().ToTable("data");
            modelBuilder.Entity<StoredDataInfo>().ToTable("instrumentinfo");
            
            modelBuilder.Entity<OHLCBar>().HasKey(x => new { x.DT, x.InstrumentID, x.Frequency });
            modelBuilder.Entity<StoredDataInfo>().HasKey(x => new { x.InstrumentID, x.Frequency });
        }
    }
}
