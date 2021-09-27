
using UFF.VendingMachine.Models;

namespace UFF.VendingMachine.Interface
{
    public enum OrderEvent
    {
        OutOfStock,
        Select,
    }

    /// <summary>
    /// OrderItem
    /// </summary>
    public interface IOrderItem : ISwitch
    {
        /// <summary>
        ///Orderaction event
        /// </summary>
        event OrderAction OrderAction;

        /// <summary>
        /// exception occured
        /// </summary>
        event FailException FailException;

    }
    public delegate void OrderAction(OrderEvent cmd, StockItem obj);
}
