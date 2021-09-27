using System;
using System.Collections.Generic;
using System.Text;

namespace UFF.VendingMachine.Interface
{
    public interface IDisplay
    {
        /// <summary>
        /// Render a message a message 
        /// </summary>
        /// <param name="message">The message to display.</param>
        void DisplayMessage(string message);

    }
}
