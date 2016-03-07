using UnityEngine;
using System.Collections;

namespace gameManager
{
    public class LaunchCountDown : MonoBehaviour
    {

        public int startingValue;
        public float timeBetweenUpdates;


        private bool runTimer = false;
        private double timer;
        private int currentValue;

        // Use this for initialization
        void Awake()
        {
            timer = 0.0;
            //runTimer = false;
            currentValue = startingValue;

        }

        // Update is called once per frame
        void Update()
        {

            if (runTimer)
            {
                timer += Time.deltaTime;

                if (timer >= timeBetweenUpdates)
                {
                    currentValue--;
                    
                    timer = 0.0;

                    if (currentValue == 0)
                    {
                        GameManager.instance.UpdateCountdownText("FIRE!");
                    }
                    else if (currentValue < 0)
                    {
                        GameManager.instance.StartFalling();
                        runTimer = false;
                        
                    } else
                    {
                        GameManager.instance.UpdateCountdownText(currentValue.ToString());
                    }
                }

            }
        }

        public void startCountDown()
        {
            runTimer = true;
            GameManager.instance.UpdateCountdownText(currentValue.ToString());

        }
    }
}