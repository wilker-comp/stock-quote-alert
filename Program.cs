using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
//senti dificuldade nesse codigo na parte da escrita pois n c# n tenho muita initimidade e um pouco na parte da logica mas no geral consegui desenrolar apenas 1 parte foi gerada por IA
namespace StockQuoteAlert
{
  class Program
  {
    static async Task Main(string[] args)
    {
      if (args.Length != 3)
      {
        Console.WriteLine("Erro: Número de parâmetros inválido.");
        Console.WriteLine("Uso correto: StockQuoteAlert <ATIVO> <PRECO_VENDA> <PRECO_COMPRA>");
        Console.WriteLine("Exemplo: StockQuoteAlert PETR4 22.67 22.59");
        return;
      }

      string asset = args[0].ToUpper();

      if (!decimal.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal sellTargetPrice))
      {
        Console.WriteLine($"Erro: O preço de venda '{args[1]}' não é numérico.");
        return;
      }

      if (!decimal.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal buyTargetPrice))
      {
        Console.WriteLine($"Erro: O preço de compra '{args[2]}' não é numérico.");
        return;
      }

      if (buyTargetPrice >= sellTargetPrice)
      {
        Console.WriteLine("Aviso: O preço de compra é maior/igual ao de venda. Verifique sua estratégia.");
      }

      Console.WriteLine("--- Inicializando Monitoramento ---");
      Console.WriteLine($"Ativo: {asset}");
      Console.WriteLine($"Preço alvo para Venda (Azul): {sellTargetPrice:C}");
      Console.WriteLine($"Preço alvo para Compra (Vermelha): {buyTargetPrice:C}");
      Console.WriteLine("-----------------------------------\n");

      //aqui eh pra carregar config
      IConfigurationRoot configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .Build();

      string? apiToken = configuration["ApiSettings:BrapiToken"];
      string? destEmail = configuration["EmailSettings:DestinationEmail"];
      string? smtpServer = configuration["EmailSettings:SmtpServer"];
      string? smtpPortStr = configuration["EmailSettings:SmtpPort"];
      string? smtpUser = configuration["EmailSettings:SmtpUser"];
      string? smtpPass = configuration["EmailSettings:SmtpPass"];

      if (string.IsNullOrEmpty(apiToken) || string.IsNullOrEmpty(destEmail) || string.IsNullOrEmpty(smtpServer) ||
          string.IsNullOrEmpty(smtpPortStr) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
      {
        Console.WriteLine("Erro: Configurações ausentes no appsettings.json.");
        return;
      }

      // iniciar os svc
      StockApiClient apiClient = new StockApiClient(apiToken);
      EmailService emailService = new EmailService(smtpServer, int.Parse(smtpPortStr), smtpUser, smtpPass);

      Console.WriteLine("Iniciando loop de monitoramento. Pressione Ctrl+C para encerrar.\n");

      // aqui eh um loop para monitorar, n sabia fzr pedi ajuda
      while (true)
      {
        try
        {
          decimal currentPrice = await apiClient.GetCurrentPriceAsync(asset);
          Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Preço atual de {asset}: {currentPrice:C}");

          if (currentPrice > sellTargetPrice)
          {
            Console.WriteLine("-> Preço acima da linha azul! Disparando e-mail de VENDA.");
            string subject = $"ALERTA DE VENDA: {asset}";
            string body = $"A cotação de {asset} subiu para {currentPrice:C}, ultrapassando seu alvo de venda de {sellTargetPrice:C}.\nÉ hora de vender!";
            await emailService.SendAlertAsync(destEmail, subject, body);
          }
          else if (currentPrice < buyTargetPrice)
          {
            Console.WriteLine("-> Preço abaixo da linha vermelha! Disparando e-mail de COMPRA.");
            string subject = $"ALERTA DE COMPRA: {asset}";
            string body = $"A cotação de {asset} caiu para {currentPrice:C}, ficando abaixo do seu alvo de compra de {buyTargetPrice:C}.\nÉ hora de comprar!";
            await emailService.SendAlertAsync(destEmail, subject, body);
          }

          // adicionei um tempo para n carregar a api, ja tinha esse bizu pois trabalhei uma vez com api e ja deu erro nisso
          await Task.Delay(30000);
        }
        catch (Exception ex)
        {
          Console.WriteLine($"\nErro durante o monitoramento: {ex.Message}");
          Console.WriteLine("Tentando novamente em 30 segundos...");
          await Task.Delay(30000);
        }
      }
    }
  }
}