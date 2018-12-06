using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;

public class Destructable : MonoBehaviour {

    public float health = 100;
    public float respawnDelay = 30;
    private bool dead = false;
    private Vector3 startLocation;
    private float respawnTimer = 0;

	// Use this for initialization
	void Start () {
        startLocation = transform.position;
	}

    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && !dead)
        {
			PlayerPrefs.SetInt ("Kills", PlayerPrefs.GetInt ("Kills") + 1);
            dead = true;
            respawnTimer = respawnDelay;
            AI a = gameObject.GetComponent<AI>();
            if (a != null)
            {
                a.setAlive(false);
                GetComponent<Animator>().SetTrigger("Death");
                GetComponent<AICharacterControl>().SetTarget(transform);
            }
        }
        if (dead && respawnTimer > 0)
        {
            respawnTimer -= Time.deltaTime;
        }
        if (dead && respawnTimer <= 0)
        {
            dead = false;
            health = 100;
            transform.position = startLocation;
            AI a = gameObject.GetComponent<AI>();
            if (a != null)
            {
                a.setAlive(true);
                GetComponent<Animator>().ResetTrigger("Death");
                GetComponent<AICharacterControl>().SetTarget(transform);
            }
        }
    }
}
