// public class FooProgram
// {
//     public static async Task Main()
//     {
//         Console.WriteLine("Hello World!");

//         HttpClient client = new HttpClient();

//         // Call asynchronous network methods in a try/catch block to handle exceptions.

//         try
//         {
//             System.Net.Http.HttpContent c;
//             using (var response = await client.GetAsync("https://www.google.com/")) {
//                 response.EnsureSuccessStatusCode();

//                 string responseBody = await response.Content.ReadAsStringAsync();
//                 c = response.Content;
//                 // Above three lines can be replaced with new helper method below
//                 // string responseBody = await client.GetStringAsync(uri);

// //                Console.WriteLine(responseBody);
//             }
//             string body = await c.ReadAsStringAsync();
//             Console.WriteLine(body);
//         }
//         catch (HttpRequestException e)
//         {
//             Console.WriteLine("\nException Caught!");
//             Console.WriteLine("Message :{0} ", e.Message);
//         }
//     }
// }


public class InteractiveCli{

    public static async Task<int> Main()
    {
        await using (var connectionManager = new ConnectionManager()) {
            connectionManager.Start();
            CommandInvoker invoker = new CommandInvoker(connectionManager);
            invoker.Run();
        }
        Console.WriteLine("Exiting.");
        return 0;
    }
}

