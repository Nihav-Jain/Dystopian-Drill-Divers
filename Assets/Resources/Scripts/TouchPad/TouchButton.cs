using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using player;

namespace touchPad
{

    public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        protected bool touched;
        protected int pointerID;

        void Awake()
        {
            touched = false;
        }

        public virtual void OnPointerDown(PointerEventData data)
        {
            //set our start point
            if (!touched)
            {
                touched = true;
                pointerID = data.pointerId;

            }

        }

        public virtual void OnPointerUp(PointerEventData data)
        {
            //reset everything
            if (data.pointerId == pointerID)
            {
                touched = false;
            }
        }

        public virtual bool IsTouched()
        {
            return touched;
        }

    }
}