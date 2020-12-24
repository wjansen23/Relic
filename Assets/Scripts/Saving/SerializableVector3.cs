using UnityEngine;

//This is required because unity doesn't naturally allow for serializable vectors
namespace RPG.Saving
{
    [System.Serializable]
    public class SerializableVector3 
    {
        //Variables
        float x, y, z;


        ///////////////////////////// PUBLIC METHODS ////////////////////////////////////////////     
        
        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector ()
        {
            return new Vector3(x, y, z);
        }

    }
}
