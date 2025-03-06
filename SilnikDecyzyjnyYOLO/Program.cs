using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Xml.Linq;

class Player : ICloneable
{
    public Player(string Name, string Tier, int SeasonElo, int GlobalElo, int Wins, int Loses, int Flag)
    {
        this.Name = Name;
        this.Tier = Tier;
        this.SeasonElo = SeasonElo;
        this.GlobalElo = GlobalElo;
        this.Wins = Wins;
        this.Loses = Loses;
        this.PlayOffFlag = Flag;
        this.MatchHistoryGlobal = new List<TekkenMatch>();
        this.MatchHistorySeason = new List<TekkenMatch>();
    }
    public string Name, Tier, Result;
    public int SeasonElo, GlobalElo, Wins, Loses, PlayOffFlag;
    public List<TekkenMatch> MatchHistorySeason, MatchHistoryGlobal;
    public void Print()
    {
        Console.WriteLine(this.Name);
        Console.WriteLine(this.Tier);
        Console.Write("Season Elo: " + this.SeasonElo + " ");
        Console.Write("Global Elo:" + this.GlobalElo);
        Console.Write("\n");
        Console.WriteLine(this.Wins + " - " + this.Loses);
        Console.Write("\n");
        Console.WriteLine($"Playoff Flag: {this.PlayOffFlag}");
        Console.WriteLine($"Result: {this.Result}");
        Console.WriteLine("Opponets this iteration");
        foreach(var opponent in this.MatchHistorySeason)
        {
            Console.Write($"{opponent.ReturnOpponent(this).Name }");
        }
        Console.WriteLine();
        Console.WriteLine("Opponets in League");
        foreach (var opponent in this.MatchHistoryGlobal)
        {
            Console.Write($"{opponent.ReturnOpponent(this).Name}");
        }
        Console.WriteLine();
        Console.WriteLine();
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
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

class TekkenMatch
{
    public TekkenMatch(Player p1, Player p2) 
    {
        this.Player1 = p1;
        this.Player2 = p2;
        this.Player1.MatchHistoryGlobal.Add(this);
        this.Player1.MatchHistorySeason.Add(this);
        this.Player2.MatchHistoryGlobal.Add(this);
        this.Player2.MatchHistorySeason.Add(this);
    }
    public Player Player1;
    public Player Player2;
    public string Winner;
    public int ScorePlayer1, ScorePlayer2, EloPlayer1GainSeason, EloPlayer2GainSeason, EloPlayer1GainGlobal, EloPlayer2GainGlobal;
    public bool HasPlayerInMatch(Player player)
    {
        if (this.Player1 == player || this.Player2 == player) return true; 
        return false;
    }
    public void Print()
    {
        //Console.WriteLine("Match:");
        Console.Write("Player 1:" + this.Player1.Name + " Player 2:" + this.Player2.Name);
        if(this.Winner!=null)
        {
            Console.Write($" Winner: {this.Winner} ");
            if(this.ScorePlayer1 == 3)
            {
                Console.Write($"Score: {this.ScorePlayer1} - {this.ScorePlayer2}");
            }
            else
            {
                Console.Write($"Score: {this.ScorePlayer2} - {this.ScorePlayer1}");
            }
            Console.Write("\n");
        }
        Console.WriteLine();
    }
    public void Score(string Winner)
    {
        if(this.Winner !=null) return;
        this.Winner = Winner;
        if(this.Player1.Name == this.Winner)
        {
            this.Player1.Wins++;
            this.Player2.Loses++;
            if (this.Player1.Wins == 2 && this.Player1.Loses == 0) this.Player1.PlayOffFlag = 1;
            if (this.Player2.Wins == 0 && this.Player2.Loses == 2) this.Player2.PlayOffFlag = 1;
        }
        else
        {
            this.Player2.Wins++;
            this.Player1.Loses++;
            if (this.Player2.Wins == 2 && this.Player2.Loses == 0) this.Player2.PlayOffFlag = 1;
            if (this.Player1.Wins == 0 && this.Player1.Loses == 2) this.Player1.PlayOffFlag = 1;
        };
        if (this.Player1.Wins == 3) this.Player1.Result = "Up";
        if (this.Player1.Loses == 3) this.Player1.Result = "Down";
        if (this.Player2.Wins == 3) this.Player2.Result = "Up";
        if (this.Player2.Loses == 3) this.Player2.Result = "Down";

        CalculateEloSeason();
        CalculateEloGlobal();

    }
    public Player ReturnOpponent(Player p)
    {
        if(this.Player1 == p)
        {
            return this.Player2;
        }
        else
        {
            return this.Player1;
        }
    }
    public void CalculateEloSeason()
    {
        int Player1OldElo = Player1.SeasonElo;
        int Player2OldElo = Player2.SeasonElo;
        double p = 1.0 / (1 + Math.Pow(10, (Player2OldElo - Player1OldElo) / 200));
        double w = (double)this.ScorePlayer1 / (this.ScorePlayer1 + this.ScorePlayer2);
        Player1.SeasonElo = (int)(Player1OldElo + 40 * (w - p) * (1 + 0.5 * Math.Abs(w - p)));
        p = 1.0 / (1 + Math.Pow(10, (Player1OldElo - Player2OldElo) / 200));
        w = (double)this.ScorePlayer2 / (this.ScorePlayer1 + this.ScorePlayer2);
        Player2.SeasonElo = (int)(Player2OldElo + 40 * (w - p)*(1+0.5*Math.Abs(w-p)));

        EloPlayer1GainSeason = Player1.SeasonElo - Player1OldElo;
        EloPlayer2GainSeason = Player2.SeasonElo - Player2OldElo;
    }
    public void CalculateEloGlobal()
    {
        int Player1OldElo = Player1.GlobalElo;
        int Player2OldElo = Player2.GlobalElo;
        double p = 1.0 / (1 + Math.Pow(10, (Player2OldElo - Player1OldElo) / 200));
        double w = (double)this.ScorePlayer1 / (this.ScorePlayer1 + this.ScorePlayer2);
        Player1.GlobalElo = (int)(Player1OldElo + 40 * (w - p) * (1 + 0.5 * Math.Abs(w - p)));
        p = 1.0 / (1 + Math.Pow(10, (Player1OldElo - Player2OldElo) / 200));
        w = (double)this.ScorePlayer2 / (this.ScorePlayer1 + this.ScorePlayer2);
        Player2.GlobalElo = (int)(Player2OldElo + 40 * (w - p) * (1 + 0.5 * Math.Abs(w - p)));

        EloPlayer1GainGlobal = Player1.GlobalElo - Player1OldElo;
        EloPlayer2GainGlobal = Player2.GlobalElo - Player2OldElo;
    }
}
class TierPlusPlayers
{
    public TierPlusPlayers(string Tier, int Week, int Lp) 
    {
        this.lp = Lp;
        this.Tier = Tier;
        this.MatchesInTier = new List<TekkenMatch>();
        this.PlayerList00 = new List<Player>();
        this.PlayerList10 = new List<Player>();
        this.PlayerList01 = new List<Player>();
        this.PlayerList = new List<Player>();
        this.PlayerList20 = new List<Player>();
        this.PlayerList11 = new List<Player>(); 
        this.PlayerList02 = new List<Player>(); 
        this.PlayerList21 = new List<Player>();
        this.PlayerList12 = new List<Player>();
        this.PlayerListWeek4Down = new List<Player>();
        this.PlayerListWeek4Up = new List<Player>();
    }
    public int lp;
    public string Tier;
    public List<Player> PlayerList00, PlayerList10, PlayerList01, PlayerList11, PlayerList20, PlayerList02, PlayerList12, PlayerList21, PlayerListWeek4Down, PlayerListWeek4Up;
    public List<Player> PlayerList;
    public List<TekkenMatch> MatchesInTier;
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
        static List<TierPlusPlayers> PlayersInTier = new List<TierPlusPlayers>();
        static void LoadData()
        {
            string Path = @"Data\PlayerBase\Players.txt";
            using (StreamReader sr = File.OpenText(Path))
            {
                string s;
                while((s=sr.ReadLine())!=null)
                {  
                    Players.Add(new Player(s.Split(",")[0], s.Split(",")[1], System.Convert.ToInt32(s.Split(",")[2]), System.Convert.ToInt32(s.Split(",")[3]), System.Convert.ToInt32(s.Split(",")[4]), System.Convert.ToInt32(s.Split(",")[5]), System.Convert.ToInt32(s.Split(",")[6])));
                }
            } 
            using (StreamReader sr = File.OpenText(@"Data\Parameters.txt"))
            {
                string s = sr.ReadLine();
                if (s != null)
                {
                    Week = System.Convert.ToInt32(s);
                }
                s = sr.ReadLine();
                if (s != null)
                {
                    foreach (var tier in s.Split(","))
                    {
                        Tiery.Add(tier);
                    }
                }
            }
            int LP = 1;
            foreach (var Tier in Tiery)
            {
                PlayersInTier.Add(new TierPlusPlayers(Tier, Week, LP));
                LP++;
                foreach (var Player in Players)
                {
                    if (Tier == Player.Tier)
                    {
                        PlayersInTier[PlayersInTier.Count - 1].PlayerList.Add(Player);
                    }
                }
            }
            Console.WriteLine("Loading complite");
        }
        static void SaveData()
        {

        }
        static List<TekkenMatch> DrawMatchesWeek4(List<Player> DrawPoolUP, List<Player> DrawPoolDown)
        {
            List<TekkenMatch> result = new List<TekkenMatch>();
            for (int j = 0; j < DrawPoolUP.Count; j++)
            {
                List<Player> drawPool = new List<Player>();
                for (int i = 0; i < DrawPoolDown.Count; i++)
                {
                    if (DrawPoolUP[j].HasPlayerBeenPlayedSeason(DrawPoolDown[i]))
                    {

                    }
                    else if (DrawPoolUP[j].HasPlayerBeenPlayedGlobal(DrawPoolDown[i]))
                    {
                        drawPool.Add(DrawPoolDown[i]);
                    }
                    else
                    {
                        drawPool.Add(DrawPoolDown[i]);
                        drawPool.Add(DrawPoolDown[i]);
                    }
                }
                Random rn = new Random();
                int rand = rn.Next(0, drawPool.Count);
                result.Add(new TekkenMatch(DrawPoolUP[j], drawPool[rand]));
                DrawPoolDown.Remove(drawPool[rand]);
            }
            return result;
        }
        static List<TekkenMatch> DrawMatches(List<Player> DrawPool)
        {
            List<TekkenMatch> Result = new List<TekkenMatch>();

            int CountOfPlayersInTier = DrawPool.Count;

            for(int i = 0;i<CountOfPlayersInTier/2;i++)
            {
                List<Player> Draw = new List<Player>();
                for (int k = 1;k<DrawPool.Count;k++)
                {
                    if (DrawPool[0].HasPlayerBeenPlayedSeason(DrawPool[k]))
                    {

                    }
                    else if(DrawPool[0].HasPlayerBeenPlayedGlobal(DrawPool[k]))
                    {
                        Draw.Add(DrawPool[k]);
                    }
                    else
                    {
                        Draw.Add(DrawPool[k]);
                        Draw.Add(DrawPool[k]);
                    }
                }
                Random rn = new Random();
                int rand = rn.Next(0, Draw.Count);
                Result.Add(new TekkenMatch(DrawPool[0], Draw[rand]));               
                DrawPool.Remove(DrawPool[0]);
                DrawPool.Remove(Draw[rand]);
                Draw.Clear();
            }


            return Result;
        }
        static void Draw()
        {
            Console.Clear();
            Console.WriteLine("Draw Start");
            switch(Week)
            {
                case 1:
                    {
                        foreach (var players in PlayersInTier)
                        {
                            List<Player> Tech = new List<Player>(players.PlayerList);
                            players.PlayerList00 = Tech;
                            players.MatchesInTier = DrawMatches(players.PlayerList00);
                            Console.WriteLine("\n"+"Drawing Tier: " + players.Tier);
                            foreach (var match in players.MatchesInTier)
                            {
                                match.Print();
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        foreach (var players in PlayersInTier)
                        {
                            
                            foreach (var Player in players.PlayerList)
                            {
                                if(Player.Tier == players.Tier)
                                {
                                    if (Player.Wins == 1)
                                    {
                                        players.PlayerList10.Add(Player);
                                        Console.WriteLine($"Win 1 - {Player.Name}");
                                    }
                                    else
                                    {
                                        players.PlayerList01.Add(Player);
                                        Console.WriteLine($"Win 0 - {Player.Name}");
                                    }
                                }
                            }
                            players.MatchesInTier = DrawMatches(players.PlayerList10);
                            List<TekkenMatch> DrawMatchesTech = new List<TekkenMatch>();
                            DrawMatchesTech = DrawMatches(players.PlayerList01);
                            foreach (var match in DrawMatchesTech)
                            {
                                players.MatchesInTier.Add(match);
                            }
                            Console.WriteLine("\n"+"Drawing Tier: " + players.Tier);
                            foreach (var match in players.MatchesInTier)
                            {
                                match.Print();
                            }
                        }
                        break;
                    }
                case 3:
                    {

                        foreach (var players in PlayersInTier)
                        {
                            foreach (var Player in players.PlayerList)
                            {
                                if (Player.Wins == 2)
                                {
                                    players.PlayerList20.Add(Player);
                                }
                                else if (Player.Wins == 1)
                                {
                                    players.PlayerList11.Add(Player);
                                }
                                else
                                {
                                    players.PlayerList02.Add(Player);
                                }

                            }
                            players.MatchesInTier = DrawMatches(players.PlayerList20);
                            List<TekkenMatch> DrawMatchesTech = new List<TekkenMatch>();
                            DrawMatchesTech = DrawMatches(players.PlayerList11);
                            foreach (var match in DrawMatchesTech)
                            {
                                players.MatchesInTier.Add(match);
                            }
                            List<TekkenMatch> DrawMatchesTech1 = new List<TekkenMatch>();
                            DrawMatchesTech1 = DrawMatches(players.PlayerList02);
                            foreach (var match in DrawMatchesTech1)
                            {
                                players.MatchesInTier.Add(match);
                            }
                            Console.WriteLine("\n"+"Drawing Tier: " + players.Tier);
                            foreach (var match in players.MatchesInTier)
                            {
                                match.Print();
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        foreach (var players in PlayersInTier)
                        {
                            foreach (var Player in players.PlayerList)
                            {
                                if (Player.Wins == 2 && Player.PlayOffFlag == 1)
                                {
                                    players.PlayerListWeek4Up.Add(Player);
                                }
                                if (Player.Loses == 2 && Player.PlayOffFlag == 1)
                                {
                                    players.PlayerListWeek4Down.Add(Player);
                                }
                            }
                        }
                        for(int i=0;i< PlayersInTier.Count-1;i++)
                        {
                            PlayersInTier[i].MatchesInTier = DrawMatchesWeek4(PlayersInTier[i].PlayerListWeek4Down, PlayersInTier[i+1].PlayerListWeek4Up);
                            foreach(var match in PlayersInTier[i].MatchesInTier)
                            {
                                match.Print();
                            }
                        }
                        break;
                    }
                default: { break; }
            }
            Console.WriteLine();
            Console.WriteLine("Draw Ended");
            Console.ReadLine();
        }
        static void PrintPlayers(string inputtier)
        {
            Console.Clear();
            Console.WriteLine(inputtier);

            foreach (var players in PlayersInTier)
            {
                if (players.Tier == inputtier)
                {
                    foreach(var p in players.PlayerList)
                    {
                        p.Print();
                    }
                }
            }
        }
        static void PrintMatches(string inputtier)
        {
            Console.WriteLine(inputtier);
            foreach (var players in PlayersInTier)
            {
                if (players.Tier == inputtier)
                {
                    //foreach(var matches in  players.MatchesInTier)
                    for(int i = 0; i < players.MatchesInTier.Count; i++)
                    {
                        Console.Write($"{i}. ");
                        players.MatchesInTier[i].Print();
                    }
                        
                }
            }
        }
        static void PlayersMenu()
        {
            while (true) { 
                Console.Clear();
                Console.WriteLine("Choose tier:");
                Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
                foreach (var tier in PlayersInTier)
                {
                    Console.WriteLine( $"{tier.lp}. {tier.Tier}");
                    keyValuePairs.Add(tier.lp, tier.Tier);
                }
                Console.WriteLine("Q. Quit");
                
                string desision = Console.ReadLine();

                try
                {
                    if(desision != "Q")
                    {
                        foreach (var d in keyValuePairs)
                        {
                            if(d.Key == Convert.ToInt32(desision))
                            {
                                PrintPlayers(d.Value);
                                Console.WriteLine("Print any key");
                                Console.ReadLine();
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                catch { }

            }
        }
        static void UpadateScoreMenu(TekkenMatch match)
        {
            while(true) 
            {
                Console.Clear();
                Console.Write($"Chosen Match: ");
                match.Print();
                Console.WriteLine("1. Update Score");
                Console.WriteLine("Q. Quit");
                string desision = Console.ReadLine();

                if (desision == "Q")
                {
                    return;
                }
                else if (desision == "1")
                {
                    Console.Write($"Chosen Winner: 1.{match.Player1.Name} 2.{match.Player2.Name}");
                    string chosenwinner = Console.ReadLine();
                    while(true) 
                    {
                        if(chosenwinner == "1")
                        {
                            Console.WriteLine("Input how many round Looser Gain");
                            int score = Convert.ToInt32(Console.ReadLine());
                            match.ScorePlayer2 = score;
                            match.ScorePlayer1 = 3;
                            match.Score(match.Player1.Name);
                            break;
                        }
                        else if(chosenwinner == "2")
                        {
                            Console.WriteLine("Input how many round Looser Gain");
                            int score = Convert.ToInt32(Console.ReadLine());
                            match.ScorePlayer1 = score;
                            match.ScorePlayer2 = 3;
                            match.Score(match.Player2.Name);
                            break;
                        }

                    }
                }

            }
        }
        static void MatchMenuInside(string inputtier)
        {
            while(true) 
            {
                Console.Clear();
                Console.WriteLine($"Chosen tier: {inputtier}");
                List<TekkenMatch> Tech = new List<TekkenMatch>();
                Dictionary<int, TekkenMatch> keyValuePairs = new Dictionary<int, TekkenMatch>();
                foreach (var tier in PlayersInTier)
                {
                    if (tier.Tier == inputtier)
                    {
                        Tech = tier.MatchesInTier;
                    }
                }
                int lp = 0;
                foreach (var match in Tech)
                {
                    Console.Write($"{lp}. ");
                    match.Print();
                    keyValuePairs.Add(lp, match);
                    lp++;
                }
                Console.WriteLine("Q. Quit");
                string desision = Console.ReadLine();
                
                try
                {
                    if (desision != "Q")
                    {
                        foreach (var d in keyValuePairs)
                        {
                            if (d.Key == Convert.ToInt32(desision))
                            {
                                UpadateScoreMenu(d.Value);
                            }
                        }

                    }
                    else
                    {
                        return;
                    }
                }
                catch { }


            }
        }
        static void MatchMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose tier:");
                Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
                foreach (var tier in PlayersInTier)
                {
                    Console.WriteLine($"{tier.lp}. {tier.Tier}");
                    keyValuePairs.Add(tier.lp, tier.Tier);
                }
                Console.WriteLine("Q. Quit");

                string desision = Console.ReadLine();

                try
                {
                    if (desision != "Q")
                    {
                        foreach (var d in keyValuePairs)
                        {
                            if (d.Key == Convert.ToInt32(desision))
                            {
                                MatchMenuInside(d.Value);
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                catch { }

            }
        }
        static bool CheckNewWeek()
        {
            bool flag = true;
            foreach(var tier in PlayersInTier)
            {
                foreach(var match in tier.MatchesInTier)
                {
                    if (match.Winner == null)
                    flag = false;
                }               
            }
            if(!flag || PlayersInTier[0].MatchesInTier.Count == 0)
            {
                Console.WriteLine("Not all matches has scores you cannot proceed with next week");
            }
            else if ((!flag || PlayersInTier[0].MatchesInTier.Count == 0)&& Week==4)
            {
                Console.WriteLine("All matches has scores you can proceed with interation");
                Console.WriteLine("4. Next Week");
            }
            else 
            { 
                Console.WriteLine("All matches has scores you can proceed with next week");
                Console.WriteLine("4. Next Week");
            }
            return flag;
        }
        static void Menu()
        {
            string desision;
            while (true) 
            {
                Console.Clear();
                Console.WriteLine("Welcome to YOLO");
                Console.WriteLine("Actual State:");
                Console.WriteLine();
                Console.WriteLine("Week: " + Week);
                Console.WriteLine("Numer of Tiers: " + Tiery.Count);
                Console.WriteLine("Numer of Players: " + Players.Count);
                Console.WriteLine();
                Console.WriteLine("Menu");
                Console.WriteLine("1. Players");
                Console.WriteLine("2. Matches");
                Console.WriteLine("3. Draw");
                CheckNewWeek();
                Console.WriteLine("Q. Quit");
                desision = Console.ReadLine();
                switch(desision) 
                {
                    case "1":
                    {
                        PlayersMenu();
                        break;
                    }
                    case "2":
                    {
                        MatchMenu();
                        break;
                    }
                    case "3":
                    {
                        Draw();
                        break;
                    }
                    case "4":
                    {    
                        if(CheckNewWeek() && Week == 4)
                        {
                            foreach (var Tier in PlayersInTier)
                            {
                                Tier.MatchesInTier.Clear();
                            }
                            for (int i = 0; i < PlayersInTier.Count - 1; i++)
                            {
                                for (int j = 0; j< PlayersInTier[i].PlayerList.Count; j++)
                                {
                                    if (PlayersInTier[i].PlayerList[j].Result == "Down")
                                    {
                                        var temp = PlayersInTier[i].PlayerList[j];
                                        PlayersInTier[i + 1].PlayerList.Add(temp);
                                    }
                                    if (PlayersInTier[i + 1].PlayerList[j].Result == "Up")
                                    {
                                        var temp = PlayersInTier[i + 1].PlayerList[j];
                                        PlayersInTier[i].PlayerList.Add(temp);
                                    }
                                }
                            }
                            for (int i = 0; i < PlayersInTier.Count - 1; i++)
                            {
                                for (int j = PlayersInTier[i].PlayerList.Count - 1; j >= 0; j--)
                                {
                                    if (PlayersInTier[i].PlayerList[j].Result == "Down")
                                    {
                                        PlayersInTier[i].PlayerList.RemoveAt(j);
                                    }
                                }

                                for (int j = PlayersInTier[i + 1].PlayerList.Count - 1; j >= 0; j--)
                                {
                                    if (PlayersInTier[i + 1].PlayerList[j].Result == "Up")
                                    {
                                        PlayersInTier[i + 1].PlayerList.RemoveAt(j);
                                    }
                                }
                            }
                            foreach (var Tier in PlayersInTier)
                            {
                                Tier.MatchesInTier.Clear();
                                foreach(var p in Tier.PlayerList)
                                {
                                    p.Wins = 0;
                                    p.Loses = 0;
                                    p.MatchHistorySeason.Clear();
                                    p.PlayOffFlag = 0;
                                    p.Result = null;
                                }
                            }
                            Week = 1;
                        }
                        else if (CheckNewWeek())
                        {
                            foreach (var Tier in PlayersInTier)
                            {
                                Tier.MatchesInTier.Clear();
                            }
                            Week++;
                        }
                        break;
                    }
                    case "Q":
                    {
                       return;

                    }
                    default: 
                    { 
                        break; 
                    }
                }
                
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello");
            Console.WriteLine("Loading");
            LoadData();
            Console.WriteLine("Loading Complite");
            Console.WriteLine("Press any key");
            Console.ReadLine();
            Menu();
        }
    }
}

