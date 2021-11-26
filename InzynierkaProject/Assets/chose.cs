using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class chose : MonoBehaviour
{
  
   
    public void Join(){

   //Add script to writing and finding server 
   SceneManager.LoadScene("JoinRoom",LoadSceneMode.Single);
    }
     public void Host()
    {
    //Add creating random hot and go to your own server
    SceneManager.LoadScene("JoinRoom",LoadSceneMode.Single);
    }
    public void Back()
    {
        // Its working
       SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
   
