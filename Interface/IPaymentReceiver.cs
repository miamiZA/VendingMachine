using System;
using System.Collections.Generic;

namespace UFF.VendingMachine.Interface
{
    public enum PaymentEvent
    {
        Cancel,
        Payment
    }
    public interface IPaymentReceiver : ISwitch
    {
        /// <summary>
        /// Conis that will be accepted
        /// </summary>
        List<Coin> AcceptedCoins { get; }

        /// <summary>
        /// The map cose to the Coin
        /// </summary>
        Dictionary<char, Coin> MapCoins { get; }

        /// <summary>
        /// The payment action event.
        /// </summary>
        event PaymentAction paymentAction;

        /// <summary>
        /// Exception
        /// </summary>
        event FailException FailException;
    }
    public delegate void PaymentAction(PaymentEvent cmd, INotionalValue obj);
    public delegate void FailException(Exception ex);
}
