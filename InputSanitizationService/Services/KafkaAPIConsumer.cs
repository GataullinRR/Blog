using Utilities.Types;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StatisticServiceExports;
using StatisticServiceExports.Kafka;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace StatisticService.Services
{
    [Service(ServiceType.SINGLETON)]
    public class KafkaAPIConsumer
    {
#warning move to config
        const string TOPIC = "statistic";

        class BinaryKafkaDeserializer<T> : IDeserializer<T>
        {
            public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            {
                return new BinaryFormatter().Deserialize(new MemoryStream(data.ToArray())).To<T>();
            }
        }

        readonly IServiceScopeFactory _scopeBuilder;
        readonly IConsumer<Null, object> _consumer;
        readonly ILogger<KafkaAPIConsumer> _logger;

        public KafkaAPIConsumer(IServiceScopeFactory scopeBuilder, ILogger<KafkaAPIConsumer> logger)
        {
            _scopeBuilder = scopeBuilder ?? throw new ArgumentNullException(nameof(scopeBuilder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var conf = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = "localhost:9092",
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Null, object>(conf)
                .SetValueDeserializer(new BinaryKafkaDeserializer<object>())
                .Build();
            _consumer.Subscribe(TOPIC);

            dispatcherLoopAsync();
        }

        async Task dispatcherLoopAsync()
        {
            await ThreadingUtils.ContinueAtDedicatedThread();

            while (true)
            {
                try
                {
                    var message = _consumer.Consume();
                    var scope = _scopeBuilder
                        .CreateScope();
                    var controller = scope.ServiceProvider
                        .GetRequiredService<IStatisticServiceAPI>();
                    var parameter = message.Value;
                    var handler = getHandler();
                    executeAsync();

                    /////////////////////////////////////////////

                    Func<Task> getHandler()
                    {
                        return message.Value switch
                        {
                            CommentaryNotification cn => () => controller.OnCommentaryActionAsync(cn),
                            PostNotification pn => () => controller.OnPostActionAsync(pn),
                            SeenNotification sn => () => controller.OnSeenAsync(sn),
                            UserNotification un => () => controller.OnUserActionAsync(un),
                            _ => throw new NotSupportedException()
                        };
                    }

                    async void executeAsync()
                    {
                        using (scope)
                        {
                            await ThreadingUtils.ContinueAtThreadPull();

                            await handler();

                            _logger.LogInformation($"Consumed message '{message.Value}' at: '{message.TopicPartitionOffset}'.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, null);
                }
            }
        }
    }
}
