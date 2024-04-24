using Newtonsoft.Json;

namespace Readit
{
    class Program
    {
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
            List<Book> readList = new List<Book>();

            string userSearchTerm = GetSearchTermFromUser();
            var bookSearchResult = GetBookDataByTitle(userSearchTerm).Result;

            DisplaySearchResultToUser(bookSearchResult);
            int userBookChoiceIndex = GetBookChoiceFromUser(bookSearchResult);
            readList.Add(bookSearchResult[userBookChoiceIndex]);

            DisplayReadList(readList);
        }

        private static void DisplayReadList(List<Book> readList)
        {
            Console.WriteLine("Your current read list is:");
            for (int i = 0; i < readList.Count; i++)
            {
                var book = readList[i];
                Console.WriteLine("{0} by {1}", book.Title, book.AuthorName[0]);
            }
        }
        private static int GetBookChoiceFromUser(List<Book> bookSearchResult)
        {
            Console.Write("Please enter the number of the book you want to add to your read list: ");
            var userBookChoiceIndex = Int32.Parse(Console.ReadLine());
            while (userBookChoiceIndex < 0 || userBookChoiceIndex >= bookSearchResult.Count())
            {
                Console.Write("That is not a valid number, please try again: ");
                userBookChoiceIndex = Int32.Parse(Console.ReadLine());
            }

            return userBookChoiceIndex;
        }

        private static void DisplaySearchResultToUser(List<Book> bookSearchResult)
        {
            for (int i = 0; i < bookSearchResult.Count; i++)
            {
                var book = bookSearchResult[i];
                Console.WriteLine("{0}) {1} by {2}", i, book.Title, book.AuthorName[0]);
            }
        }

        private static string GetSearchTermFromUser()
        {
            Console.Write("Please enter a book title: ");
            string userSearchTerm = Console.ReadLine();
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
