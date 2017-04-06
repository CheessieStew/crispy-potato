using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Animal : MonoBehaviour
{
    public GameObject TextPrefab;
    protected BrainWrapper Brain;
    protected LivingEntityBody Body;

    protected virtual void Start()
    {
        TextPrefab = Instantiate(TextPrefab);
        TextPrefab.SetActive(false);
        TextPrefab.transform.SetParent(GameObject.Find("Canvas").transform, false);
        TextPrefab.transform.localScale = new Vector3(0, 0, 0);

        Brain = GetComponent<BrainWrapper>();
        if (Brain == null || Brain.Brain == null)
        {
            if (Brain == null)
                Brain = gameObject.AddComponent<BrainWrapper>();
            Brain.Brain = new TigerBrain();
            print("added tigerbrain");
        }
        Body = GetComponent<LivingEntityBody>();
    }

    // the coroutine must be started after all Start() were invoked, e.g. here
    private bool aiCoroutineStarted = false;
    protected virtual void Update()
    {
        if (TextPrefab.activeSelf)
        {
            if (0 < Vector3.Dot((transform.position - Camera.main.transform.position), Camera.main.transform.forward))
            {
                TextPrefab.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2);
                float s;
                s = 10.0f / (Camera.main.transform.position - transform.position).magnitude;
                TextPrefab.transform.localScale = new Vector3(s, s, s);
            }
            else
            {
                TextPrefab.transform.localScale = new Vector3(0, 0, 0);
            }
        }

        if (Brain.Brain == null)
        {
            return;
        }
        if (!aiCoroutineStarted)
        {
            StartCoroutine(aiCoroutine());
            aiCoroutineStarted = true;
        }
    }

    private IEnumerator aiCoroutine()
    {
        Brain.Feel(Body.BodilyFunctions);
        Brain.See(Body.GetDescriptions());
        foreach (var command in Brain.GetDecisions())
        {
            if (command is AiProtocol.Command.Talk)
            {
                TextPrefab.SetActive(true);
                TextPrefab.GetComponent<Text>().text = (command as AiProtocol.Command.Talk).Words.ToString();
            }
            else TextPrefab.SetActive(false);
            yield return Body.Execute(command);
            Brain.Feel(Body.BodilyFunctions);
            Brain.See(Body.GetDescriptions());
        }
    }

    public void Hear(AiProtocol.Words words) 
    {
        Brain.Hear(words);
    }
}
