using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace UFF.VendingMachine.Interface
{
    public interface IReadInput
    {
       /// <summary>
       /// Read Input
       /// </summary>
       /// <param name="cancellationToken"></param>
       /// <returns></returns>
        char Read(CancellationToken cancellationToken);
    }
}
