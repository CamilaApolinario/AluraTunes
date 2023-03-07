using AluraTunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace AluraTunes
{
    class Program
    {
        static void Main(string[] args)
        {
            var generos = new List<Genero>
            {
                new Genero{ Id = 1 , Nome = "Rock"},
                new Genero{ Id = 2 , Nome = "Reggae"},
                new Genero{ Id = 3 , Nome = "Rock Progressivo"},
                new Genero{ Id = 4 , Nome = "Punk"},
                new Genero{ Id = 5 , Nome = "Clássica"}
            };

            foreach (var genero in generos)
            {
                if (genero.Nome.Contains("Rock"))
                {
                    Console.WriteLine("{0}\t{1}", genero.Id, genero.Nome);
                }
            }
            Console.WriteLine();

            //usando Linq
            var query = from g in generos
                        where g.Nome.Contains("Rock")
                        select g;

            foreach (var genero in query)
            {
                Console.WriteLine("{0}\t{1}", genero.Id, genero.Nome);
            }

            Console.WriteLine();

            var musicas = new List<Musica>
            {
                new Musica{ Id = 1 , Nome = "Sweet Child O'Mine", GeneroId = 1},
                new Musica { Id = 2, Nome = "I Shoot The Sheriff", GeneroId = 2},
                new Musica { Id = 3, Nome = "Danúbio Azul", GeneroId = 5}
            };

            var queryMusica = from m in musicas
                              join g in generos on m.GeneroId equals g.Id
                              select new { m, g };

            foreach (var musica in queryMusica)
            {
                Console.WriteLine("{0}\t{1}\t{2}", musica.m.Id, musica.m.Nome, musica.g.Nome);
            }

            Console.WriteLine("Digite alguma tecla para continuar...");
            Console.ReadLine();


            //Acessar dados com XML, utilizando a biblioteca XElement
            XElement root = XElement.Load(@"C:\Development\Alura\AluraTunes\AluraTunes\AluraTunes\Data\AluraTunes.xml");

            var queryXML =
                from g in root.Element("Generos").Elements("Genero")
                select g;

            foreach (var genero in queryXML)
            {
                Console.WriteLine("{0}\t{1}", genero.Element("GeneroId").Value, genero.Element("Nome").Value);
            }

            Console.WriteLine();

            var query2 =
                from g in root.Element("Generos").Elements("Genero")
                join m in root.Element("Musicas").Elements("Musica")
                on g.Element("GeneroId").Value equals m.Element("GeneroId").Value
                select new
                {
                    Musica = m.Element("Nome").Value,
                    Genero = g.Element("Nome").Value
                };

            foreach (var musicaEgenero in query2)
            {
                Console.WriteLine("{0}\t{1}", musicaEgenero.Musica, musicaEgenero.Genero);

            };


            Console.WriteLine();
            Console.WriteLine("Acessando dados de um Banco de dados com linq, querys poderosas");

            //Acessando dados de um Banco de dados com linq
            using (var contexto = new AluraTunesEntities())
            {
                var query3 = from g in contexto.Generos
                             select g;
                foreach (var genero in query)
                {
                    Console.WriteLine("{0}\t{1}", genero.Id, genero.Nome);
                }

                var faixaEGenero = from g in contexto.Generos
                                   join f in contexto.Faixa
                                   on g.GeneroId equals f.GeneroId
                                   select new { f, g };

                faixaEGenero = faixaEGenero.Take(10);
                contexto.Database.Log = Console.WriteLine;

                Console.WriteLine();
                foreach (var item in faixaEGenero)
                {
                    Console.WriteLine("{0}\t{1}", item.f.Nome, item.g.Nome);
                }


            }
            Console.WriteLine("Consulta linq através de uma palavra");
            using (var contexto = new AluraTunesEntities())
            {
                var textoBusca = Console.ReadLine();
                Console.WriteLine();
                Console.WriteLine("sintaxe de consulta com join");
                //sintaxe de consulta com join
                var query4 = from a in contexto.Artista
                             join alb in contexto.Album
                             on a.ArtistaId equals alb.ArtistaId
                             where a.Nome.Contains(textoBusca)
                             select new
                             {

                                 NomeArtista = a.Nome,
                                 NomeAlbum = alb.Titulo
                             };

                foreach (var item in query4)
                {
                    Console.WriteLine("{0}\t{1}", item.NomeArtista, item.NomeAlbum);
                }

                Console.WriteLine();
                Console.WriteLine("sintaxe de consulta sem join");
                //sintaxe de consulta sem join
                var query6 = from alb in contexto.Album
                             where alb.Artista.Nome.Contains(textoBusca)
                             select new
                             {
                                 NomeArtista = alb.Artista.Nome,
                                 NomeAlbum = alb.Titulo
                             };

                foreach (var item in query4)
                {
                    Console.WriteLine("{0}\t{1}", item.NomeArtista, item.NomeAlbum);
                }

                Console.WriteLine();
                Console.WriteLine("sintaxe de método");
                //sintaxe de método
                var query5 = contexto.Artista.Where(a => a.Nome.Contains(textoBusca));
                foreach (var artista in query5)
                {
                    Console.WriteLine("{0}\t{1}", artista.ArtistaId, artista.Nome);
                }

                Console.WriteLine("Digite alguma tecla para continuar...");
                Console.ReadLine();
            }

            //Refinando consultas por parametros
            Console.WriteLine("Refinando consultas por parametros");
            using (var contexto = new AluraTunesEntities())
            {
                GetFaixas(contexto, "Led Zeppelin", "");
                Console.WriteLine();
                GetFaixas(contexto, "Led Zeppelin", "Graffiti");

                Console.WriteLine("Digite alguma tecla para continuar...");
                Console.ReadLine();
            }

            //Consultas ordenadas
            Console.WriteLine("Consultas ordenadas");
            using (var contexto = new AluraTunesEntities())
            {
                GetFaixasOrdenacao(contexto, "Led Zeppelin", "");
                Console.WriteLine();
                GetFaixasOrdenacao(contexto, "Led Zeppelin", "Graffiti");

                Console.WriteLine("Digite alguma tecla para continuar...");
                Console.ReadLine();
            }

            //Realizando contagem a partir de de uma consulta linq
            Console.WriteLine("Contagem de uma consulta");
            using (var contexto = new AluraTunesEntities())
            {
                //sintaxe de consulta
                var query3 = from f in contexto.Faixa
                             where f.Album.Artista.Nome == "Led Zeppelin"
                             select f;

                var quantidade = query.Count();
                Console.WriteLine("Led Zeppelin tem {0} faixas de música.", quantidade);

                //sintaxe de metodo
                var quantidade1 = contexto.Faixa.Count(f => f.Album.Artista.Nome == "Led Zeppelin");

                Console.WriteLine("Led Zeppelin tem {0} faixas de música.", quantidade1);
                Console.WriteLine("Digite alguma tecla para continuar...");
                Console.ReadLine();

            }

            //Soma de uma consulta linq
            Console.WriteLine("Soma de uma consulta linq");
            using (var contexto = new AluraTunesEntities())
            {
                var query4 = from inf in contexto.ItemNotaFiscal
                             where inf.Faixa.Album.Artista.Nome == "Led Zeppelin"
                             select new 
                             { 
                                 totalDoItem = inf.Quantidade * inf.PrecoUnitario 
                             };

                var totalDoArtista = query4.Sum(x => x.totalDoItem);
                Console.WriteLine("Total do artista:R$ {0}.", totalDoArtista);

                Console.WriteLine("Digite alguma tecla para continuar...");
                Console.ReadLine();

            }

            //Agrupando item de uma consulta
            Console.WriteLine("Agrupando item de uma consulta");
            using (var contexto = new AluraTunesEntities())
            {
                var query5 = from inf in contexto.ItemNotaFiscal
                             where inf.Faixa.Album.Artista.Nome == "Led Zeppelin"
                             group inf by inf.Faixa.Album into agrupado
                             // let é a declaração de uma váriavél dentro de uma consulta, assim não fica codigo repetido
                             let vendasPorAlbum = agrupado.Sum(a => a.Quantidade * a.PrecoUnitario)
                             orderby vendasPorAlbum descending
                             //orderby agrupado.Sum(a => a.Quantidade * a.PrecoUnitario) descending
                             select new
                             {
                                 TituloDoAlbum = agrupado.Key.Titulo,
                                 TotalPorAlbum = vendasPorAlbum,
                                 //TotalPorAlbum = agrupado.Sum(a=>a.Quantidade * a.PrecoUnitario)
                             };

                foreach (var agrupado in query5)
                {
                    Console.WriteLine("{0}\t{1}",
                        agrupado.TituloDoAlbum.PadRight(40),
                        agrupado.TotalPorAlbum);
                }

                Console.WriteLine("Digite alguma tecla para continuar...");
                Console.ReadLine();
            }

            //Agrupando item de uma consulta
            Console.WriteLine("Agrupando item de uma consulta");
            using (var contexto = new AluraTunesEntities())
            {
                contexto.Database.Log = Console.WriteLine;

                //dessa maneira criaria 3 consultas diferentes no banco de dados
                var maiorVenda = contexto.NotaFiscal.Max(nf => nf.Total);
                var menorVenda = contexto.NotaFiscal.Min(nf => nf.Total);
                var vendaMedia = contexto.NotaFiscal.Average(nf => nf.Total);

                Console.WriteLine("A maior venda é de {0}", maiorVenda);
                Console.WriteLine("A maior venda é de {0}", menorVenda);
                Console.WriteLine("A maior venda é de {0}", vendaMedia);
                Console.WriteLine();

                //É possivel fazer uma consulta só economizaremos no processamento do servidor SQL, no tráfico de rede e no tempo de processamento.

                var vendas = (from nf in contexto.NotaFiscal
                              group nf by 1 into agrupado
                              select new
                              {
                                  MaiorVenda = agrupado.Max(nf => nf.Total),
                                  MenorVenda = agrupado.Min(nf => nf.Total),
                                  VendaMedia = agrupado.Average(nf => nf.Total)
                              }).Single(); //Esse metodo transforma a query em um objeto, com três propriedades

                Console.WriteLine("A maior venda é de R$ {0}", vendas.MaiorVenda);
                Console.WriteLine("A menor venda é de R$ {0}", vendas.MenorVenda);
                Console.WriteLine("A venda média é de R$ {0}", vendas.VendaMedia);

                Console.WriteLine("Digite alguma tecla para continuar...");
                Console.ReadLine();
            }

            // Usando linq para calcular a mediana de uma consulta
            using (var contexto = new AluraTunesEntities())
            {
                var vendaMedia = contexto.NotaFiscal.Average(nf => nf.Total);
                Console.WriteLine("Venda Média: {0}", vendaMedia);

                var query5 = from nf in contexto.NotaFiscal
                             select nf.Total;

                //chamando o método comum
                decimal mediana = Mediana(query5);
                Console.WriteLine("Mediana: {0}", mediana);

                //chamando o método de extensão
                var vendaMediana = contexto.NotaFiscal.Mediana(nf => nf.Total);
                Console.WriteLine("Mediana (com método de extensão): {0}", vendaMediana);

                Console.WriteLine("Digite alguma tecla para continuar...");
                Console.ReadLine();
            }
        }

        private static void GetFaixas(AluraTunesEntities contexto, string buscaArtista, string buscaAlbum)
        {
            var query = from f in contexto.Faixa
                        where f.Album.Artista.Nome.Contains(buscaArtista)
                        select f;
            if (!string.IsNullOrEmpty(buscaAlbum))
            {
                query = query.Where(q => q.Album.Titulo.Contains(buscaAlbum));
            }
            foreach (var faixa in query)
            {
                Console.WriteLine("{0}\t{1}", faixa.Album.Titulo.PadRight(40), faixa.Nome);
            }
        }

        //Metodo comum
        private static decimal Mediana(IQueryable<decimal> query)
        {
            var contagem = query.Count();

            var queryOrdenada = query.OrderBy(total => total);

            var elementCentral_1 = queryOrdenada.Skip(contagem / 2).First();

            var elementCentral_2 = queryOrdenada.Skip((contagem - 1) / 2).First();

            var mediana = (elementCentral_1 + elementCentral_2) / 2;

            return mediana;
        }

        private static void GetFaixasOrdenacao(AluraTunesEntities contexto, string buscaArtista, string buscaAlbum)
        {
            var query = from f in contexto.Faixa
                        where f.Album.Artista.Nome.Contains(buscaArtista)
                        && (!string.IsNullOrEmpty(buscaAlbum)
                        ? f.Album.Titulo.Contains(buscaAlbum)
                        : true)
                        orderby f.Album.Titulo, f.Nome
                        select f;

            foreach (var faixa in query)
            {

                Console.WriteLine("{0}\t{1}", faixa.Album.Titulo.PadRight(40), faixa.Nome);
            }
        }
    }

    //metodo de extensão do Linq personalizado de acordo com a necessidade
    static class LinqExtension
    {
        public static decimal Mediana<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            var contagem = source.Count();

            var funcSelector = selector.Compile();

            var queryOrdenada = source.Select(funcSelector).OrderBy(total => total);

            var elementCentral_1 = queryOrdenada.Skip(contagem / 2).First();

            var elementCentral_2 = queryOrdenada.Skip((contagem - 1) / 2).First();

            var mediana = (elementCentral_1 + elementCentral_2) / 2;

            return mediana;
        }

    }

    class Genero
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
    class Musica
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int GeneroId { get; set; }
    }
}
