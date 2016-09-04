// -----------------------------------------------------------------------
// <copyright file="Configuration.cs" company="">
// Copyright 2014 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using System.Data.Entity.Migrations;
using MySql.Data.Entity;

namespace EntityData.Migrations
{
    public class MyDbContextConfiguration : DbMigrationsConfiguration<QDMSDbContext>
    {
        public MyDbContextConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;

            SetSqlGenerator("MySql.Data.MySqlClient", new MySqlMigrationSqlGenerator());
            //SetSqlGenerator("System.Data.SqlClient", new SqlServerMigrationSqlGenerator());
        }
         
        protected override void Seed(QDMSDbContext context)
        {
            base.Seed(context);
        }
    }
}
