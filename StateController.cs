using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UFF.VendingMachine.Interface;
using UFF.VendingMachine.Models;

namespace UFF.VendingMachine
{
    public enum MachineState
    {
        TurnedOff,
        OutOfStock,
        Order,
        Payment,
        Fault
    }
    /// <summary>
    /// Implement the State Controller. 
    /// </summary>
    public class StateController : IState
    {
        TaskCompletionSource<int> _complete = new TaskCompletionSource<int>();
        readonly IDisplay _display;
        readonly IPaymentReceiver _paymentReceiver;
        readonly IOrderItem _orderItem;
        readonly IMessageRepo _MessageRepository;
        readonly IStockItemRepo _StockItemRepository;
        readonly IPurseRepo _PurseRepository;
        private ISoldItem _soldItem;

        public MachineState MachineState { get; set; }

        /// <summary>
        /// Consturctor
        /// </summary>
        /// <param name="display"></param>
        /// <param name="paymentReceiver"></param>
        /// <param name="orderItem"></param>
        /// <param name="stockItemRepository"></param>
        /// <param name="purseRepository"></param>
        /// <param name="soldItem"></param>
        /// <param name="messageRepository"></param>
        public StateController(IDisplay display,IPaymentReceiver paymentReceiver, IOrderItem orderItem,IStockItemRepo stockItemRepository,IPurseRepo purseRepository,ISoldItem soldItem,IMessageRepo messageRepository)
        {
            _MessageRepository = messageRepository;
            _display = display;
            _paymentReceiver = paymentReceiver;
            _paymentReceiver.paymentAction += PaymentAction;
            _paymentReceiver.FailException += FailException;
            _StockItemRepository = stockItemRepository;
            _PurseRepository = purseRepository;
            _orderItem = orderItem;
            _orderItem.OrderAction += OrderAction;
            _orderItem.FailException += FailException;
            _soldItem = soldItem;
        }

        #region IState interface

        public Task Completed => _complete.Task;

        void FailException(Exception ex)
        {
            MachineState = MachineState.Fault;
            _orderItem.Off();
            _paymentReceiver.Off();
            _complete.TrySetException(ex);
        }

        public void Off()
        {
            MachineState = MachineState.TurnedOff;
            DisplayMessageByCode(MessageCode.OutOfServise);
            _orderItem.Off();
            _paymentReceiver.Off();
            _complete.TrySetResult(0);
        }

        public void On()
        {
            StartTransaction();
        }

        #endregion

        private void StartTransaction()
        {
            if (_StockItemRepository.StockItemList.All((p) => _StockItemRepository.StockCount(p.Key) == 0))
            {
                MachineState = MachineState.OutOfStock;
                DisplayMessageByCode(MessageCode.OutOfStock);
            }
            else
            {
                MachineState = MachineState.Order;
                DisplayMessageByCode(MessageCode.ReadyToService);
                _orderItem.On();
            }
        }

        readonly PurseRepo _PurseRepo = new PurseRepo();
        internal PurseRepo PurseRepo => _PurseRepo;

