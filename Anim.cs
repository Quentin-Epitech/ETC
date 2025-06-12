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
        
        
        if (isFirstFrame)
        {
            lastFramePosition = currentPos;
            isFirstFrame = false;
            return;
        }
        
        
        Vector3 animationMovement = currentPos - lastFramePosition;
        
        
        if (animationMovement.magnitude > 1f) 
        {
            Debug.Log("Téléportation d'animation détectée et corrigée");
            transform.position = lastFramePosition;
        }
        else
        {
            
            lastFramePosition = currentPos;
        }
    }
    
    
    public void UpdateBasePosition()
    {
        basePosition = transform.position;
        lastFramePosition = transform.position;
    }
}