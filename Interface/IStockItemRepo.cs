using System;
using System.Collections.Generic;
using System.Text;
using UFF.VendingMachine.Models;
namespace UFF.VendingMachine.Interface
{
    public interface IStockItemRepo
    {
       
        bool UpdateItem(StockItem stockitem);

        /// <summary>
        /// Get a list of stockItems in the stockItem repo
        /// </summary>
        List<StockItem> StockItemList 
        { 
            get; 
        }

        /// <summary>
        /// Gets StockItem Count.
        /// </summary>
        /// <param name="key">The StockItem ID.</param>
        /// <returns>Number of StockItem.</returns>
        int StockCount(string key);

        /// <summary>
        /// Remove StockItem
        /// </summary>
        /// <param name="key">The StockItem ID.</param>
        /// <returns>The current stock after selling.</returns>
        int RemoveItem(string key);

        /// <summary>
        /// Add StockItem
        /// </summary>
        /// <param name="key">The StockItem ID.</param>
        /// <param name="stockAmount">StockAmount.</param>
        /// <returns>The current stock after.</returns>
        int AddItem(string key, int stockAmount);
    }
}
