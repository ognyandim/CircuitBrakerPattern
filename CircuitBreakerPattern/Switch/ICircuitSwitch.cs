using System;

namespace CircuitBreakerPattern.Switch
{
    public interface ICircuitSwitch
    {
        ICircuitSwitch TryFirst(Action action);

        ICircuitSwitch Otherwise(Action action);
      
        void Execute();
    }
}