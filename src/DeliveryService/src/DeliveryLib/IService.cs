using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryLib
{
    public interface IService
    {
        void Run(CancellationToken token);
    }
}
