using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace TextAdventureGame
{
    class StoryManager
    {
        private Dictionary<string, StoryPart> storyParts;
        private string saveFilePath = "save.txt";

        private HashSet<string> displayedStoryIds;

        public StoryManager(string storyFilePath)
        {
            string json = File.ReadAllText(storyFilePath);
            storyParts = JsonConvert.DeserializeObject<Dictionary<string, StoryPart>>(json);

            displayedStoryIds = new HashSet<string>();
        }

        public void StartStory(string startId)
        {
            string currentId = startId;

            while (true)
            {
                if (!storyParts.ContainsKey(currentId))
                {
                    Console.WriteLine("Story ID not found. Game over.");
                    break;
                }

                StoryPart story = storyParts[currentId];
                Console.Clear();

                // Check if the story text for this ID has already been displayed
                if (!displayedStoryIds.Contains(currentId))
                {
                    if (TypeText(story.Text))
                    {
                        Console.Clear();
                        Console.WriteLine(story.Text);
                    }
                    displayedStoryIds.Add(currentId);
                }
                else
                {
                    Console.WriteLine(story.Text);
                }

                if (story.Death)
                {
                    Console.WriteLine("\nYOU DIED. \n\nReturning to the last save...");
                    WaitForPlayer();
                    currentId = File.Exists(saveFilePath) ? File.ReadAllText(saveFilePath).Trim() : "1";
                    continue;
                }

                if (story.Win)
                {
                    Console.WriteLine("\nCongratulations! You have won the game!");
                    WaitForPlayer();
                    break;
                }

                // Display options
                Console.WriteLine("\nOptions: ");
                var optionIds = new Dictionary<string, string>();
                int index = 1;

                foreach (var option in story.Options)
                {
                    Console.WriteLine($"{index}. {option.Value}");
                    optionIds.Add(option.Value.ToLower(), option.Key);
                    optionIds.Add(index.ToString(), option.Key);
                    index++;
                }

                Console.Write("What do you do? ");
                string input = Console.ReadLine()?.Trim().ToLower();

                // Handle input of player
                if (input == "hint")
                {
                    Console.WriteLine($"\nHint: {story.Hint}");
                    WaitForPlayer();
                }
                else if (input == "save")
                {
                    SaveGame(currentId);
                    Console.WriteLine("Game saved!");
                    WaitForPlayer();
                }
                else if (optionIds.ContainsKey(input))
                {
                    currentId = optionIds[input];
                }
                else
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    WaitForPlayer();
                }
            }
        }

        private void SaveGame(string currentId)
        {
            File.WriteAllText(saveFilePath, currentId);
        }

        private void WaitForPlayer()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private bool TypeText(string text)
        {
            bool skipped = false;

            foreach (char c in text)
            {
                if (Console.KeyAvailable)
                {
                    skipped = true;
                    break;
                }

                Console.Write(c);

                // Apply delays based on character type
                if (c == ',' || c == ';')
                {
                    Thread.Sleep(200);
                }
                else if (c == '.' || c == '!' || c == '?')
                {
                    Thread.Sleep(500);
                }
                else
                {
                    Thread.Sleep(50);
                }
            }

            if (skipped)
            {
                while (Console.KeyAvailable) Console.ReadKey(true);
            }

            Console.WriteLine();
            return skipped;
        }
    }

    class StoryPart
    {
        public string Text { get; set; }
        public string Hint { get; set; }
        public bool Death { get; set; }
        public bool Win { get; set; }
        public Dictionary<string, string> Options { get; set; }
    }
}
