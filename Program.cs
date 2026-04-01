using System;
using System.Globalization;

namespace StockQuoteAlert
{
  class Program
  {
    static void Main(string[] args)
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
        Console.WriteLine($"Erro: O preço de venda '{args[1]}' não é um valor numérico válido.");
        return;
      }

      if (!decimal.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal buyTargetPrice))
      {
        Console.WriteLine($"Erro: O preço de compra '{args[2]}' não é um valor numérico válido.");
        return;
      }

      if (buyTargetPrice >= sellTargetPrice)
      {
        Console.WriteLine("Aviso lógico: O preço de compra está maior ou igual ao preço de venda. Verifique sua estratégia.");
      }

      Console.WriteLine("--- Inicializando Monitoramento ---");
      Console.WriteLine($"Ativo: {asset}");
      Console.WriteLine($"Preço alvo para Venda (Azul): {sellTargetPrice:C}");
      Console.WriteLine($"Preço alvo para Compra (Vermelha): {buyTargetPrice:C}");
      Console.WriteLine("-----------------------------------");
    }
  }
}