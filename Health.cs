using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    public int health;
    public float respawnTime = 5;
    public bool isMissionPickup = false;
    private bool alive = true;
    private float oldRespawnTime;

    // Use this for initialization
    void Start()
    {
        oldRespawnTime = respawnTime;

    }

    // Update is called once per frame
    void Update()
    {
        if (!alive)
        {
            respawnTime -= Time.deltaTime;
            if (respawnTime <= 0)
            {
                gameObject.GetComponent<Renderer>().enabled = true;
                gameObject.GetComponent<Collider>().enabled = true;
                alive = true;
                respawnTime = oldRespawnTime;
            }
        }

    }
    public void despawn()
    {
        alive = false;
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
    }
}
