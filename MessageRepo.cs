using System.Collections.Generic;
using UFF.VendingMachine.Interface;

namespace UFF.VendingMachine
{
    public class MessageRepo : IMessageRepo
    {
        static Dictionary<MessageCode, string> _msgRepository = new Dictionary<MessageCode, string>()
        {
            [MessageCode.OutOfServise] = "Machine Status: Out of Service",
            [MessageCode.OutOfStock] = "Machine Status: Out of Stock",
            [MessageCode.ReadyToService] = "Machine Status: Ready",

            [MessageCode.MakeYourOrder] = "\r\nPlease Make your order",
            [MessageCode.SelectOrderLine] = "Code: {0}      Item: {1}                       Price: ${2}",
            [MessageCode.SelectOrder] = "Enter 'Code' to order :",

            [MessageCode.Checkout] = "\r\nCheck out: {0} ",

            [MessageCode.RunOutOfChange] = "Error! The change required is not available",
            [MessageCode.OrderCancel] = "Order Canceled",
            [MessageCode.MakeYourPayment] = "Please Enter {0} to make payment or  '#' to Cancel;",
            [MessageCode.BalancePayment] = "Total: {0} Paid: {1} ",

            [MessageCode.CollectYourPurchase] = "\r\nCollect your order: {0}",

            [MessageCode.ReturnPayment] = "Return Payment:  {0} x {1}",
            [MessageCode.GivenChange] = "Change:  {0} x {1}",

            [MessageCode.InvalidInput] = " Incorrect choice \r\n",
            [MessageCode.Ok] = " Ok \r\n",
        };

        #region IMessageRepo interface
        public string GetMessage(MessageCode code)
        {
            if (_msgRepository.ContainsKey(code))
            {
                return _msgRepository[code];
            }
            return string.Empty;
        }
        #endregion
    }
}
