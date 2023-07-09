using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // --------------------------------------------------
    // Atributes
    // --------------------------------------------------

    // Private
    //      Standard var


    // --------------------------------------------------
    // Methods
    // --------------------------------------------------


    //Start
    public void Start()
    {
    }

    //Update
    public void Update()
    {
    }

    /// <summary>
    /// Go to main menu
    /// </summary>
    public void BackMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Nexo");
    }
}
