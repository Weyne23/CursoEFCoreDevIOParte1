using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //EnsureCreateAndDeleted();
            //GapDoEnsureCreated();
            //HealthCkeckNamcoDeDados();

            //warmup
            new curso.Data.ApplicationContext().Departamentos.Any();
            _count = 0;
            GarenciarEstadoDaConexao(false);
            _count = 0;
            GarenciarEstadoDaConexao(true);
        }

        static void ExecuteSQL()
        {
            using var db = new curso.Data.ApplicationContext();

            using(var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            var descricao = "TESTE";
            db.Database.ExecuteSqlRaw("UPDATE Departamento SET Descricao = {0} WHERE Id = 1", descricao);

            db.Database.ExecuteSqlInterpolated($"UPDATE Departamento SET Descricao = {descricao} WHERE Id = 1");
        }
        static int _count = 0;
        static void GarenciarEstadoDaConexao(bool gerenciarEastadoconexao)
        {
            using var db = new curso.Data.ApplicationContext();
            var time = System.Diagnostics.Stopwatch.StartNew();

            var conexao = db.Database.GetDbConnection();
            conexao.StateChange += (_, __) => ++ _count;

            if(gerenciarEastadoconexao)
            {
                conexao.Open();
            }

            for(int i = 0; i < 200; i++)
            {
                db.Departamentos.AsNoTracking().Any();
            }

            time.Stop();
            var mensagem = $"Tempo: {time.Elapsed.ToString()}, {gerenciarEastadoconexao}, count: {_count}";

            Console.WriteLine(mensagem);
        }

        static void HealthCkeckNamcoDeDados()
        {
            using var db = new curso.Data.ApplicationContext();

            var canConnect = db.Database.CanConnect();

            if(canConnect)
            {
                Console.WriteLine("Conectou");
            }
            else
            {
                Console.WriteLine("Não Conectou");
            }
            // try
            // {
            //     //1
            //     var connection = db.Database.GetDbConnection();
            //     connection.Open();

            //     //2
            //     db.Departamentos.Any();
            //     Console.WriteLine("Conectou");
            // }
            // catch
            // {
            //     Console.WriteLine("Não Conectou");
            // }
        }

        static void EnsureCreateAndDeleted()
        {
            using var db = new curso.Data.ApplicationContext();

            db.Database.EnsureCreated();
            //db.Database.EnsureDeleted();
        }

        static void GapDoEnsureCreated()
        {
            using var db1 = new curso.Data.ApplicationContext();
            using var db2 = new curso.Data.ApplicationContextCidade();

            db1.Database.EnsureCreated();
            db2.Database.EnsureCreated();

            var databaseCreator = db2.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
    }
}
