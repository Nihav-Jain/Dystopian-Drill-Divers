using UnityEngine;
using System.Collections;
using gameManager;
using player;

public class RPMHandler : MonoBehaviour
{

    public RectTransform startPos;
    public RectTransform endPos;
    public float trajectoryHeight = 2f;
    // Update is called once per frame
    private PlayerManager playerManager;

    void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }
    void Update()
    {
        // calculate current time within our lerping time range
        float cTime = Time.time * 1f;
        // calculate straight-line lerp position:
        float progress = (playerManager.currentRpms - playerManager.RpmMin) / (playerManager.RpmMax - playerManager.RpmMin);
        Vector3 currentPos = Vector3.Lerp(startPos.position, endPos.position, progress);
        // add a value to Y, using Sine to give a curved trajectory in the Y direction
        currentPos.y += trajectoryHeight * Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI);
        // finally assign the computed position to our gameObject:
        transform.position = currentPos;
    }
}
