﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

class Player
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
    public string Name, Tier;
    public int SeasonElo, GlobalElo, Wins, Loses, PlayOffFlag;
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
    public TekkenMatch(Player p1, Player p2) 
    {
        this.Player1 = p1;
        this.Player2 = p2;
    }
    public Player Player1;
    public Player Player2;
    public string Winner;
    public int ScorePlayer1, ScorePlayer2;
    public bool HasPlayerInMatch(Player player)
    {
        if (this.Player1 == player || this.Player2 == player) return true; 
        return false;
    }
    public void Print()
    {
        Console.WriteLine("Match:");
        Console.WriteLine("Player 1:" + this.Player1.Name + " Player 2:" + this.Player2.Name);
    }
}
class TierPlusPlayers
{
    public TierPlusPlayers(string Tier, int Week) 
    {

        this.Tier = Tier;
        this.MatchesInTier = new List<TekkenMatch>();
        switch(Week)
        {
            case 1: { this.PlayerList00 = new List<Player>();break;}
            case 2: { this.PlayerList10 = new List<Player>(); this.PlayerList01 = new List<Player>();break;}
            case 3: { this.PlayerList20 = new List<Player>(); this.PlayerList11 = new List<Player>(); this.PlayerList02 = new List<Player>(); break; }
            case 4: { this.PlayerList21 = new List<Player>(); this.PlayerList12 = new List<Player>(); break; }
        }
    }
    public string Tier;
    public List<Player> PlayerList00, PlayerList10, PlayerList01, PlayerList11, PlayerList20, PlayerList02, PlayerList12, PlayerList21;
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
        static void ZaladujDane()
        {
            string Path = @"Data\PlayerBase\Players.txt";
            using (StreamReader sr = File.OpenText(Path))
            {
                string s;
                while((s= sr.ReadLine())!=null)
                {  
                    Players.Add(new Player(s.Split(",")[0], s.Split(",")[1], System.Convert.ToInt32(s.Split(",")[2]), System.Convert.ToInt32(s.Split(",")[3]), System.Convert.ToInt32(s.Split(",")[4]), System.Convert.ToInt32(s.Split(",")[5]), System.Convert.ToInt32(s.Split(",")[6])));
                    Players[Players.Count - 1].Print();
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
            Console.WriteLine("Ładowanie danych zakończone");
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
        static void Losowanie()
        {
            Console.WriteLine("Draw Start");
            switch(Week)
            {
                case 1:
                    {
                        foreach (var Tier in Tiery)
                        {
                            PlayersInTier.Add(new TierPlusPlayers(Tier,Week));
                            foreach (var Player in Players)
                            {
                                if(Tier == Player.Tier)
                                {
                                    PlayersInTier[PlayersInTier.Count - 1].PlayerList00.Add(Player);
                                }
                            }
                        }
                        foreach (var players in PlayersInTier)
                        {
                            players.MatchesInTier = DrawMatches(players.PlayerList00);
                            Console.WriteLine("Drawing Tier: " + players.Tier);
                            foreach (var match in players.MatchesInTier)
                            {
                                match.Print();
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        foreach (var Tier in Tiery)
                        {
                            PlayersInTier.Add(new TierPlusPlayers(Tier, Week));
                            foreach (var Player in Players)
                            {
                                if (Tier == Player.Tier)
                                {
                                    if(Player.Wins == 1)
                                    {
                                        PlayersInTier[PlayersInTier.Count - 1].PlayerList10.Add(Player);
                                    }
                                    else
                                    {
                                        PlayersInTier[PlayersInTier.Count - 1].PlayerList01.Add(Player);
                                    }
                                }
                            }
                        }
                        foreach (var players in PlayersInTier)
                        {
                            players.MatchesInTier = DrawMatches(players.PlayerList10);
                            List<TekkenMatch> DrawMatchesTech = new List<TekkenMatch>();
                            DrawMatchesTech = DrawMatches(players.PlayerList01);
                            foreach(var match in DrawMatchesTech)
                            {
                                players.MatchesInTier.Add(match);
                            }
                            Console.WriteLine("Drawing Tier: " + players.Tier);
                            foreach (var match in players.MatchesInTier)
                            {
                                match.Print();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        foreach (var Tier in Tiery)
                        {
                            PlayersInTier.Add(new TierPlusPlayers(Tier, Week));
                            foreach (var Player in Players)
                            {
                                if (Tier == Player.Tier)
                                {
                                    if (Player.Wins == 2)
                                    {
                                        PlayersInTier[PlayersInTier.Count - 1].PlayerList20.Add(Player);
                                    }
                                    else if (Player.Wins == 1)
                                    {
                                        PlayersInTier[PlayersInTier.Count - 1].PlayerList11.Add(Player);
                                    }
                                    else
                                    {
                                        PlayersInTier[PlayersInTier.Count - 1].PlayerList02.Add(Player);
                                    }
                                }
                            }
                        }
                        foreach (var players in PlayersInTier)
                        {
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
                            Console.WriteLine("Drawing Tier: " + players.Tier);
                            foreach (var match in players.MatchesInTier)
                            {
                                match.Print();
                            }
                        }
                        break;
                    }
                case 4:
                    {

                        break;
                    }
                default: { break; }
            }
            Console.WriteLine("Draw Ended");  
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello");
            Console.WriteLine("Loading");
            ZaladujDane();
            Losowanie();
            Console.ReadLine();
        }
    }
}

