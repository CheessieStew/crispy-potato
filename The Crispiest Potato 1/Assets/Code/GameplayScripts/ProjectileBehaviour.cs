using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class ProjectileBehaviour : MonoBehaviour {
    public GameObject target;
    Vector3 normalPosition;
    float startingDistance;
    // Use this for initialization
	void Start () {
        normalPosition = transform.position;
        if (target != null)
            startingDistance = (target.transform.position - transform.position).magnitude;
	}
	
	// Update is called once per frame
	void Update () {
        if (target == null)
        {
            GetComponent<ParticleSystemDestroyer>().enabled = true;
            return;
        }
        Vector3 dir = target.transform.position - normalPosition;
        float distance = dir.magnitude;
        if (distance < 0.05f)
        {
            GetComponent<ParticleSystemDestroyer>().enabled = true;
            return;
        }
        dir.Normalize();
        normalPosition += dir * Time.deltaTime * 2 * startingDistance;
        float div = distance / startingDistance;
        transform.position = normalPosition + new Vector3(0, -16 * div * (div - 1), 0);
	}
}
