using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float moveSpeed = 5f;

    public Rigidbody2D rb;
    public Camera cam;
    private Animator anim;

    private Vector2 movement;
    private Vector2 mousePost;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Gets input from keypoard
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        // screen pixel cordinateds to unity unit points
        mousePost = cam.ScreenToWorldPoint(Input.mousePosition);
        
    }

    private void FixedUpdate()
    {
        rb.MovePosition((rb.position + movement * moveSpeed * Time.fixedDeltaTime));
        if (movement !=Vector2.zero)
        {
            anim.SetBool("isMoving",true);
        }
        else
        {
            anim.SetBool("isMoving",false);
        }

        // Vector path
        Vector2 lookDir = mousePost - rb.position;
        
        // Function that returns angels between two vectors from X angel. Returns in radients convert to degrees
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90f;
        rb.rotation = angle;



    }
    
}
