using UnityEngine;
using System.Collections;
using player;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using gameManager;

namespace touchPad
{
    public class RevTouchPad : TouchPad
    {
        [SerializeField]
        [Tooltip("Affects how much each swipe increase RPMs")]
        private float RpmModifier;

        [SerializeField]
        [Tooltip("connect to player manager here")]
        private PlayerManager playerManager;


        public override void OnPointerUp(PointerEventData data)
        {
            
            //reset everything
            if (data.pointerId == pointerID && (GameManager.instance.GetLaunchingState() || GameManager.instance.GetDrillingState()))
            {
                playerManager.UpdateRPM( Mathf.Abs(direction.x * RpmModifier));
                direction = Vector3.zero;
                touched = false;
				GameManager.instance.RotateGear();
                GameManager.instance.soundEffects.PlayMusic(2);
            }
        }
    }
}
