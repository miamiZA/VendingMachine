using UFF.VendingMachine.Interface;
using UFF.VendingMachine.Models;

namespace UFF.VendingMachine.Interface
{
    public interface ISoldItem
    {
        /// <summary>
        ///  Records the sale.
        /// </summary>
        /// <param name="stockItem"></param>
        void Sold(StockItem stockItem);
    }
}
