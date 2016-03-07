using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace touchPad
{
    public class TouchPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField]
        [Range(0,1)]
        private float smoothing;

        protected Vector2 origin;
        protected Vector2 direction;
        protected Vector2 smoothDirection;
        protected bool touched;
        protected int pointerID;

        void Awake()
        {
            touched = false;
            direction = Vector2.zero;
        }


        public void OnPointerDown(PointerEventData data)
        {
            //set our start point
            if (!touched)
            {
                touched = true;
                pointerID = data.pointerId;
                origin = data.position;
            }

        }

        public void OnDrag(PointerEventData data)
        {
            //compare the difference between start point and current pointer position
            if (data.pointerId == pointerID)
            {
                Vector2 currentPosition = data.position;
                Vector2 directionRaw = currentPosition - origin;
                direction = directionRaw.normalized;
            }

        }

        public virtual void OnPointerUp(PointerEventData data)
        {
            //reset everything
            if (data.pointerId == pointerID)
            {
                direction = Vector3.zero;
                touched = false;
            }
        }

        public Vector2 GetDirection()
        {
            smoothDirection = Vector2.MoveTowards(smoothDirection, direction, smoothing);
            return smoothDirection;
        }
    }

}