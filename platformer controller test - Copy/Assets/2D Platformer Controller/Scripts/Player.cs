using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

namespace testprojekts{
[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour, IPunObservable
{
    private PhotonView PV;
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpApex = .4f;
    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;
    public float moveSpeed = 12f;
    private float DashSpeedy = 5;
    [HideInInspector]
    public float DashSpeedx = 30;
    private bool IsDashing = false;
    private float DashTimer = -0.1f;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public bool canDoubleJump;
    private bool isDoubleJumping = false;

    public float wallSlideSpeedMax = 3f;
    public float wallStickTime = .25f;
    private float timeToWallUnstick;

    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector3 velocity;
    private float velocityXSmoothing;

    private Controller2D controller;

    private Vector2 directionalInput;
    private bool wallSliding;
    private int wallDirX;

    public void Awake(){
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine && GetComponent<PlayerInput>() != null) {
            Destroy(GetComponent<PlayerInput>());
        }
    }

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        controller = GetComponent<Controller2D>();
    //ENABLE, TO SWITCH TO SCINEMATIC CHARACTER FOLLOW CAM!
        //if(PV.IsMine)
        //GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().m_Follow = this.transform;
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    private void Update()
    {
        if(PV.IsMine){
        if(Input.GetKeyDown(KeyCode.F)){
            if(IsDashing == false){
                Dash();
            }
        }
        }

        //Resets the DoubleJump
        if (controller.collisions.below && !wallSliding)
        {
            isDoubleJumping = false;
        }
        //Resets Dash timer. Fixes the bug of floating in air
        if(controller.collisions.below && !wallSliding && DashTimer <=0){
            IsDashing = false;
        }

        CalculateVelocity();
        HandleWallSliding();
        controller.Flip(controller.FaceDirForWeapon);
        controller.Move(velocity * Time.deltaTime, directionalInput);


        if(IsDashing){
            DashTimer -= Time.deltaTime;
        }
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0f;
        }
    }
    public void Dash(){
        IsDashing = true;
        DashTimer = 0.3f;
        if(directionalInput.x == 1){
            velocity.x += DashSpeedx;
            velocity.y += DashSpeedy;

        }
        if(directionalInput.x == -1){
            velocity.x -= DashSpeedx;
            velocity.y += DashSpeedy;

        }

    }
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        DashTimer = -0.1f;
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
            isDoubleJumping = false;
        }
        if (controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
            isDoubleJumping = false;
        }
        if (canDoubleJump && !controller.collisions.below && !isDoubleJumping && !wallSliding)
        {
            velocity.y = maxJumpVelocity;
            isDoubleJumping = true;
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    private void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0f)
            {
                velocityXSmoothing = 0f;
                velocity.x = 0f;
                if (directionalInput.x != wallDirX && directionalInput.x != 0f)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    private void CalculateVelocity()
    {
        //HANDLES HORIZONTAL VELOCITY AND DASHES
        float targetVelocityX;
        if(DashTimer >= 0.2){
            targetVelocityX = 0;
        }
        else{
            targetVelocityX = directionalInput.x * moveSpeed;
        }
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        
        //HANDLES VERTICAL VELOCITY AND DASHES
        if(DashTimer >= 0){
            velocity.y = 0;
        }
        else{
            velocity.y += gravity * Time.deltaTime;
        }
    }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting){
                stream.SendNext(directionalInput);
                stream.SendNext(isDoubleJumping);
                stream.SendNext(velocity);
                stream.SendNext(wallSliding);
                stream.SendNext(this.transform.position);
            }else{
                directionalInput = (Vector2)stream.ReceiveNext();
                isDoubleJumping = (bool)stream.ReceiveNext();
                velocity = (Vector3)stream.ReceiveNext();
                wallSliding = (bool)stream.ReceiveNext();
                this.transform.position = (Vector3)stream.ReceiveNext();
            }
        }
    }
}