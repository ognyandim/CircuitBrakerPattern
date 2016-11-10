using System;

namespace CircuitBreakerPattern.Switch
{
    public interface ICircuitSwitch
    {
        /// <summary>
        /// Action to attempt and fail silently
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ICircuitSwitch Attempt(Action action);

        /// <summary>
        /// Action to attempt if the first action is not executable at the moment
        /// </summary>
        /// <param name="action"></param>
        ICircuitSwitch Otherwise(Action action);

        /// <summary>
        /// Start execution
        /// </summary>
        void Execute();
    }
}