﻿using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Readit
{
    class Program
    {
        private static List<string> menuNumberChoicesAsString = new List<string>() { "1", "2", "3", "4" };

        public class Book
        {
            [JsonProperty("title")]
            public string? Title { get; set; }

            [JsonProperty("author_name")]
            public List<string>? AuthorName { get; set; }
        }

        public class BookList
        {
            [JsonProperty("docs")]
            public List<Book>? Results { get; set; }
        }


        static public void Main(string[] args)
        {

            List<Book> readList;
            if (File.Exists("readListSave"))
            {
                readList = LoadReadListFromFile();
            }
            else
            {
                readList = new List<Book>();
            }

            while (true)
            {
                DisplayMainMenu();
                var userMenuChoice = GetMainMenuChoiceFromUser();
                if (userMenuChoice == "viewReadList")
                {
                    DisplayReadList(readList);
                }
                else if (userMenuChoice == "addToReadList")
                {
                    string userSearchTerm = GetSearchTermFromUser();
                    var bookSearchResult = GetBookDataByTitle(userSearchTerm).Result;

                    DisplayBookListToUser(bookSearchResult);
                    int userBookChoiceIndex = GetBookChoiceFromUser(bookSearchResult) - 1;
                    readList.Add(bookSearchResult[userBookChoiceIndex]);
                }
                else if (userMenuChoice == "removeFromReadList")
                {
                    RemoveBookFromReadList(readList);
                }
                else if (userMenuChoice == "exitReadit")
                {
                    SaveReadListToFile(readList);
                    System.Environment.Exit(0);
                }

            }
        }

        private static List<string> GenerateListOfNumbersAsString(List<Book> listOfBooks)
        {
            var listOfNumbers = new List<string>();
            for (int i = 1; i <= listOfBooks.Count; i++)
            {
                listOfNumbers.Add(i.ToString());
            }
            return listOfNumbers;
        }

        private static void SaveReadListToFile(List<Book> readList)
        {
            using (StreamWriter file = File.CreateText("readListSave"))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, readList);
            }
        }

        private static List<Book> LoadReadListFromFile()
        {
            using (var file = File.OpenText("readListSave"))
            {
                var serializer = new JsonSerializer();
                return (List<Book>)serializer.Deserialize(file, typeof(List<Book>));
            }
        }

        private static void DisplayMainMenu()
        {
            Console.WriteLine("What would you like to do");
            Console.WriteLine("1) Add a book to your read list");
            Console.WriteLine("2) Remove a book from read list");
            Console.WriteLine("3) View your read list");
            Console.WriteLine("4) Exit Readit");
        }

        private static string GetMainMenuChoiceFromUser()
        {
            Dictionary<int, string> indexFromUserToMenuAction = new Dictionary<int, string>();
            indexFromUserToMenuAction.Add(1, "addToReadList");
            indexFromUserToMenuAction.Add(2, "removeFromReadList");
            indexFromUserToMenuAction.Add(3, "viewReadList");
            indexFromUserToMenuAction.Add(4, "exitReadit");
            Console.WriteLine("Please enter your choice: ");
            var userMenuStringInput = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(userMenuStringInput) || !menuNumberChoicesAsString.Contains(userMenuStringInput)) 
            {
                Console.WriteLine("Your choice was not valid, please enter your choice: ");
                userMenuStringInput = Console.ReadLine();
            }
            var userMenuChoice = Int32.Parse(userMenuStringInput);
            return indexFromUserToMenuAction[userMenuChoice];
        }

        private static void DisplayReadList(List<Book> readList)
        {
            Console.WriteLine("Your current read list is:");
            DisplayBookListToUser(readList);
        }

        private static void RemoveBookFromReadList(List<Book> readList)
        {
            DisplayReadList(readList);
            Console.WriteLine("Please enter the number of the book you would like to remove");
            var userBookChoiceAsString = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(userBookChoiceAsString) || !GenerateListOfNumbersAsString(readList).Contains(userBookChoiceAsString))
            {
                Console.WriteLine("Your choice was not valid, please enter your choice: ");
                userBookChoiceAsString = Console.ReadLine();
            }
            int bookIndexToDelete = Int32.Parse(userBookChoiceAsString) - 1;
            readList.RemoveAt(bookIndexToDelete);
        }

        private static int GetBookChoiceFromUser(List<Book> bookSearchResult)
        {
            Console.Write("Please enter the number of the book you want to add to your read list: ");
            var userBookChoiceAsString = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(userBookChoiceAsString) || !GenerateListOfNumbersAsString(bookSearchResult).Contains(userBookChoiceAsString))
            {
                Console.WriteLine("Your choice was not valid, please enter the number of the book you want to add: ");
                userBookChoiceAsString = Console.ReadLine();
            }

            return Int32.Parse(userBookChoiceAsString);
        }

        private static void DisplayBookListToUser(List<Book> bookList)
        {
            for (int i = 1; i <= bookList.Count; i++)
            {
                var book = bookList[i-1];
                if (book.AuthorName == null)
                {
                    List<string> AuthorName = new List<string>() { "Unknown" };
                    book.AuthorName = AuthorName;
                }
                Console.WriteLine("{0}) {1} by {2}", i, book.Title, book.AuthorName[0]);
            }
        }

        private static string GetSearchTermFromUser()
        {
            Console.Write("Please enter a book title: ");
            var userSearchTerm = Console.ReadLine();
            while (string.IsNullOrEmpty(userSearchTerm))
            {
                Console.Write("That is empty, please enter a book title: ");
                userSearchTerm = Console.ReadLine();
            }
            return userSearchTerm;
        }

        private static async Task<List<Book>> GetBookDataByTitle(string bookTitle)
        {
            var url = "https://openlibrary.org/search.json?q=" + bookTitle;
            List<Book> topFiveBooks = new List<Book>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string searchResultTask = await response.Content.ReadAsStringAsync();
                    BookList bookList = JsonConvert.DeserializeObject<BookList>(searchResultTask);
                    for (int i = 0; i<5; i++)
                    {
                        topFiveBooks.Add(bookList.Results[i]);
                    }
                    return topFiveBooks;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
