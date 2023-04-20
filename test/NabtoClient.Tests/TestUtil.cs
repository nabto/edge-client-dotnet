public class TestUtil {

    public static string UniqueUsername() {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        const int length = 10;

        Random random = new Random();
        string randomString = new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
        return randomString;
    }
}