        /// <summary>
        /// Processing payment events from the coin receiver
        /// </summary>
        /// <param name="cmd">The event to process.</param>
        /// <param name="payment">The coin received or null</param>
        void PaymentAction(PaymentEvent cmd, INotionalValue payment)
        {
            if (MachineState != MachineState.Payment)
            {
                return;
            }
            if (cmd == PaymentEvent.Cancel)
            {
                // Cancellation of the order or/and payment 
                CancelOrder();
            }
            else if (cmd == PaymentEvent.Payment)
            {
                _PurseRepo.AddItem(payment, 1);
                StockPrice total = _orderedStockItems.Aggregate(StockPrice.Zero, (m, p) => m + p.Price);
                StockPrice paid = _PurseRepo.ItemList.Aggregate(StockPrice.Zero, (m, p) => m + new StockPrice(0, (int)p.Item1.Nominal * p.Item2));
                string msg = String.Format(_MessageRepository.GetMessage(MessageCode.BalancePayment), total, paid);
                _display.DisplayMessage(msg);
                if (paid >= total)
                {
                    // give change or cancel
                    if (paid > total)
                    {
                        StockPrice change = paid - total;
                        PurseRepo collectedChange = Composer(_PurseRepository, change);
                        if (collectedChange == null)
                        {
                            DisplayMessageByCode(MessageCode.RunOutOfChange);
                            CancelOrder();
                            return;
                        }
                        foreach (var pm in collectedChange.ItemList)
                        {
                            msg = String.Format(_MessageRepository.GetMessage(MessageCode.GivenChange), pm.Item1.StockPriceValue, pm.Item2);
                            _display.DisplayMessage(msg);
                        }

                    }
                    // complete purchase
                    foreach (var p in _orderedStockItems)
                    {
                        msg = String.Format(_MessageRepository.GetMessage(MessageCode.CollectYourPurchase), p);
                        _soldItem.Sold(p);
                        _display.DisplayMessage(msg);
                    }

                    _orderedStockItems.Clear();
                    foreach (var pm in _PurseRepo.ItemList)
                    {
                        _PurseRepo.AddItem(pm.Item1, pm.Item2);
                    }

                    _PurseRepo.ClearItems();
                    // switch back to transaction
                    StartTransaction();
                    _paymentReceiver.Off();
                }
            }
        }

        private void CancelOrder()
        {
            MachineState = MachineState.Order;

            // Display cancel message 
            DisplayMessageByCode(MessageCode.OrderCancel);

            // Return paid money
            foreach (var pm in _PurseRepo.ItemList)
            {
                var msg = String.Format(_MessageRepository.GetMessage(MessageCode.ReturnPayment), pm.Item1.StockPriceValue,pm.Item2);
                _display.DisplayMessage(msg);
            }
            _PurseRepo.ClearItems();

            // Return reserved stock of product
            foreach (var p in _orderedStockItems)
            {
                _StockItemRepository.AddItem(p.Key, 1);
            }
            _orderedStockItems.Clear();

            _paymentReceiver.Off();
            StartTransaction();
        }

        Stack<StockItem> _orderedStockItems = new Stack<StockItem>();

        internal List<StockItem> OrderedStockItems => _orderedStockItems.ToList();
        void OrderAction(OrderEvent cmd, StockItem obj)
        {
            if (MachineState != MachineState.Order)
            {
                return;
            }
            if (cmd == OrderEvent.Select)
            {
                if (_StockItemRepository.StockCount(obj.Key) > 0)
                {
                    _orderedStockItems.Push(obj);
                    string msg = String.Format(_MessageRepository.GetMessage(MessageCode.Checkout), obj);
                    _display.DisplayMessage(msg);
                    _StockItemRepository.RemoveItem(obj.Key);
                    MachineState = MachineState.Payment;
                    _orderItem.Off();
                    _paymentReceiver.On();
                }
            }
            else if (cmd == OrderEvent.OutOfStock)
            {
                DisplayMessageByCode(MessageCode.OutOfStock);
                _orderItem.Off();
                MachineState = MachineState.OutOfStock;
            }
        }

        private void DisplayMessageByCode(MessageCode messageCode)
        {
            _display.DisplayMessage(_MessageRepository.GetMessage(messageCode));
        }
        public static PaymentPurseRepo Composer(IPurseRepo purseRepo, StockPrice changeValue)
        {
            PaymentPurseRepo retval = new PaymentPurseRepo();

            foreach (var position in purseRepo.ItemList.OrderByDescending(w => w.Item1.Nominal).Where(p => p.Item2 > 0))
            {
                (INotionalValue val, int count) = position;
                while (changeValue >= val.StockPriceValue && count > 0)
                {
                    count--;
                    changeValue -= val.StockPriceValue;
                    retval.AddItem(val, 1);
                    purseRepo.RemoveItem(val, 1);
                }
                if (changeValue == StockPrice.Zero)
                {
                    break;
                }
            }

            //Cannot get the change
            if (changeValue != StockPrice.Zero)
            {
                foreach (var mc in retval.ItemList)
                {
                    purseRepo.AddItem(mc.Item1, mc.Item2);
                }
                return null;
            }
            return retval;
        }
    }
}
