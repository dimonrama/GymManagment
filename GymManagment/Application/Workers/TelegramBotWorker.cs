using GymManagment.Application.Interfaces;
using GymManagment.Domain.Models;
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
        private readonly IServiceScopeFactory _scopeFactory;

        public TelegramBotWorker(
            ITelegramBotClient botClient,
            ILogger<TelegramBotWorker> logger,
            IServiceScopeFactory scopeFactory)
        {
            _botClient = botClient;
            _logger = logger;
            _scopeFactory = scopeFactory;
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
            _logger.LogInformation("Получено сообщение от {ChatId}", chatId);

            using var scope = _scopeFactory.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            if (messageText.StartsWith("/link"))
            {
                await HandleLinkCommandAsync(chatId, messageText, authService, cancellationToken);
                return;
            }

            var user = await authService.GetUserByTelegramChatIdAsync(chatId);
            if (user == null)
            {
                await botClient.SendMessage(chatId,
                    "Сначала привяжите аккаунт: /link <логин> <пароль>",
                    cancellationToken: cancellationToken);
                return;
            }

            if (user.Role == UserRole.Admin)
            {
                await botClient.SendMessage(chatId, $"Вы вошли как администратор ({user.Username}). Авторизация работает, но функционала для админов пока нет.", cancellationToken: cancellationToken);
                return;
            }

            if (user.Role == UserRole.Trainer)
            {
                await botClient.SendMessage(chatId, "Здесь пока ничего интересного для вас нет", cancellationToken: cancellationToken);
                return;
            }

            var memberService = scope.ServiceProvider.GetRequiredService<IMemberService>();
            var member = await memberService.GetMemberByUserIdAsync(user.Id);
            if (member == null)
            {
                await botClient.SendMessage(chatId, "Ваш аккаунт не привязан к профилю клиента, обратитесь к администратору", cancellationToken: cancellationToken);
                return;
            }

            string reply = $"Информация о вас:\nИмя: {member.FullName}\nВозраст: {member.Age}\nEmail: {member.Email}\n";

            if (member.TrainerId != null)
            {
                var trainerService = scope.ServiceProvider.GetRequiredService<ITrainerService>();
                var trainer = await trainerService.GetTrainerByIdAsync(member.TrainerId.Value);
                reply += trainer != null
                    ? $"\nВаш тренер: {trainer.FullName}"
                    : "\nТренер назначен, но не найден в базе";
            }

            await botClient.SendMessage(chatId, reply, cancellationToken: cancellationToken);
        }

        private async Task HandleLinkCommandAsync(long chatId, string messageText, IAuthService authService, CancellationToken cancellationToken)
        {
            var parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
            {
                await _botClient.SendMessage(chatId, "Использование: /link <логин> <пароль>", cancellationToken: cancellationToken);
                return;
            }

            var result = await authService.LinkTelegramAsync(parts[1], parts[2], chatId);
            string reply = result.Success
                ? "Аккаунт успешно привязан. Теперь просто напишите что-нибудь, чтобы получить информацию."
                : $"Не удалось привязать аккаунт: {result.ErrorMessage}";

            await _botClient.SendMessage(chatId, reply, cancellationToken: cancellationToken);
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Ошибка при получении обновлений от Telegram");
            return Task.CompletedTask;
        }
    }
}