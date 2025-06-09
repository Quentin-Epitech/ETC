using UnityEngine;
using UnityEngine.SceneManagement;
 
public class Menu : MonoBehaviour
{
    public string NomDeScene;
 
    public void AllerAuNiveau()
    {
        SceneManager.LoadScene(NomDeScene);
    }
 
    private void OnTriggerEnter(Collider other)
    {
        AllerAuNiveau();
    }
}
