using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class chose : MonoBehaviour
{
    public void Join(){

   
   SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
     public void Host()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    public void Back()
    {
       SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }




}
