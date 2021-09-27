using System;
using System.Collections.Generic;
using System.Text;

namespace UFF.VendingMachine.Interface
{
    public enum MessageCode
    {
        OutOfServise,
        OutOfStock,
        ReadyToService,

        MakeYourOrder,
        SelectOrderLine,
        SelectOrder,

        MakeYourPayment,
        OrderCancel,
        BalancePayment,
        Checkout,
        CollectYourPurchase,
        ReturnPayment,
        RunOutOfChange,
        GivenChange,
        InvalidInput,
        Ok,
    }

    /// <summary>
    /// Message to front end
    /// </summary>
    public interface IMessageRepo
    {
        string GetMessage(MessageCode code);
    }
}
