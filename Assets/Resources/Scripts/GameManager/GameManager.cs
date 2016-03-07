using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using player;
using Environment;
using cameraControl;
using enumerations;
using Shop;

namespace gameManager
{
	public enum GameOverState
	{
		EndOfLevel,
		OutOfFuel,
		Crashed
	}

    public class GameManager : MonoBehaviour
    {
        public SoundManager music;
        public SoundManager soundEffects;
        public Canvas PlayerCanvas;

        public Slider m_Slider;
        public Image m_FillImage;
        public Color m_FullFuelColor = Color.green;
        public Color m_ZeroFuelColor = Color.red;

        public Slider RPMSlider;

        public bool StartSpawners;

        [SerializeField]
        [Tooltip("Place UI Text object that corresponds to the RPMs")]
        private Text RpmText;

        [SerializeField]
        [Tooltip("Place UI Text object that corresponds to the Fuel")]
        private Text FuelText;

        //public Text ObstacleHealthText;

        public Text CountdownText;
        public Text CoutdownInstructionText;
        public Text DeployText;
        public Image TransformButton;

        [SerializeField]
        [Tooltip("Place Game Object with playerManager component here")]
        private PlayerManager playerManager;

        public static GameManager instance = null;
		public static PlayerController playerController;

        public CameraController mainCameraController;

        private bool LaunchingState;
        private bool FallingState;
        private bool DrillingState;
        private bool ResultsState;

		public bool isInEarth = false;
		GameObject dirtParticleSystem;

		[SerializeField]
		private Text altimeter;

        public Text CurrencyText;
        private int currentCurrency;

        public Image PauseMenu;
        public Image ResultsMenu;

		private GameObject earthLevels;
		public GameObject player;
		private bool isHeaterBought;
		private bool isHeaterActive;

		[SerializeField]
		private GameObject heatedRockReplacement;
		[SerializeField]
		private Button heaterButton;
        [SerializeField]
        private float heaterRadius;
		[SerializeField]
		private float heaterDuration;
		[SerializeField]
		private float phaserDuration;
		[SerializeField]
		private Button phaserButton;

        private bool gamePaused;

		private GameObject gear;
		private bool rotategear;

		private bool isPhaserActive;
		private bool isPhaserBought;

        private Transform Glow;
        float scaleProg = 0.0f;
        bool GlowIncrease = true;

        [SerializeField]
		private Dictionary<GameOverState, string> gameOverStrings = new Dictionary<GameOverState, string>()
		{
			{GameOverState.Crashed, "CRASHED!!!"},
			{GameOverState.EndOfLevel, "You Reached The Core!!"},
			{GameOverState.OutOfFuel, "You ran out of fuel!"}
		};

		public int MaxLevelNumber = 10;
		public int LevelNumber = 1;
		public float fuelSpwnProbability = 0.1f;
		public float fuelDangerLevel = 2.0f;

		private List<GameObject> obstaclesHitByPhaser;
		private GameObject phaserLineRendererGO;
		private LineRenderer phaserLineRenderer;
		private Material phaserMaterial;
		private float phaserLength = 6f;

