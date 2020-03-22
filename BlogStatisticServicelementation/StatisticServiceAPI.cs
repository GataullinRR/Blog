using Confluent.Kafka;
using Newtonsoft.Json;
using StatisticServiceExports;
using StatisticServiceExports.Kafka;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace StatisticServiceClient
{
    public class StatisticServiceAPI : IStatisticServiceAPI
    {
        class BinaryKafkaSerializer<T> : ISerializer<T>
        {
            public byte[] Serialize(T data, SerializationContext context)
            {
                var result = new MemoryStream();
                new BinaryFormatter().Serialize(result, data);

                return result.ToArray();
            }
        }

#warning move to config
        const string TOPIC = "statistic";

        readonly IProducer<Null, PostNotification> _postActionProducer;
        readonly IProducer<Null, CommentaryNotification> _commentaryActionProducer;
        readonly IProducer<Null, UserNotification> _userActionProducer;
        readonly IProducer<Null, SeenNotification> _seenProducer;

        public StatisticServiceAPI()
        {
#warning move to config
            var conf = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
            };

            _postActionProducer = createProducer<PostNotification>();
            _commentaryActionProducer = createProducer<CommentaryNotification>();
            _userActionProducer = createProducer<UserNotification>();
            _seenProducer = createProducer<SeenNotification>();

            IProducer<Null, T> createProducer<T>()
            {
                return new ProducerBuilder<Null, T>(conf).SetValueSerializer(new BinaryKafkaSerializer<T>()).Build();
            }
        }

        public async Task OnCommentaryActionAsync(CommentaryNotification info)
        {
            _commentaryActionProducer.Produce(TOPIC, new Message<Null, CommentaryNotification>() { Value = info });
        }

        public async Task OnSeenAsync(SeenNotification info)
        {
            _seenProducer.Produce(TOPIC, new Message<Null, SeenNotification>() { Value = info });
        }

        public async Task OnUserActionAsync(UserNotification info)
        {
            _userActionProducer.Produce(TOPIC, new Message<Null, UserNotification>() { Value = info });
        }

        public async Task OnPostActionAsync(PostNotification info)
        {
            _postActionProducer.Produce(TOPIC, new Message<Null, PostNotification>() { Value = info });
        }
    }
}
