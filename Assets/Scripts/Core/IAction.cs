using UnityEngine;

namespace RPG.Core
{
    //Interface for actions that can be scheduled.
    //Can only contain methods or properties
    public interface IAction
    {
        void Cancel();
    }
}
