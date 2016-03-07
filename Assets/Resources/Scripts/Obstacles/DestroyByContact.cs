using UnityEngine;
using System.Collections;
using Environment;
using gameManager;
using player;
using UnityEngine.UI;
using Collectibles;

public class DestroyByContact : MonoBehaviour
{
    public float health;

    private EnvironmentBuilder environmentBuilder;
    private PlayerController playerController;
    public  PlayerManager playerManager;
    private bool isColliding;
	public bool isAttakedByPhaser;
    private float damageTimer;
    public float damageBase;
    public Slider healthSlider;
    // Use this for initialization
    void Start()
    {
        isColliding = false;
        environmentBuilder = GameObject.Find("GameManager").GetComponent<EnvironmentBuilder>();
        damageTimer = 0;
        healthSlider.maxValue = health;
        healthSlider.value = healthSlider.minValue;
        //healthSlider.enabled = false;
		playerController = GameManager.instance.player.GetComponent<PlayerController>();
		playerManager = GameManager.instance.player.GetComponent<PlayerManager>();
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            isColliding = true;
            playerController.CanPlayerMove = false;
            environmentBuilder.Scroll = false;
            //GameManager.instance.SetActiveObstacleHealth(true);
            //GameManager.instance.UpdateObstacleHealthText(health.ToString());
            healthSlider.value = health;
            //healthSlider.enabled = true;
			GameManager.instance.gameObject.GetComponent<SpawnHazards>().waitForDestroy = true;
			GameManager.instance.gameObject.GetComponent<CollectibleSpawner>().waitForDestroy = true;

            //GameManager.instance.soundEffects.PlayMusic(0);
            //Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(isColliding || isAttakedByPhaser)
        {
            damageTimer += Time.deltaTime;

            //Debug.Log(health);
            if(damageTimer > damageBase)
            {
                health -= playerManager.GetCurrentRPMs();
                //GameManager.instance.UpdateObstacleHealthText(Mathf.RoundToInt(health).ToString());
                damageTimer = 0;
                healthSlider.value = health;

                if(health <= 0)
                {
                    //GameManager.instance.SetActiveObstacleHealth(false);
                    SelfDestruct();
                }

            }
        }
    }



    void SelfDestruct()
    {
        playerController.CanPlayerMove = true;
        environmentBuilder.Scroll = true;
        GameManager.instance.soundEffects.PlayMusic(0);

		if(gameObject.name.IndexOf("mine") >= 0)
		{
			gameObject.GetComponent<DiamondMineBonus>().instantiateDiamondGUI();
		}

        Destroy(gameObject);
		Debug.Log("Rock Destroyed");
		GameManager.instance.gameObject.GetComponent<SpawnHazards>().waitForDestroy = false;
		GameManager.instance.gameObject.GetComponent<CollectibleSpawner>().waitForDestroy = false;
    }
}
