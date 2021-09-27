using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UFF.VendingMachine.Interface;

namespace UFF.VendingMachine
{
    public class OrderItem : IOrderItem
    {
        /// <summary>
        /// Gets or sets status flag.
        /// </summary>
        public bool StatusOn { get; set; }

        private readonly IDisplay _display;
        private readonly IReadInput _readInput;
        readonly IMessageRepo _MessageRepository;
        readonly IStockItemRepo _StockItemRepository;
        CancellationTokenSource _cancellationTokenSource;
        CancellationToken _cancellationToken;
        Task _readTask = null;

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="displayPanel"></param>
        /// <param name="readKeypadInput"></param>
        /// <param name="productRepository"></param>
        /// <param name="vendingMessageRepository"></param>
        public OrderItem(IDisplay display, IReadInput readInput,IStockItemRepo stockItemRepository, IMessageRepo messageRepository)
        {
            _MessageRepository = messageRepository;
            _display = display;
            _StockItemRepository = stockItemRepository;
            _readInput = readInput;
        }

        #region IOrderPanel interface

        public event OrderAction OrderAction;

        public event FailException FailException;

        public void Off()
        {
            _cancellationTokenSource?.Cancel();
            StatusOn = false;
            _readTask = null;
        }

        public void On()
        {
            try
            {
                StatusOn = true;
                // verify stock 
                if (StockAvaliable())
                {
                    // Activate key pad reader
                    _cancellationTokenSource = new CancellationTokenSource();
                    _cancellationToken = _cancellationTokenSource.Token;
                    _readTask = Task.Run((Action)ReadInstructions, _cancellationToken);
                }
                else
                {
                    // signal nothing to sell
                    OrderAction?.Invoke(OrderEvent.OutOfStock, null);
                    StatusOn = false;
                }
            }
            catch (Exception e)
            {
                FailException?.Invoke(e);
            }
        }

        #endregion
        private bool StockAvaliable()
        {
            return _StockItemRepository.StockItemList.Any((p) => _StockItemRepository.StockCount(p.Key) > 0);
        }

        /// <summary>
        /// Display the stock available to sell.
        /// </summary>
        private void RenderAvailableStock()
        {
            DisplayMessageByCode(MessageCode.MakeYourOrder);
            var list = _StockItemRepository.StockItemList;
            foreach (var item in list)
            {
                if (_StockItemRepository.StockCount(item.Key) > 0)
                {
                    string template = _MessageRepository.GetMessage(MessageCode.SelectOrderLine);
                    if (!string.IsNullOrWhiteSpace(template))
                    {
                        string msg = string.Format(template, item.Key, item.DisplayName, item.Price);
                        _display.DisplayMessage(msg);
                    }
                }
            }
        }
        private void ReadInstructions()
        {
            try
            {
                do
                {
                    // verify stock 
                    if (StockAvaliable())
                    {
                        RenderAvailableStock();
                    }
                    else
                    {
                        // Signal nothing to sell
                        OrderAction?.Invoke(OrderEvent.OutOfStock, null);
                        StatusOn = false;
                        continue;
                    }

                    DisplayMessageByCode(MessageCode.SelectOrder);

                    char code = _readInput.Read(_cancellationToken);

                    // check the product code
                    if (_StockItemRepository.StockItemList.Any((p) => p.Key == code.ToString()))
                    {
                        // valid code
                        var product = _StockItemRepository.StockItemList.First((p) => p.Key == code.ToString());
                        DisplayMessageByCode(MessageCode.Ok);
                        // select it
                        OrderAction?.Invoke(OrderEvent.Select, product);
                    }
                    else
                    {
                        // inform - invalid code
                        DisplayMessageByCode(MessageCode.InvalidInput);
                    }
                }
                while (StatusOn);
            }
            catch (Exception e)
            {
                FailException?.Invoke(e);
            }
        }

        /// <summary>
        /// Display message from message repository.
        /// </summary>
        /// <param name="messageCode">THe message code.</param>
        private void DisplayMessageByCode(MessageCode messageCode)
        {
            _display.DisplayMessage(_MessageRepository.GetMessage(messageCode));
        }

    }
}
