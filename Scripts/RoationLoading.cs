using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoationLoading : MonoBehaviour {

    public RectTransform loadingIcon;
    public float timestep;
    public float oneStepAngle;

    float startTime;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        Loading();
	}

    private void Loading()
    {
        if(Time.time - startTime >= timestep)
        {
            Vector3 iconAngle = loadingIcon.localEulerAngles;
            iconAngle.z += oneStepAngle;

            loadingIcon.localEulerAngles = iconAngle;

            startTime = Time.time;
        }
    }
}
