using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using gameManager;
using Shop;

namespace player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Starting RPMs and the value that it will return to")]
        private int BaseRpms;

        [SerializeField]
        [Tooltip("Max RPM value that the player can achieve")]
        public int RpmMax;

        [SerializeField]
        [Tooltip("Min RPM value that the player can achieve")]
        public int RpmMin;

        [SerializeField]
        [Tooltip("The rate at which RPMs returns to Base Rpm value")]
        private float RpmRestoreRate;

        [SerializeField]
        [Tooltip("starting Fuel value")]
        private float BaseFuel;
        
        [SerializeField]
        private float MinFuelRefreshTime;
        
        [SerializeField]
        private float MaxFuelRefreshTime;

        [SerializeField]
        [Tooltip("adjust how much the fuel rate changes at each RPM value")]
        private float FuelModifier;

        [SerializeField]
        private float FuelConsumptionRate;

        private bool isLaunching;
        private bool isFalling;
        private bool isDrilling;

        public float currentRpms { get; private set; }
		public float currentFuel { get; private set; }

        private float FuelRefreshReset = 0f;

		//private GameObject dirtParticleSystem;
        // Use this for initialization
        void Awake()
        {
            currentFuel = BaseFuel;
            currentRpms = BaseRpms;
            isLaunching = false;
            isFalling = false;
            isDrilling = false;

            //FuelDecrementTemp = 0.0f;
            //GameManager.instance.soundEffects.PlayMusic(1);
        }

		public void Start()
		{
			//dirtParticleSystem = GameObject.Find("Particle System");
			//dirtParticleSystem.SetActive(false);
			BaseFuel = PlayerPrefs.GetInt(ShopManager.VALUE_PREFIX + ShopItem.FuelCapacity, 10);
		}

        // Update is called once per frame
        void FixedUpdate()
        {

            //Return RPMs to normal
            //only allow RPM changes if we are launching or drilling
            if(isLaunching || isDrilling)
            if (Mathf.RoundToInt(currentRpms) != RpmMin)
            {

                if (currentRpms > RpmMin)
                    UpdateRPM(-RpmRestoreRate);
            }

            FuelRefreshReset += Time.deltaTime;

            float temp = (currentRpms-RpmMin) / (RpmMax - RpmMin) ;
            FuelConsumptionRate = (1.0f - temp);
            FuelConsumptionRate = FuelConsumptionRate * (MaxFuelRefreshTime - MinFuelRefreshTime) + MinFuelRefreshTime;

            //decrease fuel if we are in drilling state
            if (isDrilling)
            {
                if (FuelRefreshReset > FuelConsumptionRate)
                {
                    UpdateFuel(-1);

                    FuelRefreshReset = 0.0f;
                }

                //handle case where player runs out of fuel;
                if(currentFuel <= 0)
                {
                    isLaunching = false;
                    isFalling = false;
                    isDrilling = false;

                    GameManager.instance.HandleGameOver(GameOverState.OutOfFuel);

                }
            }


        }

        void Update()
        {

        }

        public int GetBaseRPMs()
        {
            return BaseRpms;
        }

        public float GetCurrentRPMs()
        {
            return currentRpms;
        }

        public int GetRpmMin()
        {
            return RpmMin;
        }

        public int GetRpmMax()
        {
            return RpmMax;
        }

        public void UpdateRPM(float RpmIncrease)
        {
            currentRpms += RpmIncrease;
            if (currentRpms > RpmMax)
            {
                currentRpms = RpmMax;
            }
            if (currentRpms < RpmMin)
            {
                currentRpms = RpmMin;
            }

            GameManager.instance.UpdateRpmText(Mathf.RoundToInt(currentRpms).ToString());
            GameManager.instance.UpdateRpmSlider(currentRpms);
        }

        public float GetBaseFuel()
        {
            return BaseFuel;
        }

        public float GetCurrentFuel()
        {
            return currentFuel;
        }

        public void UpdateFuel(float FuelIncrease)
        {
            //Debug.Log("Fuel Change: " + FuelIncrease);

            currentFuel += FuelIncrease;
            if(currentFuel > BaseFuel)
            {
                currentFuel = BaseFuel;
            }
            GameManager.instance.UpdateFuelText(Mathf.RoundToInt(currentFuel).ToString());
            GameManager.instance.SetFuelUI(currentFuel);
        }

        public void SetIsLaunching()
        {
            isLaunching = true;
            isFalling = false;
            isDrilling = false;
        }

        public void SetIsFalling()
        {
            isLaunching = false;
            isFalling = true;
            isDrilling = false;
        }

        public void SetIsDrilling()
        {
            isLaunching = false;
            isFalling = false;
            isDrilling = true;
        }

		//public void OnCollisionEnter2D(Collider2D collider)
		//{
		//	Debug.Log(collider.gameObject.name);
		//	if(collider.gameObject.name == "StartingStrata")
		//	{
		//		Debug.Log("set particle syste actiuve");
		//		dirtParticleSystem.SetActive(true);
		//	}
		//}
    }
}
