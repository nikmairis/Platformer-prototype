using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using System;

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
    public Vector3 velocity;
    private float velocityXSmoothing;

    private Controller2D controller;

    private Vector2 directionalInput;
    private bool wallSliding;
    private int wallDirX;
    private Vector3 NetworkVelocity = new Vector3(0,0,0);
    private Vector2 NetworkInput;
    private Vector3 NetworkPosition = new Vector3(0,0,0);
    private double TimeOfSending = 0;
    private double NowTime = 0;
    public Vector3 PositionWhenReceived = new Vector3(0,0,0);
    private Vector3 PrevNetworkPosition = new Vector3(0,0,0);
    public double time1;
    public double time2 = 0;
    public bool reset;
    public float counter;
    public double SmoothFact =1;

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
        } else{
            NetworkCalculations();          
        }
    }
    // CALCULATES THE POSITION OF NETWORKED PLAYERS
    public void NetworkCalculations(){
            // value that boosts movespeed if distance between positions is too small.
            float booster = 1;
            // counts time thats passed from last received package
            counter += Time.deltaTime;
            // Resets timer, if new package has been received
            if(reset==true){
                counter = Time.deltaTime;
                reset= false;
            }
            // Calculates the distance between position when received and the networked players sent pos.
            float fulldistance = Vector3.Distance(PositionWhenReceived, NetworkPosition);
            // speeds up movement, if distance too small
            if(fulldistance <=0.5f){
                booster=2;
            }
            // ACTUALLY MOVES PLAYER. Lerps from Position in witch this avatar was, when received data to the
            // position that was sent through the network and adds 1/4 of the velocity that the player had
            // at the moment of sending. SmoothFact =  1/(time between last 2 packets)
            this.transform.position = Vector3.Lerp(PositionWhenReceived, NetworkPosition+(NetworkVelocity*(float)time1/4),
             (float)SmoothFact*counter*booster);
             //Doesn't let the t parameter of Vector3.Lerp go above 1
            if((float)SmoothFact*counter*booster >=0.95f && fulldistance <=0.5f){
                counter -= Time.deltaTime;
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
        if(directionalInput.x == 0){
            if(controller.FaceDirForWeapon == 1){
            velocity.x += DashSpeedx;
            velocity.y += DashSpeedy;

            }
            if(controller.FaceDirForWeapon == -1){
            velocity.x -= DashSpeedx;
            velocity.y += DashSpeedy;

        }
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
                stream.SendNext(velocity);
                stream.SendNext(this.transform.position);
                stream.SendNext(NetworkPosition);

            }else{
                NetworkVelocity = (Vector3)stream.ReceiveNext();
                NetworkPosition = (Vector3)stream.ReceiveNext();
                PrevNetworkPosition = (Vector3)stream.ReceiveNext();
                time2 = info.timestamp;
                time1 = PhotonNetwork.Time - time2;
                time2 = PhotonNetwork.Time;
                SmoothFact = 1/time1;
                TimeOfSending = info.timestamp;
                PositionWhenReceived = this.transform.position;
                NowTime = PhotonNetwork.Time;
                reset=true;
            }
        }
    }
}