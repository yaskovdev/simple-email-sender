// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleEmailSender;

internal static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var jsonFilePath = GetResourcePath("Resources/Config.json");
        var jsonString = File.ReadAllText(jsonFilePath);

        var config = JsonSerializer.Deserialize<Config>(jsonString, new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true
        });

        Console.WriteLine($"Sender: {config.Sender}");
        Console.WriteLine($"Test recipients: {string.Join(", ", config.TestRecipients)}");
        Console.WriteLine($"Recipients: {string.Join(", ", config.Recipients)}");

        using var smtpClient = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(config.Sender, config.SenderPassword)
        };
        Console.WriteLine("Testing...");
        foreach (var recipient in config.TestRecipients)
        {
            Console.WriteLine($"Sending email to {recipient}, confirm by pressing any key");
            Console.ReadKey();
            smtpClient.Send(config.Sender, recipient, "subject", "body");
        }
    }

    private static string GetResourcePath(string name)
    {
        var location = Assembly.GetExecutingAssembly().Location;
        var directory = Path.GetDirectoryName(location);
        return Path.Combine(directory ?? "", name);
    }
}