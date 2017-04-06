using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityStats : MonoBehaviour {
    public Text EnergyText;
    public Text HPText;
    public Text NameText;
    public Text IDText;
    public Text CurrentActivityText;
    public RectTransform ProgressBar;
    public GameObject Background;

    EntityBody lastSelected;
    int hp;
    int maxhp;
    int energy;
    int maxenergy;
    float progress = 1;

	// Use this for initialization
	void Start()
    {
		
	}
	
	// Update is called once per frame
	void Update()
    {
        EntityBody body = null;
        if (GameManager.Instance.SelectionController != null)
            body = GameManager.Instance.SelectionController.Selected;
        if (body)
        {
            if (!Background.activeSelf) Background.SetActive(true);
            if (body != lastSelected)
            {
                UpdateSelection(body);
            }

            if (energy != body.Energy
                || maxenergy != body.MaxEnergy)
                UpdateEnergy(body);

            if (hp != body.Health
                || maxhp != body.MaxHealth)
                UpdateHP(body);

            LivingEntityBody living = body as LivingEntityBody;
            if (living != null)
            {
                if (!ProgressBar.parent.gameObject.activeInHierarchy)
                    ProgressBar.parent.gameObject.SetActive(true);
                UpdateProgress(living);
            }
            else if (ProgressBar.parent.gameObject.activeInHierarchy)
                ProgressBar.parent.gameObject.SetActive(false);
        }
        else if (Background.activeInHierarchy)
            Background.SetActive(false);
	}

    void UpdateSelection(EntityBody body)
    {
        lastSelected = body;
        UpdateHP(body);
        UpdateEnergy(body);
        UpdateName(body);
        UpdateProgress(body as LivingEntityBody);
    }

    void UpdateHP(EntityBody body)
    {
        hp = body.Health;
        maxhp = body.MaxHealth;
        HPText.text = hp.ToString() +
        "/" + maxhp.ToString();
    }

    void UpdateEnergy(EntityBody body)
    {
        energy = body.Energy;
        maxenergy = body.MaxEnergy;
        EnergyText.text = energy.ToString() +
        "/" + maxenergy.ToString();
    }

    void UpdateName(EntityBody body)
    {
        NameText.text = body.Name.ToString();
        IDText.text = body.EntityID.ToString();
    }

    void UpdateProgress(LivingEntityBody body)
    {
        CurrentActivityText.text = "";
        if (body == null)
        {
            // HideProgressBar
        }
        else
        {
            progress = body.Progress;
            if (progress > 1)
                progress = 1;
            if (body.CurrentCommand != null)
                CurrentActivityText.text = body.CurrentCommand.ToString();

            ProgressBar.localScale = new Vector3(progress, 1, 1);
        }
    }

}
