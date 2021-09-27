using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using UFF.VendingMachine.Configuration;
using UFF.VendingMachine.Interface;

namespace UFF.VendingMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Start the Application
               var prog = new Program();
                prog.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Occured: {0}", ex.Message);
            }
            finally
            {
                Thread.Sleep(15000);
            }
        }
        private void Start()
        {
            ServiceProvider sp = Initialize();
            try
            {
                //lets do initial load of stock into the machine.
                LoadConfiguration(sp);
                var state = sp.GetService<IState>();
                Console.WriteLine("=====================================");
                Console.WriteLine("Starting Vending Machine...");
                state.On();
                Thread.Sleep(1000);
                // Wait until "turned off"
                state.Completed.Wait();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
        }
        /*
         Initialize Dependancy injection
         */
        private ServiceProvider Initialize()
        {
            ServiceCollection services = new ServiceCollection();
            //as Service to service collection
            services.AddSingleton<IPurseRepo, PurseRepo>();
            services.AddSingleton<IStockItemRepo, StockItemRepo>();
            services.AddSingleton<IReadInput, ReadInput>();
            services.AddSingleton<IDisplay, DisplayMsg>();
            services.AddSingleton<IPaymentReceiver, PaymentReceiver>();
            services.AddSingleton<ISoldItem, SoldItem>();
            services.AddSingleton<IOrderItem, OrderItem>();
            services.AddSingleton<IMessageRepo, MessageRepo>();
            services.AddSingleton<IState, StateController>();

            return services.BuildServiceProvider();
        }
        private void LoadConfiguration(ServiceProvider serviceProvider)
        {
            try
            {
                var StockItems = serviceProvider.GetService<IStockItemRepo>();
                var purse = serviceProvider.GetService<IPurseRepo>();
                var payment = serviceProvider.GetService<IPaymentReceiver>();

                ConfigurationBuilder configBuilder = new ConfigurationBuilder();
                //gets stockitems to load from config file
                configBuilder.AddJsonFile("Settings.json");
                IConfigurationRoot config = configBuilder.Build();
          
                MachineConfiguration machineConfig = config.Get<MachineConfiguration>();

                if (machineConfig.PurseConfiguration.Count == 0)
                {
                    throw new InvalidDataException("No change in System!");
                }

                if (machineConfig.StockConfiguration.Count == 0)
                {
                    throw new InvalidDataException("No Stock in Inventory");
                }
                machineConfig.LoadStockItems(StockItems);
                machineConfig.LoadPurse(purse, payment);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
        }
    }
}
