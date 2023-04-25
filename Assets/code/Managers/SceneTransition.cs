using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string scene_to_load;

    public void OnTriggerEnter2D(Collider2D hit)
    {
        print("Se llama");

        if (hit.CompareTag("Player"))
        {
            SceneManager.LoadScene(scene_to_load);
        }
    }
}
