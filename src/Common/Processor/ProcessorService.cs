using Common.Logging;
using Contracts.StateMachines;
using MassTransit;
using MassTransit.DapperIntegration;
using MassTransit.RabbitMqTransport;
using MassTransit.Saga;
using MassTransit.Util;
using System;

namespace Processor
{
    public class ProcessorService
    {
        private const int MAX_NUMBER_OF_PROCESSING_MESSAGES = 8;
        private readonly ILog logger;
        private IBusControl busControl;
        private BusHandle busHandler;
        public ProcessorService()
        {
            logger = LogManager.GetLogger<ProcessorService>();

        }

        public void Start()
        {
            logger.Info("Starting bus");
            (busControl, busHandler) = CreateBus();
        }


        private (IBusControl, BusHandle) CreateBus()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(ConfigureBus);
            var busHandle = TaskUtil.Await(() => bus.StartAsync());
            return (bus, busHandle);
        }

        private void ConfigureBus(IRabbitMqBusFactoryConfigurator factoryConfigurator)
        {
            var rabbitHost = new Uri("rabbitmq://localhost");
            var inputQueue = "sagas-demo-proccessor";

            factoryConfigurator.Host(rabbitHost, ConfigureCredentials);
            factoryConfigurator.ReceiveEndpoint(inputQueue, ConfigureSagaEndpoint);
        }
        private void ConfigureCredentials(IRabbitMqHostConfigurator hostConfiurator)
        {
            var user = "guest";
            var password = "guest";
            hostConfiurator.Username(user);
            hostConfiurator.Password(password);
        }

        private void ConfigureSagaEndpoint(IRabbitMqReceiveEndpointConfigurator endPointConfigurator)
        {
            var stateMachine = new ProductCatalogStateMachine();
            var repository = CreateRepository();

            endPointConfigurator.PrefetchCount = MAX_NUMBER_OF_PROCESSING_MESSAGES;
            endPointConfigurator.StateMachineSaga(stateMachine, repository);
        }

        private ISagaRepository<ProductCatalogState> CreateRepository()
        {
            return new InMemorySagaRepository<ProductCatalogState>();
            //return DapperSagaRepository<ProductCatalogState>.Create("Server=(localdb)\\MSSQLLocalDB;Database=SagaDb;Trusted_Connection=True;MultipleActiveResultSets=false");
        }

        public void Stop()
        {
            logger.Info("Stopping bus");
            TryToStopBus();
        }

        private void TryToStopBus() =>
            busHandler?.Stop();
    }
}
