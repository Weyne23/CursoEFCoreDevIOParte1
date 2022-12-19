using System;
using curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace curso.Data
{
    public class ApplicationContextCidade : DbContext
    {
        public DbSet<Cidade> Cidades { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConection = "Data source=(localdb)\\mssqllocaldb; Initial Catalog=C002;Integrated Security=true; pooling=true";
            optionsBuilder.UseSqlServer(strConection)
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}