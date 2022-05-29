using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour {

    // Normal-Mode
    Vector3 FollowPos;
    [SerializeField]
    private float clampAngle = 80.0f; // 80 66

    [SerializeField]
    private float inputSensitivity = 150.0f;

    [SerializeField]
    private float clampAngle_GunMode = 40.0f; // 80 66

    [SerializeField]
    private float inputSensitivity_GunMode = 70.0f;

    [SerializeField]
    private GameObject CameraObj;

    [SerializeField]
    private GameObject PlayerObj;

    private float inputX;
    private float inputZ;
    private float mouseX;
    private float mouseY;
    private float finalInputX;
    private float finalInputY;
    private float rotY = 0.0f;
    private float rotX = 0.0f;
    private Quaternion localRotation;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private float cameraRotationXLimit;
    [SerializeField]
    private float cameraRotationYLimit;

    [SerializeField]
    private GameObject cam;

    // 인벤토리 창
    [SerializeField] private GameObject theInventoryWindow;

    // PlayerController
    private PlayerController controller;

    // Use this for initialization
    void Start () {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = FindObjectOfType<PlayerController>();
    }        
	
	// Update is called once per frame
	void Update () {
        if(!theInventoryWindow.activeSelf)
        {
            if (!(controller.isTalking == true || controller.val == true))
            {
                CameraUpdate();
            }
        }
    }

    // PauseMenu: Setting Menu에서 Sensitvity 정보 가져오기
    // In-Field Mouse
    public void SetInFieldSensitivity(float _sensitivity)
    {
        inputSensitivity = _sensitivity;
    }
    // Out-Field Mouse
    public void SetOutFieldSensitivity(float _sensitivity)
    {
        inputSensitivity_GunMode = _sensitivity;
    }
    // Shot-Mode Mouse
    public void SetShotModeSensitivity(float _sensitivity)
    {
        lookSensitivity = _sensitivity;
    }

    private void CameraUpdate()
    {
        inputX = Input.GetAxis("RightStickHorizontal");
        inputZ = Input.GetAxis("RightStickVertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        // Normal
        // We setup the rotation of the sticks here
        finalInputX = inputX + mouseX;
        finalInputY = inputZ + mouseY;

        if (controller.getShotMode())
        {
            rotY += finalInputX * lookSensitivity * Time.deltaTime;
            rotX += finalInputY * lookSensitivity * Time.deltaTime;
        }
        else if(controller.isGunMode)
        {
            rotY += finalInputX * inputSensitivity_GunMode * Time.deltaTime;
            rotX += finalInputY * inputSensitivity_GunMode * Time.deltaTime;
        }
        else
        {
            rotY += finalInputX * inputSensitivity * Time.deltaTime;
            rotX += finalInputY * inputSensitivity * Time.deltaTime;
        }

        if (controller.getShotMode())
        {
            Shot_Mode();
            PlayerRoatation();
        }
        else
        {
            if (controller.isGunMode)
            {
                Gun_Mode();
                //PlayerRoatation_GunMode();
            }
            else
                Normal_Mode();
        }
    }

    private void Normal_Mode()
    {
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }

    private void Gun_Mode()
    {
        rotX = Mathf.Clamp(rotX, -clampAngle_GunMode, clampAngle_GunMode);
        localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }

    private void Shot_Mode()
    {
        rotX = Mathf.Clamp(rotX, -cameraRotationXLimit, cameraRotationXLimit);
        localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }

    private void PlayerRoatation_GunMode()
    {
        //localRotation.y = 0;
        PlayerObj.transform.rotation = localRotation;
        //StartCoroutine(PlayerRoatationDelay());
    }

    IEnumerator PlayerRoatationDelay()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerObj.transform.rotation = localRotation;
    }

    private void PlayerRoatation()
    {
        PlayerObj.transform.rotation = localRotation;
    }

    public Quaternion getRoatation()
    {
        return localRotation;
    }

    public void setPos()
    {
        cam.GetComponent<CinemachineVirtualCamera>().Priority = 111;
    }

    public void resPos()
    {
        cam.GetComponent<CinemachineVirtualCamera>().Priority = 10;
    }
}
