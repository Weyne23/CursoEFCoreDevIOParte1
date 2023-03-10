using System;
using curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace curso.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConection = "Data source=(localdb)\\mssqllocaldb;Initial Catalog=C002;Integrated Security=true;pooling=true"; //MultipleActiveResultSets=true
            optionsBuilder.UseSqlServer(strConection)
            .EnableSensitiveDataLogging()
            //.UseLazyLoadingProxies()
            .LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}