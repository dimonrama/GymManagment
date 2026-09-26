
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GymManagment.Application.Workers
{
    public class TelegramBotWorker : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<TelegramBotWorker> _logger;

        public TelegramBotWorker(ITelegramBotClient botClient, ILogger<TelegramBotWorker> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            };

        
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken);

            _logger.LogInformation("Telegram-бот запущен и слушает сообщения");

          
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

       
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            
            if (update.Message is not { Text: { } messageText } message)
                return;

            long chatId = message.Chat.Id;
            _logger.LogInformation("Получено сообщение от {ChatId}: {Text}", chatId, messageText);

            await botClient.SendMessage(
                chatId: chatId,
                text: $"Ты написал: {messageText}",
                cancellationToken: cancellationToken);
        }

     
        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Ошибка при получении обновлений от Telegram");
            return Task.CompletedTask;
        }
    }
}
