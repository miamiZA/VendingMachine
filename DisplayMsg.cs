using UFF.VendingMachine.Interface;

namespace UFF.VendingMachine
{
    public class DisplayMsg : IDisplay
    {
        public void DisplayMessage(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
