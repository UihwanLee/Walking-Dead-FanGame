using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCoilison : MonoBehaviour {

    public float minDistance = 0.001f;
    public float maxDistance = 0.002f;
    public float smooth = 10.0f;
    Vector3 doIlyDir;
    public Vector3 doIlyDirAdjusted;
    public float distance;

	// Use this for initialization
	void Awake () {
        doIlyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 desiredCameraPos = transform.parent.TransformPoint(doIlyDir * maxDistance);
        RaycastHit hit;

        if(Physics.Linecast(transform.parent.position, desiredCameraPos, out hit))
        {
            distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, doIlyDir * distance, Time.deltaTime * smooth);
	}
}
