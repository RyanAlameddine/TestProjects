using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonVisInterface
{
    public interface IRunnable<T>
    {
        IVisualizable<T> Visualizable { get; }
        Task Run(CancellationToken cancellation);
    }
}
