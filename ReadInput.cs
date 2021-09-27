using System.Threading;
using UFF.VendingMachine.Interface;

namespace UFF.VendingMachine
{
    public class ReadInput : IReadInput
    {

        /// <summary>
        /// Reads string from input device
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The text string.</returns>
        public char Read(CancellationToken cancellationToken)
        {
            do
            {
                if (!System.Console.KeyAvailable)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    var key = System.Console.ReadKey();
                    char cki = key.KeyChar;
                    if (char.IsLetterOrDigit(cki) || cki == '#' || cki == '*')
                    {
                        return char.ToUpper(cki);
                    }
                }

            } while (!cancellationToken.IsCancellationRequested);

            return (char)0;
        }
    }
}
