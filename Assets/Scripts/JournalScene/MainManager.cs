using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainManager : MonoBehaviour
{
    public List<string> quests = new();
    public static MainManager mainManager;
    
    //makes sure there is only one instace of main manager throughout game
    //can transition between scenes without losing data
    private void Awake()
    {
        if (mainManager != null)
        {
            Destroy(gameObject);
            return;
        }

        mainManager = this;
        DontDestroyOnLoad(gameObject);
    }
    //trying to make it reload
}
