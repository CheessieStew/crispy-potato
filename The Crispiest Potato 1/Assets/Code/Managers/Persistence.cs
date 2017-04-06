using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Persistence : MonoBehaviour {

	// Use this for initialization
	void Awake()
	{
	    DontDestroyOnLoad(gameObject);
	    foreach (Transform child in transform)
	    {
	        DontDestroyOnLoad(child);
	    }
        print("lol");
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    static void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        print("scene loaded: " + scene);
        bool isMenu = scene.name == "InitScene";
        GameManager.SceneLoaded(isMenu);
        EntityManager.SceneLoaded(isMenu);

    }

    // Update is called once per frame
    void Update () {
		
	}
}
