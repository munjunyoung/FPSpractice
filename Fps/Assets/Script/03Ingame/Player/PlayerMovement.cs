using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using FpsLib;
public class PlayerMovement : MonoBehaviour
{

    //Move
    private float moveSpeed = 10f;
    private float moveMaxSpeed = 50f;
    public Vector3 playerMoving;
    public Vector3 playerRotation;

    //Rotation
    public float rotationX = 0f;
    public float rotationY = 0f;
    private float rotationSens = 100f;

    private float rotationMinY = -35f;
    private float rotationMaxY = 35f;

    GameObject camObject;
    public PositionInfo playerPosition;
    public Transform playerCenterObject;
    public Transform playerSpineTransform;

    public Animator playerAnimator;

    void Start()
    {
        camObject = GameObject.Find("Main Camera");
        playerAnimator = GetComponent<Animator>();
        playerPosition = new PositionInfo(transform.position.x, transform.position.y, transform.position.z);
        playerCenterObject = transform.Find("CenterObject").transform;
        playerSpineTransform = transform.Find("Bip001").transform.Find("Bip001 Pelvis").transform.Find("Bip001 Spine").transform;

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (GetComponent<Player>().Client)
        {
            Move();
            PlayerTurn();
            PlayerAnim();
        }
    }
    private void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        playerMoving = new Vector3(moveHorizontal, 0f, moveVertical);
        if (Input.GetKey(KeyCode.LeftShift))
            transform.Translate(playerMoving * moveMaxSpeed * Time.deltaTime);
        else
            transform.Translate(playerMoving * moveSpeed * Time.deltaTime);
      
        
    }
    
    private void PlayerTurn()
    {
        rotationX += Input.GetAxis("Mouse X") * rotationSens * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * rotationSens * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, rotationMinY, rotationMaxY);

        transform.localEulerAngles = new Vector3(0f, rotationX, 0f);
        playerCenterObject.localEulerAngles = new Vector3(-rotationY, 0f, 0f);
        playerSpineTransform.localEulerAngles = new Vector3(0f, 0f, rotationY);
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
