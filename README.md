# Stock Quote Alert - Desafio Técnico INOA

Aplicação de console em C# (.NET 8) desenvolvida para monitorar continuamente a cotação de ativos da B3 e disparar alertas por e-mail quando o preço atingir gatilhos configuráveis de compra ou venda.

## 🚀 Arquitetura e Decisões Técnicas
- **Linguagem/Framework:** C# com .NET 8.
- **API de Cotações:** [Brapi](https://brapi.dev/), escolhida por sua estabilidade e facilidade de acesso a ativos da B3.
- **Serviço de E-mail:** Implementação nativa usando `System.Net.Mail` com suporte a autenticação SMTP segura.
- **Configurações:** Uso do `appsettings.json` (padrão da indústria) para injetar credenciais e chaves de API sem hardcoding, mantendo a segurança do código.
- **Assincronismo:** Uso intensivo de `Task` e `async/await` para garantir que as chamadas HTTP e de rede não bloqueiem a thread principal do sistema.

## ⚙️ Como executar o projeto

1. Clone o repositório:
   ```bash
   git clone [https://github.com/SEU_USUARIO/stock-quote-alert.git](https://github.com/SEU_USUARIO/stock-quote-alert.git)

2. Navegue até a pasta do projeto e preencha o arquivo appsettings.json com suas credenciais de e-mail e seu token da Brapi.

3. Execute via linha de comando passando os 3 parâmetros obrigatórios (Ativo, Preço de Venda, Preço de Compra):

dotnet run PETR4 22.67 22.59