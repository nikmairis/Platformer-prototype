using UnityEngine;
namespace testprojekts{
[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player player;
    public GameObject JumpParticle;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if (Input.GetButtonDown("Jump"))
        {   
            Instantiate(JumpParticle, this.transform.position + new Vector3(0,-0.5f,0), this.transform.rotation);
            player.OnJumpInputDown();
        }

        if (Input.GetButtonUp("Jump"))
        {
            player.OnJumpInputUp();
        }
    }
}
}