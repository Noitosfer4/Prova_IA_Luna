using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
   public static void LoadScenes(string cena) { 
        SceneManager.LoadScene(cena);
    }

    public void fechaessaporra(){
        Application.Quit();
    }
}
