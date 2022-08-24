using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Consumer;

public abstract class KafkaBaseConsumer<T> : BackgroundService, IConsumerService<T> where T : class
{
    protected readonly ILogger<KafkaBaseConsumer<T>> _logger;

    public KafkaBaseConsumer(ILogger<KafkaBaseConsumer<T>> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        new Thread(async () =>
        {
            var configuracaoConsumer = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092,localhost:9093", //Local que o kafka esta executando
                GroupId = GetConsumerGroup(), //Grupo que esse consumer vai ficar escutando
                AutoOffsetReset = AutoOffsetReset.Earliest, //define que tem que pegar todas as msgs que não foram consumidas até o momento, Podendo ser: Mais recente, Mais Antiga ou as que Teve erro
                MaxInFlight = 1, //Quantidade de solicitações em andamento por conexão do broker
                MaxPartitionFetchBytes = 1048576, //Tamanho máximo inicial de bytes por topico/partição a serem solicitado ao buscar mensagem do broker
                EnableAutoCommit = false, //Definimos como false para que se houver deslocamento das msg's entre as partições não perca as msg's que foram re-ordenadas
                AllowAutoCreateTopics = true //Para permitir que seja criado os topicos no broker caso eles ainda não exista
            };
            
            //Passa as configuração para o criar o consumer e caso der erro podemos logar o erro
            var consumer = new ConsumerBuilder<string, string>(configuracaoConsumer)
                .SetValueDeserializer(Deserializers.Utf8)
                .SetLogHandler((c, logMessage) =>
                {
                    if (logMessage.Level != SyslogLevel.Error) return;
                    _logger.LogError($"Erro KafkaConsumer: {logMessage.Message}");
                })
                .Build();
            
            //Informar o topico que esta responsavel por consumir
            consumer.Subscribe(GetTopic());
            
            ConsumeResult<string, string> consumerResult = null;
            
            //Fica verificando se chegou alguma msg
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    consumerResult = consumer.Consume(stoppingToken);
                    var menssagemValue = JsonConvert.DeserializeObject<T>(consumerResult.Message.Value);
                    var parsedMessagem = new Message<string, T>
                    {
                        Key = consumerResult.Message.Key,
                        Value = menssagemValue
                    };

                    await Parse(parsedMessagem); //Repassa a msg tratada para a classe que extende o KafkaBaseConsumer
                    consumer.Commit(); //Commita a msg informando que foi consumida
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, $"Erro ao consumir mensagem: {e.Error.Reason}");
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
                catch (JsonReaderException e)
                {
                    _logger.LogError($"Erro de conversão JSON ao executar o consumer: {consumerResult.Message.Value}");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Erro ao executar o consumer Message : {e.Message}");
                    //_logger.LogError(e, $"Erro ao executar o consumer Stack : {e.StackTrace}");
                }
            }
        }).Start();
        return Task.CompletedTask;
    }


    public abstract Task Parse(Message<string, T> message);
    public abstract string GetTopic();
    public abstract string GetConsumerGroup();
}