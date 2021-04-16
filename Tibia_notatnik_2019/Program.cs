using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Threading;
using System.Linq;
using static System.Console;
using System.IO;
namespace Tibia_notatnik_2019
{
    class Program
    {
        static string Filter(string world)
        {
            // Leaves only nicknames in string
            world = world.Replace("&quot;", "");
            world = world.Replace("name", "\n");
            world = world.Replace("vocation", "");
            world = world.Replace("Royal Paladin", "");
            world = world.Replace("Paladin", "");
            world = world.Replace("Elite Knight", "");
            world = world.Replace("Knight", "");
            world = world.Replace("Master Sorcerer", "");
            world = world.Replace("Sorcerer}", "");
            world = world.Replace("Elder Druid", "");
            world = world.Replace("Druid", "");
            world = world.Replace("None", "");
            world = world.Replace("level", "");
            world = world.Replace(",", "");
            world = world.Replace(":", "");
            world = world.Replace(",", "");
            world = world.Replace("[", "");
            world = world.Replace("]", "");
            world = world.Replace("{", "");
            world = world.Replace("}", "");
            for (int Number = 0; Number < 10; Number++)
                world = world.Replace(Number.ToString(), "");
            return world;
        }
        static void Write_Online(string Name_of_world, List<string> My_Nicks, List<string> Beep_List)
        {
            var web = new HtmlAgilityPack.HtmlWeb();
            HtmlDocument doc = web.Load("https://refugian-association.ml/v2/read&key=ZLS-RE5-450-474-WDQ&w=" + Name_of_world);
            string Raw_string = (doc.DocumentNode.SelectNodes("/html")[0].InnerText);// loads content from website into string
            Raw_string = (Raw_string.Substring(Raw_string.LastIndexOf("players_online") + 21));// Deletes first useless part of string
            Raw_string = Filter(Raw_string.Remove((Raw_string.IndexOf("information"))));// Deletes the ending useless part of string and filters it
            List<string> Nicks_Online = Raw_string.Split('\n').ToList(); //LINQ
            for (int Index_of_Nicks_Online = 0; Index_of_Nicks_Online < Nicks_Online.Count; Index_of_Nicks_Online++)
            {
                for (int Index_of_My_Nicks = 0; Index_of_My_Nicks < My_Nicks.Count; Index_of_My_Nicks++)//Comparing 2 lists of nicknames
                    if (My_Nicks[Index_of_My_Nicks] == Nicks_Online[Index_of_Nicks_Online])
                    {
                        WriteLine("vip-" + Name_of_world + ": " + My_Nicks[Index_of_My_Nicks]);
                        if (Beep_List.Contains(Nicks_Online[Index_of_Nicks_Online]))
                            Beep(4000, 1000);
                    }
            }
        }
        static void Main(string[] args)
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName; //Get's path of project
            projectDirectory = (projectDirectory + "\\Dane.txt"); // Turn path of project into path of Dane.txt file
            string Note_File_path = @projectDirectory;
            List<string> Beep_List = new List<string>();
            List<string> My_nicks_List = new List<string>();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int Other_than_web_exception = 0;
            while (Other_than_web_exception == 0)
            {
                try
                {
                    int Count = 0;
                    while (true)
                    {
                        using (StreamReader Stream = File.OpenText(Note_File_path))
                        {
                            List<string> Note_To_List = File.ReadLines(Note_File_path).ToList();
                            int count = 0;
                            while (Note_To_List[count] != "")
                            {
                                Beep_List.Add(Note_To_List[count]);
                                count++;
                            }
                            while (Note_To_List.Count - 1 != count)// Iteration untill the end of file
                            {
                                count++; // Jumping one line gap between segments
                                while (Note_To_List[count] != "")
                                {
                                    My_nicks_List.Add(Note_To_List[count]);
                                    count++;
                                }
                                Write_Online(My_nicks_List[0], My_nicks_List, Beep_List);
                                My_nicks_List.Clear();
                            }
                        }
                        Count++;
                        WriteLine("Loop: " + Count);
                        watch.Stop();
                        var Time_of_compilation = watch.ElapsedMilliseconds;
                        if (100000 > Time_of_compilation)
                            Thread.Sleep(120000 - Convert.ToInt32(Time_of_compilation));
                        if (Count % 20 == 0)
                            Clear();
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    WriteLine("Couldn't connect to refugian api");
                }
                catch (System.Net.WebException)
                {
                    WriteLine("Web Exception");
                }
                catch (Exception ex)
                {
                    Other_than_web_exception++;
                    WriteLine(ex.ToString());
                    ReadLine();
                }
            }
        }
    }
}