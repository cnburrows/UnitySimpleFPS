using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Actions : MonoBehaviour {
    public bool PlayerCharacter;

    public Camera PlayerCamera;

    public GameObject grenade;

    public GameObject[] actions;

    public GameObject lockHack;

    private GameObject currentAction;
    private GameObject g;
    private AI[] ais;
	PlayerGUI playerGUI;
    // Use this for initialization
    void Start()
    {
        ais = FindObjectsOfType(typeof(AI)) as AI[];
        currentAction = Instantiate(actions[0], transform.position, transform.rotation) as GameObject;
        currentAction.transform.parent = PlayerCamera.transform;
        currentAction.transform.localPosition = new Vector3(currentAction.transform.localPosition.x + 0.25f, currentAction.transform.localPosition.y + 0.7f, currentAction.transform.localPosition.z + 0.3f);
		playerGUI = FindObjectOfType< PlayerGUI > ();
	}

    // Update is called once per frame
    void Update()
    {
		if ((Input.GetButton("Fire1") || Input.GetAxis ("Fire1") < -0.1) && playerGUI.alive)
        {
            currentAction.GetComponent<Weapon>().Shoot();
        }
        if (Input.GetButtonDown("Reload"))
        {
            currentAction.GetComponent<Weapon>().Reload();
        }
        if (Input.GetButtonDown("Interact"))
        {
            RaycastHit hit;
            if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit))
            {
                if (Vector3.Distance(hit.collider.transform.position, transform.position) < 5)
                {
                    Ammo a = hit.collider.gameObject.GetComponent<Ammo>();
                    if (a != null)
                    {
                        currentAction.GetComponent<Weapon>().AddAmmo(a.ammo);
			            if(a.isMissionPickup){
				            GetComponent<PlayerGUI>().Pickup();
			            }
                        a.despawn();
                    }
                    Health h = hit.collider.gameObject.GetComponent<Health>();
                    if (h != null)
                    {
                        GetComponent<PlayerGUI>().AddHealth(h.health);
			            if(h.isMissionPickup){
				            GetComponent<PlayerGUI>().Pickup();
			            }
                        h.despawn();
                    }
                }
            }
        }
        if (Input.GetButtonDown("Grenade"))
        {
            if (g == null)
            {
				PlayerPrefs.SetInt ("Shots", PlayerPrefs.GetInt ("Shots") + 1);
                g = Instantiate(grenade, new Vector3(PlayerCamera.transform.position.x + 0.5f, PlayerCamera.transform.position.y, PlayerCamera.transform.position.z), PlayerCamera.transform.rotation) as GameObject;
                g.GetComponent<Rigidbody>().AddForce(g.transform.forward * 750);
                g.GetComponent<Grenade>().SetAI(ais);
            }
        }
    }
}