		void Awake()
        {
			Time.timeScale = 1;
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        void Start()
        {
			player = GameObject.Find("Player");
			playerController = player.GetComponent<PlayerController>();
			dirtParticleSystem = GameObject.Find("Dirt");
			dirtParticleSystem.SetActive(false);
			earthLevels = GameObject.Find("EarthLevels");
			gear = GameObject.Find("gear");

			int heaterBoughtStatus = PlayerPrefs.GetInt(ShopManager.BOUGHT_PREFIX + ShopItem.OverHeat, 0);
			isHeaterBought = (heaterBoughtStatus > 0) ? true : false;
			heaterButton.gameObject.SetActive(isHeaterBought);
			isHeaterActive = false;

			int phaserBoughtStatus = PlayerPrefs.GetInt(ShopManager.BOUGHT_PREFIX + ShopItem.Phasers, 0);
			isPhaserBought = (phaserBoughtStatus > 0) ? true : false;
			phaserButton.gameObject.SetActive(isPhaserBought);
			isPhaserActive = false;

			obstaclesHitByPhaser = new List<GameObject>();
			phaserLineRendererGO = GameObject.Find("GO_LineRenderer");
			phaserLineRenderer = phaserLineRendererGO.GetComponent<LineRenderer>();
			phaserMaterial = phaserLineRenderer.material;

			Vector3 linePos = Vector3.zero;
			linePos.z = -5;
			phaserLineRenderer.SetPosition(0, linePos);
			phaserLineRenderer.SetPosition(1, linePos + Vector3.down * phaserLength);
			phaserLineRendererGO.SetActive(false);

            Glow = player.transform.FindChild("HeatPulse");

            music.PlayMusic(0);

			gamePaused = false;
			rotategear = false;

			float x, y, z;
			x = PlayerPrefs.GetFloat(GameConstants.DeadBotPositionPref + "X", -1000);
			if(x > -1000)
			{
				y = PlayerPrefs.GetFloat(GameConstants.DeadBotPositionPref + "Y", 0);
				z = PlayerPrefs.GetFloat(GameConstants.DeadBotPositionPref + "Z", 0);

				GameObject deadbot = GameObject.Instantiate(Resources.Load("Prefabs/deadbot", typeof(GameObject))) as GameObject;
				deadbot.name = "DeadBot";
				deadbot.transform.position = new Vector3(x, y, z);
				deadbot.transform.parent = earthLevels.transform;
				Debug.Log(string.Format("deadbot at {0}, {1}, {2}", x, y, z));
			}

			Setup();
		}

        public void Setup()
        {
            UpdateRpmText(playerManager.GetBaseRPMs().ToString());
            UpdateFuelText(playerManager.GetBaseFuel().ToString());
            m_Slider.maxValue = playerManager.GetBaseFuel();
            SetFuelUI(playerManager.GetBaseFuel());
            LaunchingState = false;
            FallingState = false;
            TransformButton.enabled = false;
            DeployText.enabled = false;
            DrillingState = false;

			StartSpawners = false;

            currentCurrency = PlayerPrefs.GetInt("Currency", 3000);
            CurrencyText.text = ": " + currentCurrency;


            RPMSlider.minValue = playerManager.GetRpmMin();
            RPMSlider.maxValue = playerManager.GetRpmMax();
            RPMSlider.value = playerManager.GetCurrentRPMs();


            StartLaunchCountdown();

        }

//Game State Control Functions//////////////////////////////////////////////////////////
        void StartLaunchCountdown()
        {
			playerController.CanPlayerMove = false;
			gameObject.GetComponent<EnvironmentBuilder>().Scroll = false;
            LaunchingState = true;
            gameObject.GetComponent<LaunchCountDown>().startCountDown();
            playerManager.SetIsLaunching();
        }

        public void StartFalling()
        {
            //this is what will happen after the countdown launch finishes
            LaunchingState = false;
            CoutdownInstructionText.enabled = false;
            CountdownText.enabled = false;

            FallingState = true;
            soundEffects.PlayMusic(5);
            
           
            playerManager.SetIsFalling();
            playerController.CanPlayerMove = true;
            playerController.CanMovePlayerOffScreen = true;
            //playerController.skyScrollSpeed;
            mainCameraController.StartZoom = true;
            TransformButton.enabled = false;
            DeployText.enabled = true;
			gameObject.GetComponent<EnvironmentBuilder>().Scroll = true;
            
        }

        public void StartDrilling()
        {
			player.GetComponent<Animator>().SetBool("isTransforming", true);
			playerController.CanPlayerMove = true;
            FallingState = false;
            TransformButton.enabled = false;
            DeployText.enabled = false;

            soundEffects.PlayMusic(4);

            DrillingState = true;
            
            playerManager.SetIsDrilling();
            
            PlayerCanvas.GetComponent<Canvas>().enabled = true;           
            
            
        }

		private void Update()
		{
			if(!isInEarth && gameObject.GetComponent<EnvironmentBuilder>().isInEarth)
			{
				if(DrillingState == false)
				{
					gameObject.GetComponent<EnvironmentBuilder>().Scroll = false;
                    HandleGameOver(GameOverState.Crashed);
				}
				isInEarth = true;
				StartSpawners = true;
				dirtParticleSystem.SetActive(true);
                soundEffects.PlayMusic(1);
                soundEffects.PlayMusic(3);

                music.StopMusic();
                music.PlayMusic(1);
                soundEffects.PlayMusic(5);

            }
			altimeter.text = gameObject.GetComponent<EnvironmentBuilder>().getAltitude().ToString("0") + " m";

			if(isHeaterActive)
			{
				Vector2 circleCenter = new Vector2(player.transform.position.x, player.transform.position.y);
				Collider2D[] nearByColliders = Physics2D.OverlapCircleAll(circleCenter, heaterRadius);
				foreach(Collider2D collider in nearByColliders)
				{
					if(collider.gameObject.name.IndexOf("Fuel") >= 0)
					{
						GameObject explosion = GameObject.Instantiate(Resources.Load("Prefabs/explosion", typeof(GameObject))) as GameObject;
						explosion.transform.position = collider.transform.position;
						explosion.transform.SetParent(earthLevels.transform);
						GameObject.Destroy(collider.gameObject);
						Debug.Log("fuel destroyed");
					}
					else if(collider.gameObject.name.IndexOf("Rock") >= 0)
					{
						Vector3 position = collider.gameObject.transform.position;
						GameObject.Destroy(collider.gameObject);
						GameObject diamond = Instantiate(heatedRockReplacement, position, Quaternion.identity) as GameObject;
						diamond.transform.SetParent(earthLevels.transform);
					}
				}
			}


			if (isPhaserActive)
			{
				Vector2 lineRenderedOffset = phaserMaterial.GetTextureOffset("_MainTex");
				lineRenderedOffset.x -= 0.025f;
				phaserMaterial.SetTextureOffset("_MainTex", lineRenderedOffset);
				Vector3 rayOrigin = player.transform.position;
				//rayOrigin.z = 0;
				Vector3 dir = Quaternion.Euler(0, 0, player.transform.localEulerAngles.z) * Vector3.down;
				//dir.z = 0;
				Debug.DrawRay(rayOrigin, dir, Color.white);

				for (int i = 0; i < obstaclesHitByPhaser.Count; i++)
				{
					if (obstaclesHitByPhaser[i] == null)
						obstaclesHitByPhaser.RemoveAt(i);
					else
						obstaclesHitByPhaser[i].GetComponent<DestroyByContact>().isAttakedByPhaser = false;
				}
				RaycastHit2D[] hitByPhaser = Physics2D.RaycastAll(new Vector2(player.transform.position.x, player.transform.position.y), new Vector2(dir.x, dir.y), 25);
				foreach (RaycastHit2D rayCastHit in hitByPhaser)
				{
					if (rayCastHit.collider.gameObject.name.IndexOf("Fuel") >= 0)
					{
						GameObject explosion = GameObject.Instantiate(Resources.Load("Prefabs/explosion", typeof(GameObject))) as GameObject;
						explosion.transform.position = rayCastHit.collider.gameObject.transform.position;
						explosion.transform.SetParent(earthLevels.transform);
						GameObject.Destroy(rayCastHit.collider.gameObject);
					}
					else if (rayCastHit.collider.gameObject.name.IndexOf("Rock") >= 0)
					{
						if (obstaclesHitByPhaser.IndexOf(rayCastHit.collider.gameObject) < 0)
						{
							obstaclesHitByPhaser.Add(rayCastHit.collider.gameObject);
						}
						rayCastHit.collider.gameObject.GetComponent<DestroyByContact>().isAttakedByPhaser = true;
					}
					else if (rayCastHit.collider.gameObject.name.IndexOf("mine") >= 0)
					{
						if (obstaclesHitByPhaser.IndexOf(rayCastHit.collider.gameObject) < 0)
						{
							obstaclesHitByPhaser.Add(rayCastHit.collider.gameObject);
						}
						rayCastHit.collider.gameObject.GetComponent<DestroyByContact>().isAttakedByPhaser = true;
					}
				}
			}

            if (isHeaterActive)
            {               
                

                if (Glow.localScale.x <= .15f && GlowIncrease == true)
                {
                    scaleProg = (scaleProg + 0.03f) % 1.0f;
                    Glow.localScale = new Vector3(Mathf.Lerp(.11f, .16f, scaleProg), Mathf.Lerp(.11f, .16f, scaleProg), 0);

                    if (Glow.localScale.x > .15f)
                    {
                        GlowIncrease = false;
                    }
                }
                else if (GlowIncrease == false)
                {
                    scaleProg = (scaleProg - 0.03f) % 1.0f;
                    Glow.localScale = new Vector3(Mathf.Lerp(.11f, .16f, scaleProg), Mathf.Lerp(.11f, .16f, scaleProg), 0);

                    if (Glow.localScale.x < .12f)
                    {
                        GlowIncrease = true;
                    }

                }
                

                

            }
		}
        
		public void HandleGameOver(GameOverState results)
        {
            //code to handle game over state goes here
            playerController.CanPlayerMove = false;
            LaunchingState = false;
            FallingState = false;
            DrillingState = false;
            ResultsState = true;
            gameObject.GetComponent<EnvironmentBuilder>().Scroll = false;

            ResultsMenu.gameObject.SetActive(true);

            Text resultsText = ResultsMenu.transform.GetChild(2).GetComponent<Text>();
            Text crystalText = ResultsMenu.transform.GetChild(3).GetComponent<Text>();
            Text depthText = ResultsMenu.transform.GetChild(4).GetComponent<Text>();

			resultsText.text = gameOverStrings[results];
            crystalText.text = "Diamonds: \n\n" + currentCurrency.ToString();
            depthText.text = "Depth: \n\n" + gameObject.GetComponent<EnvironmentBuilder>().getAltitude().ToString("0") + " m";

			if(phaserButton.IsActive())
				phaserButton.interactable = false;
			if(heaterButton.IsActive())
				heaterButton.interactable = false;
			//GameObject.Find("PauseButton").GetComponent<Button>().interactable = false;
			//GameObject.Find("HeaterButton").

			if(results == GameOverState.OutOfFuel || results == GameOverState.EndOfLevel)
			{
				WriteCurrencyToPlayerPrefs();
				Time.timeScale = 0;
			}

			if(results == GameOverState.OutOfFuel)
			{
				PlayerPrefs.SetFloat(GameConstants.DeadBotPositionPref + "X", player.transform.position.x + earthLevels.transform.position.x);
				PlayerPrefs.SetFloat(GameConstants.DeadBotPositionPref + "Y", player.transform.position.y - earthLevels.transform.position.y);
				PlayerPrefs.SetFloat(GameConstants.DeadBotPositionPref + "Z", player.transform.position.z + earthLevels.transform.position.z);
				PlayerPrefs.Save();

				Debug.Log("Deadbot saved at " + player.transform.position);
			}
			else
			{
				PlayerPrefs.DeleteKey(GameConstants.DeadBotPositionPref + "X");
				PlayerPrefs.DeleteKey(GameConstants.DeadBotPositionPref + "Y");
				PlayerPrefs.DeleteKey(GameConstants.DeadBotPositionPref + "Z");
				PlayerPrefs.Save();
			}

			if(results == GameOverState.Crashed)
			{
				GameObject explosion = GameObject.Instantiate(Resources.Load("Prefabs/explosion", typeof(GameObject))) as GameObject;
				explosion.transform.position = player.transform.position + Vector3.back * 5;
				explosion.transform.SetParent(earthLevels.transform);
				player.SetActive(false);
			}
        }

        void RestartStage()
        {
            WriteCurrencyToPlayerPrefs();
            Application.LoadLevel(Application.loadedLevel);

        }

        void ReturnToMainMenu(bool saveCurrency)
        {
            if (saveCurrency)
                WriteCurrencyToPlayerPrefs();
            Application.LoadLevel((int)ScenesEnum.Shop);
        }

//Menu Button Functions////////////////////////////////////////////////////////////
        public void Pause()
        {
			gamePaused = !gamePaused;
			PauseMenu.gameObject.SetActive(gamePaused);
			
			Time.timeScale = (gamePaused) ? 0 : 1;
        }

        public void RecalibrateAccelerometer()
        {
            playerController.CalibrateAccellerometer();
        }
        

        public void Quit()
        {
            ReturnToMainMenu(false);
        }

        public void ToMainMenu()
        {
            ReturnToMainMenu(true);
        }

        public void Retry()
        {
            RestartStage();
        }


        //UI Update functions////////////////////////////////////////////////////////////
        public void UpdateRpmText(string rpmValue)
        {
            RpmText.text = "RPM: " + rpmValue;
        }

        public void UpdateRpmSlider(float rpmValue)
        {
            RPMSlider.value = rpmValue;
        }


        public void UpdateFuelText(string fuelValue)
        {
            FuelText.text = "Fuel: " + fuelValue;
        }

        public void SetFuelUI(float amount)
        {
            m_Slider.value = amount;

            m_FillImage.color = Color.Lerp(m_ZeroFuelColor, m_FullFuelColor, amount / playerManager.GetBaseFuel());
        }


        //public void SetActiveObstacleHealth(bool state)
        //{
        //    ObstacleHealthText.enabled = state;
        //}

        //public void UpdateObstacleHealthText(string healthValue)
        //{
        //    ObstacleHealthText.text = healthValue;
        //}

        public void UpdateCountdownText(string value)
        {
            CountdownText.text = value;
        }

        public void UpdateCurrency(int amount)
        {
            currentCurrency += amount;
            CurrencyText.text = ": " + currentCurrency;
        }

        //When level is finished write currency back to player prefs
        void WriteCurrencyToPlayerPrefs()
        {
            PlayerPrefs.SetInt("Currency", currentCurrency);
            PlayerPrefs.Save();
        }

//Game state accessor functions///////////////////////////////////////////////////////////
        public bool GetLaunchingState()
        {
            return LaunchingState;
        }

        public bool GetFallingState()
        {
            return FallingState;
        }

        public bool GetDrillingState()
        {
            return DrillingState;
        }


		public void activateHeater()
		{
			isHeaterActive = true;
            player.transform.FindChild("HeatPulse").GetComponent<SpriteRenderer>().enabled = true;
			heaterButton.interactable = false;
			Invoke("deactivateHeater", heaterDuration);
		}

		private void deactivateHeater()
		{
			isHeaterActive = false;
			player.transform.FindChild("HeatPulse").GetComponent<SpriteRenderer>().enabled = false;
		}

		private float angle = 0;
		public void RotateGear()
		{
			if(LaunchingState)
			{
                gear.GetComponent<Rigidbody2D>().AddTorque(-1000);
			}
		}

		public void stopGearRotation()
		{
			rotategear = false;
		}

		public bool isFuelLow()
		{
			return (player.GetComponent<PlayerManager>().currentFuel <= fuelDangerLevel);
		}

		public void activatePhaser()
		{
			isPhaserActive = true;
			phaserLineRendererGO.SetActive(isPhaserActive);
			phaserButton.interactable = false;

			if(!isPhaserActive)
			{
				for (int i = 0; i < obstaclesHitByPhaser.Count; i++)
				{
					if (obstaclesHitByPhaser[i] == null)
						obstaclesHitByPhaser.RemoveAt(i);
					else
						obstaclesHitByPhaser[i].GetComponent<DestroyByContact>().isAttakedByPhaser = false;
				}
			}

			Invoke("deactivatePhaser", phaserDuration);
		}

		private void deactivatePhaser()
		{
			isPhaserActive = false;
			phaserLineRendererGO.SetActive(isPhaserActive);
		}
    }
}
