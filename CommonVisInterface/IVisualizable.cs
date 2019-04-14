using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonVisInterface
{

    public delegate void VisualizeEventHandler<T>(IEnumerable<T> newPositions, List<List<int>> connections, string message);

    public interface IVisualizable<T>
    {
        event VisualizeEventHandler<T> OnUpdate;
    }
}
