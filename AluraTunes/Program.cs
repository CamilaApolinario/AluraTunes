using AluraTunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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


                Console.ReadLine();
            }
        }
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
