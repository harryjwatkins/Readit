namespace Readit
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }

    }

    class Program
    {

        static public void Main(string[] args)
        {
            SearchForBook();
        }

        public static void SearchForBook()
        {
            string userSearchTerm = GetSearchTermFromUser();
            var bookSearchResult = GetBookDataByTitle(userSearchTerm);
            Console.WriteLine(bookSearchResult);
        }

        private static string GetSearchTermFromUser()
        {
            Console.Write("Please enter a book title");
            string userSearchTerm = Console.ReadLine();
            return userSearchTerm;
        }

        private static async Task<string> GetBookDataByTitle(string bookTitle)
        {
            var url = "https://openlibrary.org/search.json?q=" + bookTitle;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string searchResult = await response.Content.ReadAsStringAsync();
                    return searchResult;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
