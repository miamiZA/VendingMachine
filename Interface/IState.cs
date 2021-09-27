using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UFF.VendingMachine.Interface
{
    public interface IState :ISwitch
    {
        /// <summary>
        /// Get the task of vending machine completion.
        /// </summary>
        Task Completed { get; }
    }
}
