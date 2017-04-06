using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//it's not generic atm, since we only have 1 kind of animal
public class AnimalSpawner : MonoBehaviour {
    public TigerBody AnimalPrefab;

    void Start()
    {
        timer = GameManager.Instance.Config.TigerSpawnTimer;
    }

    float timer;
	void Update ()
    {
        timer -= Time.deltaTime;
        if (timer < 0 && GameManager.Instance.Config.TigersOnMap < GameManager.Instance.Config.TigersLimit)
        {

            TigerBody newChild = Instantiate(AnimalPrefab, transform.position, Quaternion.identity);
            newChild.transform.parent = transform.parent;
            newChild.Female = UnityEngine.Random.value >= 0.5f;
            BrainWrapper newBrain = newChild.gameObject.AddComponent<BrainWrapper>();

            newChild.Speed = GameManager.Instance.Config.TigerSpeed;
            newChild.SneakMastery = GameManager.Instance.Config.TigerSneakMastery;
            newChild.Damage = GameManager.Instance.Config.TigerDamage;
            newChild.AttackSpeed = GameManager.Instance.Config.TigerAttackSpeed;

            timer = GameManager.Instance.Config.TigerSpawnTimer;
        }
	}

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 1 );
        Gizmos.DrawSphere(transform.position, 5);
    }
}
