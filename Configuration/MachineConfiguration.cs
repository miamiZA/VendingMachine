using System.Collections.Generic;
using System.IO;
using System.Linq;
using UFF.VendingMachine.Interface;
using UFF.VendingMachine.Models;

namespace UFF.VendingMachine.Configuration
{
    /// <summary>
    /// MachineConfiguration
    /// </summary>
    public class MachineConfiguration
    {
        /// <summary>
        /// Gets the max number of sections for StockItems.
        /// </summary>
        public int MaxSections { get; protected set; } = 20;

        /// <summary>
        /// Purse configuration settings.
        /// </summary>
        public List<ChangeDepositConfiguration> PurseConfiguration { get; set; } = new List<ChangeDepositConfiguration>();

        /// <summary>
        /// Product stock configuration settings. 
        /// </summary>
        public List<ProductConfiguration> StockConfiguration { get; set; } = new List<ProductConfiguration>();
    }
    public static class MachineConfigurationExtention
    {
        /// <summary>
        /// Loads StockItem Repo.
        /// </summary>
        /// <param name="machineConfig">The configuration.</param>
        /// <param name="productRepository">The product repository.</param>
        public static void LoadStockItems(this MachineConfiguration machineConfig, IStockItemRepo productRepository)
        {
            char currentKey = 'A';
            int sectionNumber = 0;
            foreach (var p in machineConfig.StockConfiguration)
            {
                if (sectionNumber >= machineConfig.MaxSections)
                {
                    throw new InvalidDataException($"Exceeded number of sections {machineConfig.MaxSections}");
                }

                var stockItem = new StockItem()
                {
                    Key = currentKey.ToString(),
                    DisplayName = p.Title,
                    Price = new StockPrice(0, p.Cost)
                };
                productRepository.UpdateItem(stockItem);
                productRepository.AddItem(currentKey.ToString(), (int)p.NumberInStock);
                currentKey++;
                sectionNumber++;
            }
        }

        /// <summary>
        /// Loads Items into Purse. 
        /// </summary>
        /// <param name="machineConfig"></param>
        /// <param name="purseRepository"></param>
        /// <param name="paymentReceiver"></param>
        public static void LoadPurse(this MachineConfiguration machineConfig, IPurseRepo purseRepository, IPaymentReceiver paymentReceiver)
        {
            foreach (var ch in machineConfig.PurseConfiguration)
            {
                if (paymentReceiver.AcceptedCoins.Any(c => c.Nominal == ch.Nominal))
                {
                    purseRepository.AddItem(new Coin(ch.Nominal), (int)ch.Number);
                }
                else
                {
                    throw new InvalidDataException($"Invalid type of nominal {ch.Nominal}");
                }

            }
        }
    }
}
