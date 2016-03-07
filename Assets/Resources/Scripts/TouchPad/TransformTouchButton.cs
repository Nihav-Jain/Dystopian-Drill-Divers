using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using player;
using gameManager;

namespace touchPad
{

    public class TransformTouchButton: TouchButton
    {

        void Awake()
        {
            touched = false;
        }

        public override void OnPointerDown(PointerEventData data)
        {
            //set our start point
            if (!touched)
            {
                touched = true;
                pointerID = data.pointerId;
                GameManager.instance.StartDrilling();

            }

        }



    }
}