using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {

    public GameObject explosion;
    public float range = 10;
    public float damage = 50;

    private float t = 5;
    AudioSource audio;
    private bool esplode = false;
    private AI[] ais;

    // Use this for initialization
    void Start () {
        audio = GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
        t -= Time.deltaTime;
        if (t <= 0 && !esplode)
        {
            esplode = true;
            GameObject e;
            e = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            audio.Play();
            GameObject player = GameObject.FindWithTag("Player");
            if (Vector3.Distance(player.transform.position, transform.position) < range)
            { 
                player.GetComponent<PlayerGUI>().Damage(damage);
            }
            foreach (AI ai in ais)
            {
                if(Vector3.Distance(ai.transform.position, transform.position) < range)
                {
					PlayerPrefs.SetInt ("Hits", PlayerPrefs.GetInt ("Hits") + 1);
                    ai.GetComponent<Destructable>().health -= damage;
                }
                ai.Hear(transform);
            }
            Component[] rs = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rs)
            {
                r.enabled = false;
            }
            gameObject.GetComponent<Collider>().enabled = false;
            Destroy(e, 5);
            Destroy(gameObject, 5);
        }
	}
    public void SetAI(AI[] aiArray)
    {
        ais = aiArray;
    }
}
