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
            // new curso.Data.ApplicationContext().Departamentos.Any();
            // _count = 0;
            // GarenciarEstadoDaConexao(false);
            // _count = 0;
            // GarenciarEstadoDaConexao(true);
            //SqlInjection();
            //MigracoesPendentes();
            //AplicarMigrationEmTempodeExecucao();
            //TodasMigracoes();
            //MigracoesJaAplicadas();
            //ScriptGeraldoBancodeDados();
            //CarregamentoAdiantado();
            //CarregamentoExplicito();
            CarregamentoLento();
        }
    static void CarregamentoLento()
        {
            using var db = new curso.Data.ApplicationContext();
            SetupTipoCarregamento(db);

            //db.ChangeTracker.LazyLoadingEnabled = false;
            var departamentos = db.Departamentos.ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("--------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if(departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"Funcionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }
        static void CarregamentoExplicito()
        {
            using var db = new curso.Data.ApplicationContext();
            SetupTipoCarregamento(db);

            var departamentos = db.Departamentos.ToList();

            foreach (var departamento in departamentos)
            {
                if(departamento.Id == 2)
                    //db.Entry(departamento).Collection(x => x.Funcionarios).Load();
                    db.Entry(departamento).Collection(x => x.Funcionarios).Query().Where(x => x.Id > 2).ToList();
                Console.WriteLine("--------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if(departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"Funcionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }
        static void CarregamentoAdiantado()
        {
            using var db = new curso.Data.ApplicationContext();
            SetupTipoCarregamento(db);

            var departamentos = db.Departamentos
                                  .Include(p => p.Funcionarios);

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("--------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if(departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"Funcionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }
        private static void SetupTipoCarregamento(curso.Data.ApplicationContext db)
        {
            if(!db.Departamentos.Any())
            {
                db.Departamentos.AddRange(
                    new curso.Domain.Departamento
                    {
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<curso.Domain.Funcionario>
                        {
                            new curso.Domain.Funcionario
                            {
                                Nome = "Rafael Almeida",
                                CPF = "99999999911",
                                RG = "2100062"
                            }
                        }
                    },
                    new curso.Domain.Departamento
                    {
                        Descricao = "Departamento 02",
                        Funcionarios = new System.Collections.Generic.List<curso.Domain.Funcionario>
                        {
                            new curso.Domain.Funcionario
                            {
                                Nome = "Bruno Brito",
                                CPF = "99988999911",
                                RG = "3100062"
                            },
                            new curso.Domain.Funcionario
                            {
                                Nome = "Eduardo Pires",
                                CPF = "99779999911",
                                RG = "4100062"
                            }
                        }
                    }
                );
            }

            db.SaveChanges();
            db.ChangeTracker.Clear();
        }
        static void ScriptGeraldoBancodeDados()
        {
            using var db = new curso.Data.ApplicationContext();
            var script = db.Database.GenerateCreateScript();

            Console.WriteLine(script);
        }
        static void MigracoesJaAplicadas()
        {
            using var db = new curso.Data.ApplicationContext();

            var migrations = db.Database.GetAppliedMigrations();

            Console.WriteLine($"Total: {migrations.Count()}");

            foreach (var migration in migrations)
            {
                Console.WriteLine($"Migração: {migration}");
            }
        }
        static void TodasMigracoes()
        {
            using var db = new curso.Data.ApplicationContext();

            var migrations = db.Database.GetMigrations();

            Console.WriteLine($"Total: {migrations.Count()}");

            foreach (var migration in migrations)
            {
                Console.WriteLine($"Migração: {migration}");
            }
        }
        static void AplicarMigrationEmTempodeExecucao()
        {
            using var db = new curso.Data.ApplicationContext();

            db.Database.Migrate();
        }
        static void MigracoesPendentes()
        {
            using var db = new curso.Data.ApplicationContext();

            var migrations = db.Database.GetPendingMigrations();

            Console.WriteLine($"Total: {migrations.Count()}");

            foreach (var migration in migrations)
            {
                Console.WriteLine($"Migração: {migration}");
            }
        }
        static void SqlInjection()
        {
            using var db = new curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departamentos.AddRange(
                new curso.Domain.Departamento
                {
                    Descricao = "Departamento 01"
                },
                new curso.Domain.Departamento
                {
                    Descricao = "Departamento 02"
                }
            );
            db.SaveChanges();

            var descricao = "Teste ' or 1='1";
            db.Database.ExecuteSqlRaw($"update Departamentos set Descricao = 'Departamento Alterado' where Descricao = '{descricao}'");
            foreach(var departamento in db.Departamentos.AsNoTracking())
            {
                Console.WriteLine($"Id: {departamento.Id}, Descrição: {departamento.Descricao}");
            }
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
