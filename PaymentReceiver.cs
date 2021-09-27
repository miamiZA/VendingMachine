using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UFF.VendingMachine.Interface;
namespace UFF.VendingMachine
{
    public class PaymentReceiver : IPaymentReceiver
    {
        /// <summary>
        /// Gets or sets status flag.
        /// </summary>
        public bool StatusOn { get; set; }

        readonly IDisplay _display;
        readonly IReadInput _readInput;
        readonly IMessageRepo _messageRepository;
        CancellationTokenSource _cancellationTokenSource;
        CancellationToken _cancellationToken;
        Task _readTask;

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="display"></param>
        /// <param name="readInput"></param>
        /// <param name="messageRepository"></param>
        public PaymentReceiver(IDisplay display, IReadInput readInput, IMessageRepo messageRepository)
        {
            _messageRepository = messageRepository;
            _display = display;
            _readInput = readInput;
        }

        #region IPaymentReceiver interface

        public event PaymentAction paymentAction;
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
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;
                _readTask = Task.Run((Action)ReadPaymentInstructions, _cancellationToken);
            }
            catch (Exception e)
            {
                FailException?.Invoke(e);
            }
        }

        public List<Coin> AcceptedCoins => MapCoins.Values.ToList();

        public Dictionary<char, Coin> MapCoins { get; } = new Dictionary<char, Coin>()
        {
            {'1', new Coin(10)},
            {'2', new Coin(20)},
            {'3', new Coin(50)},
            {'4', new Coin(100)},
            {'5', new Coin(200)}
        };

        #endregion

        private string msgChoice = " 1 -[10c]; 2 -[20c]; 3 -[50c]; 4 -[$1]; 5 -[$2]";

        /// <summary>
        /// Read Payment Instruction.
        /// </summary>
        private void ReadPaymentInstructions()
        {
            try
            {
                do
                {
                    string template = _messageRepository.GetMessage(MessageCode.MakeYourPayment);
                    if (!string.IsNullOrWhiteSpace(template))
                    {
                        string msg = string.Format(template, msgChoice);
                        _display.DisplayMessage(msg);
                    }
                    // get the key code
                    char code = _readInput.Read(_cancellationToken);

                    if (code == '#')
                    {
                        // cancel purchase
                        DisplayMessageByCode(MessageCode.Ok);
                        paymentAction?.Invoke(PaymentEvent.Cancel, null);
                    }
                    else if (MapCoins.ContainsKey(code))
                    {
                        var coin = MapCoins[code];
                        DisplayMessageByCode(MessageCode.Ok);
                        // make payment
                        paymentAction?.Invoke(PaymentEvent.Payment, coin);
                    }
                    else
                    {
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

        private void DisplayMessageByCode(MessageCode messageCode)
        {
            _display.DisplayMessage(_messageRepository.GetMessage(messageCode));
        }

    }
}
