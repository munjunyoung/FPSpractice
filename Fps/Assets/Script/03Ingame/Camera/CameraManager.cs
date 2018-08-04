using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public GameObject playerCenterObject = null; //타겟 오브젝트
    

    private float smoothing = 25f;
    float camZpos = 5f;
    bool zoomIn = false;

    void Start()
    {
        GetComponent<Camera>().fieldOfView = 40;
    }

    void Update()
    {
        if (playerCenterObject != null)
        {
            FollowCam();
            TransCamZoomAngle();
        }
    }

    /// <summary>
    /// Camera Moving
    /// </summary>
    private void FollowCam()
    {
        //원하는 앵글을 시간변위에따라 현재 앵글값을 camCurrentangley변수에 저장
        float camCurrentAngleY = Mathf.LerpAngle(transform.eulerAngles.y, playerCenterObject.transform.eulerAngles.y, smoothing * Time.deltaTime);
        float camCurrentAngleX = Mathf.LerpAngle(transform.eulerAngles.x, playerCenterObject.transform.eulerAngles.x, smoothing * Time.deltaTime);


        //이동할 앵글을 회전으로 변환
        Quaternion rotY = Quaternion.Euler(camCurrentAngleX, camCurrentAngleY, 0);

        //카메라 포지션 이동

        Vector3 newPosition = transform.position;
        newPosition = playerCenterObject.transform.position - (rotY * Vector3.forward * camZpos);

        transform.position = newPosition;
    }

    /// <summary>
    /// Camera zoom
    /// </summary>
    void TransCamZoomAngle()
    {
        if (Input.GetMouseButtonDown(1) && !zoomIn)
        {
            zoomIn = true;
            camZpos = 5f;

            playerCenterObject.transform.localPosition = new Vector3(0.3f, 2.5f, 0f);
        }
        else if (Input.GetMouseButtonDown(1) && zoomIn)
        {
            zoomIn = false;
            camZpos = -0.1f;

            playerCenterObject.transform.localPosition = new Vector3(0.2f, 2.2f, 0f);
        }
    }
}



//캠포스
//float camCurrentAngleX = Mathf.LerpAngle(transform.eulerAngles.x, playerObject.transform.eulerAngles.x, smoothing * Time.deltaTime);
//Quaternion rotX = Quaternion.Euler(camCurrentAngleX, 0, 0);

// transform.LookAt(playerObject.transform);
//transform.position = playerObject.transform.position - (rotX * Vector3.forward * 10f) + (Vector3.up * 5f);
// transform.LookAt(playerObject.transform);
// Vector3 targetCamPos = playerObject.transform.position + camOffset;
// transform.localPosition = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
// Quaternion camRotation = Quaternion.LookRotation(camOffset);
//transform.rotation = camRotation;
