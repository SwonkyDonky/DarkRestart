using System;
using System.IO;

namespace TextAdventureGame
{
    class GameManager
    {
        private StoryManager storyManager;
        private string saveFilePath = "save.txt";

        public GameManager()
        {
            storyManager = new StoryManager("story.json");
        }

        public void Run()
        {
            while (true)
            {
                ShowMenu();
                string choice = Console.ReadLine()?.Trim().ToLower();

                switch (choice)
                {
                    case "1":
                        StartNewGame();
                        break;
                    case "2":
                        if (IsValidSaveFile())
                        {
                            ContinueGame();
                        }
                        else
                        {
                            Console.WriteLine("No valid saved game found!");
                        }
                        break;
                    case "3":
                        Console.WriteLine("You cannot escape your fate.");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Dark Restart ===");
            Console.WriteLine("1. New Game");
            if (IsValidSaveFile()) Console.WriteLine("2. Continue");
            Console.WriteLine("3. Quit Game");
            Console.WriteLine("===========================");
            Console.Write("Choose an option: ");
        }

        // Checks for valid save Id for loading in save
        private bool IsValidSaveFile()
        {
            if (File.Exists(saveFilePath))
            {
                string content = File.ReadAllText(saveFilePath).Trim();
                return !string.IsNullOrEmpty(content) && int.TryParse(content, out _); 
            }
            return false;
        }

        private void StartNewGame()
        {
            File.Delete(saveFilePath);
            storyManager.StartStory("tutorial");
        }

        private void ContinueGame()
        {
            string lastStoryId = File.ReadAllText(saveFilePath).Trim();
            storyManager.StartStory(lastStoryId);
        }
    }
}
