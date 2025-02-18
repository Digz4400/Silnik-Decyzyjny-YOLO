using System;
using System.IO;

class Player
{
    public Player(string Name, string Tier, int SeasonElo, int Wins, int Loses)
    {
        this.Name = Name;
        this.Tier = Tier;
        this.SeasonElo = SeasonElo;
        this.Wins = Wins;
        this.Loses = Loses;
    }
    public string Name, Tier;
    public int SeasonElo, Wins, Loses;
    public Player[] MatchHistoryInteration, MatchHistorySeason;
    public void Print()
    {
        Console.WriteLine(this.Name);
        Console.WriteLine(this.Tier);
        Console.WriteLine(this.SeasonElo);
        Console.WriteLine(this.Wins);
        Console.WriteLine(this.Loses);
        Console.Write("\n");
    }
}



//--------------------------------------------------------------------------------------------------------

namespace SilnikDecyzyjnyYOLO
{
    internal class Program
    {
        //Global Variables
        const int NumberOfInterations = 3;
        const int NumberofTiers = 2;
        int AktualnyTydzien;
        static List<Player> Players = new List<Player>();
        static void ZaladujDane()
        {
            string folderPath = @"PlayerBase";
            int FilesIterator = 0;
            foreach (string file in Directory.GetFiles(folderPath))
            {
                using (StreamReader sr = File.OpenText(file))
                {
                    string s = sr.ReadLine(); 
                    if (s != null)
                    {
                        Players.Add(new Player(s.Split(",")[0], s.Split(",")[1], System.Convert.ToInt32(s.Split(",")[2]), System.Convert.ToInt32(s.Split(",")[3]), System.Convert.ToInt32(s.Split(",")[4])));
                    }
                }
                FilesIterator++;
            }
            /*foreach (var p in Players)
            {
                p.Print();
            }*/
            Console.WriteLine("Ładowanie danych zakończone");
        }
        static void Main(string[] args)
        {


            Console.WriteLine("Witaj");
            Console.WriteLine("Załaduj Dane");
            ZaladujDane();
            Console.ReadLine();
        }
    }
}

