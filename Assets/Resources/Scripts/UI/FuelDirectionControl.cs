using UnityEngine;
using System.Collections;

public class FuelDirectionControl : MonoBehaviour
{
    public bool m_UseRelativeRotationAndPosition = true;

    private Quaternion m_FuelRelativeRotation;

    private Quaternion m_RpmRelativeRotation;


	// Use this for initialization
	void Start ()
    {
        m_FuelRelativeRotation = transform.localRotation;
        m_RpmRelativeRotation = transform.localRotation;

    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(m_UseRelativeRotationAndPosition)
        {
            transform.rotation = m_FuelRelativeRotation;

            transform.rotation = m_RpmRelativeRotation;
        }
	}
}
