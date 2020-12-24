

namespace RPG.Saving
{
    //Interface for saving.
    //Can only contain methods or properties
    public interface ISaveable
    {
        object CaptureState();
        void RestoreState(object state);
    }
}