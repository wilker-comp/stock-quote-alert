using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
//90% desse codigo pedi ajuda, realmente nunca tentei fazer isso
namespace StockQuoteAlert
{
  public class EmailService
  {
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass;

    public EmailService(string smtpServer, int smtpPort, string smtpUser, string smtpPass)
    {
      _smtpServer = smtpServer;
      _smtpPort = smtpPort;
      _smtpUser = smtpUser;
      _smtpPass = smtpPass;
    }

    public async Task SendAlertAsync(string toEmail, string subject, string body)
    {
      try
      {
        using var client = new SmtpClient(_smtpServer, _smtpPort)
        {
          Credentials = new NetworkCredential(_smtpUser, _smtpPass),
          EnableSsl = true // n sabia mas aparentemente eh obrigatorio para gmail
        };

        using var mailMessage = new MailMessage
        {
          From = new MailAddress(_smtpUser),
          Subject = subject,
          Body = body,
          IsBodyHtml = false,
        };
        mailMessage.To.Add(toEmail);

        Console.WriteLine("\nEnviando e-mail de alerta...");
        await client.SendMailAsync(mailMessage);
        Console.WriteLine($"[SUCESSO] E-mail enviado para {toEmail}!");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"\n[ERRO E-MAIL] Falha ao enviar: {ex.Message}");
      }
    }
  }
}