public class TestUtil
{

    public static string UniqueUsername()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        const int length = 10;

        Random random = new Random();
        string randomString = new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
        return randomString;
    }

    public static string RandomFingerprint()
    {
        const string chars = "0123456789abcdef";
        const int length = 64;

        Random random = new Random();
        string randomString = new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
        return randomString;
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        Random random = new Random();
        string randomString = new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
        return randomString;
    }

    public static string RandomProductId()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        const int length = 8;

        Random random = new Random();
        string randomString = new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());

        return "pr-" + randomString;
    }
    public static string RandomDeviceId()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        const int length = 8;

        Random random = new Random();
        string randomString = new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
        return "de-" + randomString;
    }
}
