using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Services
{
    public interface ITaskFilterStrategy
    {
        bool Filter(TaskItem task);
    }


    public class CompositeFilterStrategy : ITaskFilterStrategy
    {
        private readonly IEnumerable<ITaskFilterStrategy> _strategies;

        public CompositeFilterStrategy(IEnumerable<ITaskFilterStrategy> strategies)
        {
            _strategies = strategies;
        }

        public bool Filter(TaskItem task)
        {
            foreach (var strategy in _strategies)
            {
                if (!strategy.Filter(task)) return false;
            }
            return true;
        }
    }
}
