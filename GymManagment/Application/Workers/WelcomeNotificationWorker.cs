

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace GymManagment.Application.Workers
{
    public class WelcomeNotificationWorker : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly ILogger<WelcomeNotificationWorker> _logger;
        private IChannel? _channel;

        public WelcomeNotificationWorker(IConnection connection, ILogger<WelcomeNotificationWorker> logger)
        {
            _connection = connection;
            _logger = logger;
        }

         
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            
            await _channel.QueueDeclareAsync(
                queue: "welcome-notifications",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

             
            var consumer = new AsyncEventingBasicConsumer(_channel);

           
            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                byte[] body = eventArgs.Body.ToArray();
                string json = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Получено сообщение из очереди welcome-notifications: {Json}", json);

                // Здесь в будущем будет реальная отправка (Telegram-бот, письмо и т.д.)
                // Пока просто имитируем работу небольшой задержкой
                await Task.Delay(500, stoppingToken);

                _logger.LogInformation("Приветствие обработано");

               
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            };

            
            await _channel.BasicConsumeAsync(
                queue: "welcome-notifications",
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);
 
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}

 