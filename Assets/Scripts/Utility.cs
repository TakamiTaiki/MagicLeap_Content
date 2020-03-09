using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Utility
    {
        public static void ChangeObjColor(GameObject obj, Color color)
        {
            ChangeObjColor(obj.GetComponent<Renderer>(), color);
        }

        public static void ChangeObjColor(Renderer rend, Color color)
        {
            rend.material.color = color;
        }

            public static void Alignment(GameObject from, GameObject to)
        {
            Alignment(from.transform, to.transform);
        }

        public static void Alignment(Transform from, Transform to)
        {
            from.position = to.position;
        }

        public static void SetStage(int era, GameObject[] castles, GameObject[] planes)
        {
            if (era == 1585)
            {

            }
            else if (era == 1615)
            {

            }
        }
    }

}

