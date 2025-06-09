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
    public float baseSpeed = 10f; // Vitesse de départ
    public float maxSpeed = 25f; // Vitesse maximum
    public float accelerationRate = 0.5f; // Augmentation par seconde
    public float accelerationInterval = 5f; // Accélère toutes les X secondes

    [Header("Saut et Gravité")]
    public float jumpHeight = 3f;
    public float gravity = -30f;
    public float groundY = 0f; // Hauteur du sol
    
    private Vector3 velocity;
    private bool isGrounded;
    private bool isJumping = false;
    private float gameTime = 0f; // Temps de jeu écoulé
    private bool jumpInput = false; // Pour capturer l'input de saut

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("❌ CharacterController manquant sur le joueur !");
        }
        
        // Initialiser les valeurs
        jumpHeight = 3f;
        gravity = -30f;
        forwardSpeed = baseSpeed; // Commencer avec la vitesse de base
        
        // Positionner le joueur au sol au démarrage
        Vector3 startPos = transform.position;
        startPos.y = groundY + (controller.height / 2f);
        transform.position = startPos;
        
        velocity = Vector3.zero;
        gameTime = 0f;
    }

    void Update()
    {
        // Capturer l'input de saut dans Update() pour ne pas le rater
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInput = true;
        }

        // Augmenter le temps de jeu
        gameTime += Time.deltaTime;
        
        // Augmenter la vitesse progressivement
        UpdateSpeed();

        // Vérification améliorée du sol
        CheckGrounded();

        // Gestion du saut - CORRIGÉE
        HandleJump();

        // Mouvement vers l'avant (avec la vitesse actuelle)
        Vector3 forwardMove = Vector3.forward * forwardSpeed;

        // Gestion des touches pour changer de voie
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

        // Calculer la position cible selon la voie
        Vector3 targetPosition = new Vector3(0, transform.position.y, transform.position.z);

        if (currentLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (currentLane == 2)
            targetPosition += Vector3.right * laneDistance;

        // Mouvement latéral vers la voie cible
        Vector3 moveDirection = targetPosition - transform.position;
        Vector3 lateralMove = new Vector3(moveDirection.x, 0, 0).normalized * laneChangeSpeed;

        // Appliquer la gravité
        ApplyGravity();

        // Combiner tous les mouvements
        Vector3 finalMove = (forwardMove + lateralMove) * Time.deltaTime;
        finalMove.y = velocity.y * Time.deltaTime;

        // Appliquer le mouvement
        controller.Move(finalMove);

        // Correction de position et atterrissage
        HandleLanding();
    }

    void CheckGrounded()
    {
        // Vérification plus précise du sol
        float playerBottomY = transform.position.y - (controller.height / 2f);
        float groundTolerance = 0.1f;
        
        // Le joueur est au sol si :
        // 1. Il est proche du sol
        // 2. Sa vélocité verticale n'est pas positive (pas en train de monter)
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
            // CORRECTION PRINCIPALE : Force de saut consistante
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            
            // Réinitialiser complètement la vélocité Y pour éviter l'accumulation
            velocity.y = 0f;
            
            // Appliquer la nouvelle vélocité de saut
            velocity.y = jumpVelocity;
            
            isJumping = true;
            isGrounded = false; // Forcer isGrounded à false pendant le saut
            
            Debug.Log($"SAUT! Vélocité Y: {velocity.y:F2}, JumpHeight: {jumpHeight}, Gravity: {gravity}");
        }
        
        // Réinitialiser l'input de saut
        jumpInput = false;
    }

    void ApplyGravity()
    {
        // Appliquer la gravité seulement si on n'est pas fermement au sol
        if (!isGrounded || isJumping)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            // Au sol : maintenir une légère pression vers le bas
            velocity.y = Mathf.Max(velocity.y, -2f);
        }
    }

    void HandleLanding()
    {
        // Forcer le joueur à rester au sol minimum
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
        // Calculer combien de paliers d'accélération ont été atteints
        int speedLevels = Mathf.FloorToInt(gameTime / accelerationInterval);
        
        // Calculer la nouvelle vitesse
        float targetSpeed = baseSpeed + (speedLevels * accelerationRate);
        
        // Limiter à la vitesse maximum
        targetSpeed = Mathf.Clamp(targetSpeed, baseSpeed, maxSpeed);
        
        // Appliquer la vitesse (changement immédiat à chaque palier)
        forwardSpeed = targetSpeed;
    }

    // Fonction pour obtenir la vitesse actuelle (utile pour d'autres scripts)
    public float GetCurrentSpeed()
    {
        return forwardSpeed;
    }
    
    // Fonction pour obtenir le temps de jeu (utile pour le score)
    public float GetGameTime()
    {
        return gameTime;
    }

    void OnDrawGizmosSelected()
    {
        // Dessiner le niveau du sol
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(0, groundY, transform.position.z), new Vector3(20, 0.1f, 20));
        
        // Dessiner la position minimale du joueur
        if (controller != null)
        {
            float minY = groundY + (controller.height / 2f);
            Gizmos.color = isGrounded ? Color.blue : Color.red;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, minY, transform.position.z), 0.5f);
        }
    }
}