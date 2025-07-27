using BCrypt;
namespace ConsoleApp1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string password = "hoanyu123";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            Console.WriteLine(hashedPassword);
        }

    }
}
