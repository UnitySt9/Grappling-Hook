using UnityEngine;

namespace _Project.Scripts
{
    public class RopeSegment
    {
        public Vector3 currentPos;
        public Vector3 previousPos;

        public RopeSegment(Vector3 pos)
        {
            currentPos = pos;
            previousPos = pos;
        }
    }
}
