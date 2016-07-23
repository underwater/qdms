using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace QDMSService.Config
{
    public enum DatabaseConnectionType
    {
        MySql,
        SqlServer,
        Sqlite
    }

    public class DatabaseConnectionConfig
    {
        [Required]
        public DatabaseConnectionType Type { get; set; }

        [Required]
        public string ConnectionString { get; set; }

        public DbContextOptions BuildDbContextOptions<T>()
            where T : DbContext
        {
            DbContextOptionsBuilder<T> optionsBuilder = new DbContextOptionsBuilder<T>();

            switch (Type)
            {
                case DatabaseConnectionType.MySql:
                    throw new NotImplementedException();
                case DatabaseConnectionType.SqlServer:
                    optionsBuilder.UseSqlServer(ConnectionString);
                    return optionsBuilder.Options;
                case DatabaseConnectionType.Sqlite:
                    optionsBuilder.UseSqlite(ConnectionString);
                    return optionsBuilder.Options;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type));
            }
        }
    }
}
