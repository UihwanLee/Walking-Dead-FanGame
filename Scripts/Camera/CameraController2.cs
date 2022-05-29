using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2 : MonoBehaviour {

    private float CameraMoveSpeed = 120.0f;

    [SerializeField]
    private GameObject CameraFollowObj;
	
	// Update is called once per frame
	void Update () {
        CameraUpdate();
	}

    private void CameraUpdate()
    {
        Transform target = CameraFollowObj.transform;

        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
