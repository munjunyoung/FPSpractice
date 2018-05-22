using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
public class PlayerMovement : MonoBehaviour {

    //Move
    private float moveSpeed = 10f;
    private float moveMaxSpeed = 50f;
    private Vector3 playerMoving;

    //Rotation
    public float rotationX = 0f;
    public float rotationY = 0f;
    private float rotationSens = 100f;

    private float rotationMinY = -35f;
    private float rotationMaxY = 35f;

    public GameObject camObject;
    public GameObject centerObject;

    Animator playerAnimator;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        transform.position = new Vector3(ClientNetworkManager.playerPos.x, ClientNetworkManager.playerPos.y, ClientNetworkManager.playerPos.z);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Move();
        PlayerTurn();
    }
    private void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        playerMoving = new Vector3(moveHorizontal, 0f, moveVertical);

        if (Input.GetKey(KeyCode.LeftShift))
            transform.Translate(playerMoving * moveMaxSpeed*Time.deltaTime);
        else
            transform.Translate(playerMoving * moveSpeed*Time.deltaTime);

        PlayerAnim();

    }

    private void PlayerTurn()
    {
        rotationX += Input.GetAxis("Mouse X") * rotationSens * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * rotationSens * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, rotationMinY, rotationMaxY);
        
        transform.localEulerAngles = new Vector3(0f, rotationX, 0f);
        centerObject.transform.localEulerAngles = new Vector3(-rotationY,0f, 0f);
        camObject.transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0f);
    }
    

    void PlayerAnim()
    {
        if (Mathf.Abs(playerMoving.x) < 0.1f && Mathf.Abs(playerMoving.z) < 0.1f)
        {
            playerAnimator.SetTrigger("Idle");
        }
        else
        {
            playerAnimator.SetTrigger("Run");
        }
        
        if (Input.GetMouseButton(0))
        {
            playerAnimator.SetTrigger("Shoot");
        }
    }
}
