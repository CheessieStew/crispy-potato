using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StopGameScreen : MonoBehaviour {

    float oldTimeScale = 0;
	public void StopGame()
    {
        GameObject.Find("UI/Canvas/TimeController").SetActive(false);
        GameObject.Find("UI/Canvas/Console").SetActive(false);
        GameObject.Find("UI/Canvas/EntityStats").SetActive(false);
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = oldTimeScale;
        GameObject.Find("UI/Canvas/TimeController").SetActive(true);
        GameObject.Find("UI/Canvas/Console").SetActive(true);
        GameObject.Find("UI/Canvas/EntityStats").SetActive(true);
        gameObject.SetActive(false);
    }

    public void Leave()
    {
        SceneManager.LoadScene("InitScene");
    }
}
