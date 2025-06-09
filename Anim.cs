using UnityEngine;

public class AnimationFix : MonoBehaviour
{
    [Header("Configuration")]
    public bool fixAnimationTeleport = true;
    
    private Vector3 lastFramePosition;
    private Vector3 basePosition;
    private bool isFirstFrame = true;
    
    void Start()
    {
        basePosition = transform.position;
        lastFramePosition = transform.position;
    }
    
    void LateUpdate()
    {
        if (!fixAnimationTeleport) return;
        
        Vector3 currentPos = transform.position;
        
        // Si c'est le premier frame, on enregistre juste la position
        if (isFirstFrame)
        {
            lastFramePosition = currentPos;
            isFirstFrame = false;
            return;
        }
        
        // Calculer le déplacement causé par l'animation
        Vector3 animationMovement = currentPos - lastFramePosition;
        
        // Si l'animation cause un grand saut (téléportation), on l'ignore
        if (animationMovement.magnitude > 1f) // Seuil de détection de téléportation
        {
            Debug.Log("Téléportation d'animation détectée et corrigée");
            transform.position = lastFramePosition;
        }
        else
        {
            // Mouvement normal, on met à jour la position de référence
            lastFramePosition = currentPos;
        }
    }
    
    // Méthode pour réinitialiser quand le joueur se déplace normalement
    public void UpdateBasePosition()
    {
        basePosition = transform.position;
        lastFramePosition = transform.position;
    }
}