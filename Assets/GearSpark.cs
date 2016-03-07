using UnityEngine;
using System.Collections;
using gameManager;
using player;
using UnityEngine.UI;

public class GearSpark : MonoBehaviour {

    public ParticleSystem GearSparks;
    public Slider Energy;
    public AudioSource Welding;
    

    private float deltaGearRotation;
    private float lastGearRotation;
    private float turningDuration = 2.0f;
    private float revingUpTime = 0f;
    private float energyStore = 0f;
    private float deltaEmissionRate = 0f;
   

	// Use this for initialization
	void Start () {

        GearSparks.emissionRate = 0;	
	}
	
	// Update is called once per frame
	void Update () {

        //deltaGearRotation = Mathf.Abs(gameObject.transform.rotation.z - lastGearRotation);
        //lastGearRotation = gameObject.transform.rotation.z;

        //if (deltaGearRotation == 0)
        //{
        //    GearSparks.emissionRate = 0;
        //    revingUpTime = 0;
        //}
        //else if (deltaGearRotation > 0)
        //{
        //    revingUpTime += Time.deltaTime;

        //    if (revingUpTime>3f)
        //    {
        //        GearSparks.emissionRate = 150;
        //    }
        //    else
        //    {
        //        GearSparks.emissionRate = Mathf.Clamp(GearSparks.emissionRate + .5F, 0, 150);
        //    }

        //}
        //Debug.Log(gameObject.GetComponent<Rigidbody2D>().angularVelocity);
        GearSparks.emissionRate = Mathf.Lerp(0, 150, Mathf.Abs(gameObject.GetComponent<Rigidbody2D>().angularVelocity) / 5000);

        deltaEmissionRate = GearSparks.emissionRate - deltaEmissionRate ;

        if (GearSparks.emissionRate> 0)
        {
            if (Welding.isPlaying == false  )
            {
                Debug.Log("Welding Sound");
                Welding.Play();
            }

        }

        if (GearSparks.emissionRate<= 10)
        {
            Welding.Stop();
        }

        if (deltaEmissionRate> 0)
        {

            


            energyStore += .2f ;

            if (energyStore >5)
            {
                energyStore = 5f;

            }

            Energy.value = energyStore;

            GameManager.instance.player.GetComponent<PlayerController>().skyScrollSpeed = Mathf.Lerp(5, 20, energyStore / 5f);

            //Debug.Log(energyStore);            

        }
        else if (deltaEmissionRate<=0)
        {
           
        }

        deltaEmissionRate = GearSparks.emissionRate;

	}
}
