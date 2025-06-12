using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private float laneDistance = 3f; // Distance entre les voies
    private int currentLane = 1; // 0 = gauche, 1 = centre, 2 = droite

    [Header("Vitesses")]
    public float forwardSpeed = 10f;
    public float laneChangeSpeed = 10f;
    
    [Header("Accélération")]
    public float baseSpeed = 10f; // départ
    public float maxSpeed = 25f; // maximum
    public float accelerationRate = 0.5f; // Augmentation 
    public float accelerationInterval = 5f; // Accélère 

    [Header("Saut et Gravité")]
    public float jumpHeight = 3f;
    public float gravity = -30f;
    public float groundY = 0f; // Hauteur 
    
    private Vector3 velocity;
    private bool isGrounded;
    private bool isJumping = false;
    private float gameTime = 0f; // Temps de jeu 
    private bool jumpInput = false; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("❌ CharacterController manquant sur le joueur !");
        }
        
        
        jumpHeight = 3f;
        gravity = -30f;
        forwardSpeed = baseSpeed; // vitesse de base
        
        // joueur au sol 
        Vector3 startPos = transform.position;
        startPos.y = groundY + (controller.height / 2f);
        transform.position = startPos;
        
        velocity = Vector3.zero;
        gameTime = 0f;
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInput = true;
        }

       
        gameTime += Time.deltaTime;
        
       
        UpdateSpeed();

        
        CheckGrounded();

       
        HandleJump();

     
        Vector3 forwardMove = Vector3.forward * forwardSpeed;

       
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentLane > 0)
                currentLane--;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentLane < 2)
                currentLane++;
        }

        
        Vector3 targetPosition = new Vector3(0, transform.position.y, transform.position.z);

        if (currentLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (currentLane == 2)
            targetPosition += Vector3.right * laneDistance;

        
        Vector3 moveDirection = targetPosition - transform.position;
        Vector3 lateralMove = new Vector3(moveDirection.x, 0, 0).normalized * laneChangeSpeed;

        
        ApplyGravity();

        
        Vector3 finalMove = (forwardMove + lateralMove) * Time.deltaTime;
        finalMove.y = velocity.y * Time.deltaTime;

        
        controller.Move(finalMove);

        
        HandleLanding();
    }

    void CheckGrounded()
    {
        
        float playerBottomY = transform.position.y - (controller.height / 2f);
        float groundTolerance = 0.1f;
        
        
        
        isGrounded = (playerBottomY <= groundY + groundTolerance) && (velocity.y <= 0.1f);
        
        if (isGrounded && isJumping && velocity.y <= 0)
        {
            isJumping = false;
            velocity.y = 0;
            Debug.Log("ATTERRISSAGE détecté!");
        }
    }

    void HandleJump()
    {
        if (jumpInput && isGrounded && !isJumping)
        {
            
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            
            
            velocity.y = 0f;
            
            
            velocity.y = jumpVelocity;
            
            isJumping = true;
            isGrounded = false; 
            
            Debug.Log($"SAUT! Vélocité Y: {velocity.y:F2}, JumpHeight: {jumpHeight}, Gravity: {gravity}");
        }
        
        
        jumpInput = false;
    }

    void ApplyGravity()
    {
       
        if (!isGrounded || isJumping)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            
            velocity.y = Mathf.Max(velocity.y, -2f);
        }
    }

    void HandleLanding()
    {
        
        Vector3 currentPos = transform.position;
        float minY = groundY + (controller.height / 2f);
        
        if (currentPos.y < minY)
        {
            currentPos.y = minY;
            transform.position = currentPos;
            velocity.y = 0;
            
            if (isJumping)
            {
                isJumping = false;
                Debug.Log("CORRECTION: Remis au sol après atterrissage!");
            }
        }
    }

    void UpdateSpeed()
    {
        
        int speedLevels = Mathf.FloorToInt(gameTime / accelerationInterval);
        
        
        float targetSpeed = baseSpeed + (speedLevels * accelerationRate);
        
        
        targetSpeed = Mathf.Clamp(targetSpeed, baseSpeed, maxSpeed);
        
        
        forwardSpeed = targetSpeed;
    }

    
    public float GetCurrentSpeed()
    {
        return forwardSpeed;
    }
    
    
    public float GetGameTime()
    {
        return gameTime;
    }

    void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(0, groundY, transform.position.z), new Vector3(20, 0.1f, 20));
        
        
        if (controller != null)
        {
            float minY = groundY + (controller.height / 2f);
            Gizmos.color = isGrounded ? Color.blue : Color.red;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, minY, transform.position.z), 0.5f);
        }
    }
}