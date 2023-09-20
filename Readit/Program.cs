using Newtonsoft.Json;

namespace Readit
{
    class Program
    {

        static public void Main(string[] args)
        {
            string userSearchTerm = GetSearchTermFromUser();
            var bookSearchResult = GetBookDataByTitle(userSearchTerm);
            Console.WriteLine(bookSearchResult.Result);
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

        private static async Task<string> GetBookDataByTitle(string bookTitle)
        {
            var url = "https://openlibrary.org/search.json?q=" + bookTitle;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string searchResultTask = await response.Content.ReadAsStringAsync();
                    return searchResultTask;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
