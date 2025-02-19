using System;
using System.IO;

class Player
{
    public Player(string Name, string Tier, int SeasonElo, int GlobalElo, int Wins, int Loses)
    {
        this.Name = Name;
        this.Tier = Tier;
        this.SeasonElo = SeasonElo;
        this.GlobalElo = GlobalElo;
        this.Wins = Wins;
        this.Loses = Loses;
    }
    public string Name, Tier;
    public int SeasonElo, GlobalElo, Wins, Loses;
    public List<TekkenMatch> MatchHistorySeason, MatchHistoryGlobal;
    public void Print()
    {
        Console.WriteLine(this.Name);
        Console.WriteLine(this.Tier);
        Console.WriteLine(this.SeasonElo);
        Console.WriteLine(this.GlobalElo);
        Console.WriteLine(this.Wins);
        Console.WriteLine(this.Loses);
        Console.Write("\n");
    }
    public bool HasPlayerBeenPlayedSeason(Player opponent)
    {
        foreach (var check in this.MatchHistorySeason)
        {
            if((check.Player1.Name == opponent.Name) ||( check.Player2.Name == opponent.Name))
            {
                return true;
            }
        }
        return false;
    }
    public bool HasPlayerBeenPlayedGlobal(Player opponent)
    {
        foreach (var check in this.MatchHistoryGlobal)
        {
            if ((check.Player1.Name == opponent.Name) || (check.Player2.Name == opponent.Name))
            {
                return true;
            }
        }
        return false;
    }
}
class TekkenMatch
{
    TekkenMatch() { }
    public Player Player1;
    public Player Player2;
    public string Winner;
    public int ScorePalyer1, ScorePalyer2;
}
class TierPlusPlayers
{
    public TierPlusPlayers(string Tier) 
    {
        this.Tier = Tier;
        this.PlayerList00 = new List<Player>();
    }
    public string Tier;
    public List<Player> PlayerList00, PlayerList10, PlayerList01, PlayerList11, PlayerList20, PlayerList02;
    public void PrintTierPlusPlayersWeek1() 
    {
        foreach (var player in this.PlayerList00)
        {
            Console.WriteLine(player.Name);
        }
    }
}

//--------------------------------------------------------------------------------------------------------

namespace SilnikDecyzyjnyYOLO
{
    internal class Program
    {
        //Global Variables
        static int NumberOfInterations = 3;
        static int NumberofTiers = 2;
        static int Week;
        static List<string> Tiery = new List<string>();
        static List<Player> Players = new List<Player>();
        static void ZaladujDane()
        {
            string folderPath = @"Data\PlayerBase";
            int FilesIterator = 0;
            foreach (string file in Directory.GetFiles(folderPath))
            {
                using (StreamReader sr = File.OpenText(file))
                {
                    string s = sr.ReadLine(); 
                    if (s != null)
                    {
                        Players.Add(new Player(s.Split(",")[0], s.Split(",")[1], System.Convert.ToInt32(s.Split(",")[2]), System.Convert.ToInt32(s.Split(",")[3]), System.Convert.ToInt32(s.Split(",")[4]), System.Convert.ToInt32(s.Split(",")[5])));
                    }
                }
                FilesIterator++;
            }
            /*foreach (var p in Players)
            {
                p.Print();
            }*/
            
            using (StreamReader sr = File.OpenText(@"Data\Parameters.txt"))
            {
                string s = sr.ReadLine();
                if (s != null)
                {
                    Week = System.Convert.ToInt32(s);
                }
                s = sr.ReadLine();
                //Console.WriteLine(Week);
                if (s != null)
                {
                    foreach (var tier in s.Split(","))
                    {
                        Tiery.Add(tier);
                    }
                }
                //Console.WriteLine(Tiery[0]);
            }
            Console.WriteLine("Ładowanie danych zakończone");
        }
        static void Losowanie()
        {
            Console.WriteLine("Losowanie rozpoczęte");
            switch(Week)
            {
                case 1:
                    {
                        List<TierPlusPlayers> PlayersInTier = new List<TierPlusPlayers>();
                        foreach (var Tier in Tiery)
                        {
                            PlayersInTier.Add(new TierPlusPlayers(Tier));
                            foreach (var Player in Players)
                            {
                                if(Tier == Player.Tier)
                                {
                                    PlayersInTier[PlayersInTier.Count - 1].PlayerList00.Add(Player);
                                }
                            }
                        }
                        /*foreach(var print in PlayersInTier)
                        {   
                            Console.WriteLine(print.Tier);
                            print.PrintTierPlusPlayersWeek1();
                        }*/
                        List<Player> Pool = new List<Player>();
                        
                        /*foreach (var pTier in PlayersInTier)
                        {
                            int licznik = 0;
                            foreach (var )
                        }*/
                        
                        break;
                    }
                case 2:
                    {

                        break;
                    }
                case 3:
                    {

                        break;
                    }
                case 4:
                    {

                        break;
                    }
                default: { break; }
            }
            Console.WriteLine("Losowanie zakończone");  
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Witaj");
            Console.WriteLine("Załaduj Dane");
            ZaladujDane();
            Losowanie();
            Console.ReadLine();
        }
    }
}

