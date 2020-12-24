
//Interface for handling raycasts
namespace RPG.Control 
{ 
    public interface IRayCastable
        {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController playerCont);
        }

}
