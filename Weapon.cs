using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    //transform to mark the projectile exit point
    public GameObject gunMuzzle;
    //angle of the gun's kick back
    public float kickAngle;
    //prefab for the muzzle flash
    public GameObject muzzleExplosion;
    //use a projectile or use raycasting if no projectile prefab assigned
    public GameObject ballisticProjectile;
    //if a ballistic projectile is designated, make it have a different speed
    public float launchForce = 2000.0f;
    //add a constant force on the projectile during flight
    public float propulsionForce;
    //so that it looks like it hit something 
    public GameObject projectileExplosion;
    //power of the explosive force of the projectile
    public int explosiveForce;
    //delay of the explosion from the projectile
    public int explosiveDelay;

    public float damage = 20;

    public int magazineSize = 30;

    public int ammunitionTotal = 100;

    public bool forAI;

    public AudioClip shot;

    public AudioClip reload;

    public AudioClip dryFire;

    private float recoil = 0;
    private bool shooting = false;
    private float timer = 0f;

    AI[] ais;
    PlayerGUI playerGUI;
    int magazine;
    AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        magazine = magazineSize;
        ais = FindObjectsOfType(typeof(AI)) as AI[];
        playerGUI = FindObjectOfType< PlayerGUI > ();
        playerGUI.Ammo(ammunitionTotal, magazine);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (shooting && timer > 0.15f && !forAI)
        {
            Fire();
            timer = 0f;
			shooting = false;
        }
        if(shooting && forAI)
        {
            Fire();
            shooting = false;
        }
        if (recoil > 0 || recoil < 0)
        {
            Recoil();
        }
    }

    public void Shoot()
    {
        if (shooting)
        {
            shooting = false;
        }
        else
        {
            shooting = true;
        }
    }
    public void Fire()
    {
        if (magazine > 0)
        {
            audio.PlayOneShot(shot);
            GameObject muzzleFlash = Instantiate(muzzleExplosion, gunMuzzle.transform.position, Quaternion.identity) as GameObject;
            muzzleFlash.transform.parent = this.transform;
            Destroy(muzzleFlash, 1);
            if (!forAI)
            {
				PlayerPrefs.SetInt ("Shots", PlayerPrefs.GetInt ("Shots") + 1);
                magazine -= 1;
                playerGUI.Ammo(ammunitionTotal, magazine);
                foreach (AI ai in ais)
                {
                    ai.Hear(transform);
                }
                RaycastHit hit;

                float xAxis = 0.5f;
                float yAxis = 0.5f;
                if (PlayerPrefs.GetInt("Frustration_Condition") == 2)
                {
                    xAxis = Random.Range(0.45f, 0.55f);
                    yAxis = Random.Range(0.45f, 0.55f);
                }
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(xAxis, yAxis, 0f)), out hit))
                {
                    Destructable d = hit.collider.gameObject.GetComponent<Destructable>();
                    if (d != null)
                    {
						PlayerPrefs.SetInt ("Hits", PlayerPrefs.GetInt ("Hits") + 1);
                        d.health -= damage;
                    }
                    if (projectileExplosion != null)
                    {
                        GameObject bulletExplosion = Instantiate(projectileExplosion, hit.point, Quaternion.identity) as GameObject;
                        Destroy(bulletExplosion, 0.5f);
                    }
                }
            }
            if (!forAI)
            {
                recoil = 0.1f;
            }
        }
        else
        {
            audio.PlayOneShot(dryFire);
            shooting = false;
        }
    }
    public void Reload()
    {
        if (ammunitionTotal - (magazineSize - magazine) < 0) {
            audio.PlayOneShot(reload);
            magazine += ammunitionTotal;
            ammunitionTotal = 0;
            playerGUI.Ammo(ammunitionTotal, magazine);
        }
        else {
            audio.PlayOneShot(reload);
            ammunitionTotal -= (magazineSize - magazine);
            magazine = magazineSize;
            playerGUI.Ammo(ammunitionTotal, magazine);
        }
    }
    public void AddAmmo(int ammo)
    {
        ammunitionTotal += ammo;
        playerGUI.Ammo(ammunitionTotal, magazine);
    }
    void Recoil()
    {
        if (recoil > 0)
        {
            recoil -= Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x- kickAngle, transform.eulerAngles.y, transform.eulerAngles.z), Time.deltaTime * 10);
            if(recoil <= 0)
            {
                recoil = -1;
            }
        }
        else if (recoil < 0)
        {
            recoil += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.parent.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z), Time.deltaTime * 10);
            if (recoil >= 0)
            {
                recoil = 0;
            }
        }
    }

    // how to actually find out of it hits something 
    /*void OnCollisionEnter(Collision c)
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        // destroy the rocket because of note below that I found online because it's a bit confusing
        // note: Destroy(this) would just destroy the rocket script attached to it Destroy(gameObject) destroys the whole thing
        Destroy(gameObject);
    }*/
}