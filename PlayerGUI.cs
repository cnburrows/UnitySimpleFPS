using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerGUI : MonoBehaviour {

    public Texture crosshair;
    public float totalHealth = 100;
    public float totalPickups = 0;

    float barDisplay = 1;
    float barDisplayPickups = 1;
    Vector2 pos1 = new Vector2(Screen.width/4,Screen.height/1.1f);
    Vector2 pos2 = new Vector2(Screen.width/4,Screen.height/1.2f);
    Vector2 size = new Vector2(Screen.width / 2, Screen.height / 20);
    Texture2D progressBarEmpty;
    GUIStyle progressBarEmptyStyle;
    Texture2D progressBarFull;
    GUIStyle ammoCounterStyle;

    private int magazine = 0;
    private int ammo = 0;
    public bool alive = true;
    private Color hitBackground = Color.clear;
    private float t = 0;
    private bool fadeOut = false;
    private bool fadeIn = false;
    public bool paused = false;

    void OnGUI() {
        if (!paused)
        {
            pos1.x = Screen.width / 4;
            pos1.y = Screen.height / 1.1f;
            pos2.x = Screen.width / 4;
            pos2.y = Screen.height / 1.2f;
            size.x = Screen.width / 2;
            size.y = Screen.height / 20;
            if (progressBarFull == null)
            {
                progressBarFull = new Texture2D(1, 1);
            }
            if (progressBarEmptyStyle == null)
            {
                progressBarEmptyStyle = new GUIStyle();
            }
            if (ammoCounterStyle == null)
            {
                ammoCounterStyle = new GUIStyle();
            }
            progressBarFull.SetPixel(0, 0, Color.red);
            progressBarFull.Apply();

            progressBarEmptyStyle.normal.background = progressBarFull;

            ammoCounterStyle.fontSize = 20;
            ammoCounterStyle.richText = true;

            //Draw the background part of the health bar
            GUI.BeginGroup(new Rect(pos1.x, pos1.y, size.x, size.y));
            GUI.Box(new Rect(0, 0, size.x, size.y), progressBarEmpty);
            //Draw the filled in part of the health bar
            GUI.BeginGroup(new Rect(1, 1, size.x * barDisplay - 2, size.y - 2));
            GUI.Box(new Rect(0, 0, size.x, size.y), GUIContent.none, progressBarEmptyStyle);
            GUI.EndGroup();
            GUI.EndGroup();
            if (totalPickups > 0)
            {
                //Change color for the # of mission objects picked up
                progressBarFull.SetPixel(0, 0, Color.blue);
                progressBarFull.Apply();
                //Draw the background part of the pickup counter bar
                GUI.BeginGroup(new Rect(pos2.x, pos2.y, size.x, size.y));
                GUI.Box(new Rect(0, 0, size.x, size.y), progressBarEmpty);
                //Draw the filled in part of the pickup counter bar
                GUI.BeginGroup(new Rect(1, 1, size.x * barDisplayPickups - 2, size.y - 2));
                GUI.Box(new Rect(0, 0, size.x, size.y), GUIContent.none, progressBarEmptyStyle);
                GUI.EndGroup();
                GUI.EndGroup();
            }
            //Draw ammo counter
            GUI.Label(new Rect(Screen.width / 16, Screen.height / 1.1f, 100, 20), "<color=white><b>" + ammo + " /  " + magazine + "</b></color>", ammoCounterStyle);
            //Overlay red blood screen if damage taken
            progressBarFull.SetPixel(0, 0, hitBackground);
            progressBarFull.Apply();
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none, progressBarEmptyStyle);
            //Draw in crosshair
            GUI.DrawTexture(new Rect((Screen.width / 2) - 30, (Screen.height / 2) - 30, 60, 60), crosshair, ScaleMode.ScaleToFit, true, 0);
        }
    }
    public void AddHealth(int health){
        float f = barDisplay + ((health * 1.0f) / totalHealth);
        if(f < 1.0f)
        {
            barDisplay = f;
        }
        else
        {
            barDisplay = 1.0f;
        }
    }
    public void Pickup(){
	    if(totalPickups > 0){
		    float f = ((totalPickups * barDisplayPickups) - 1f) / totalPickups;
		    if(f > 0f)
		    {
		        barDisplayPickups = f;
		    }
		    else
		    {
		        barDisplayPickups = 0f;
		    }
	    }
    }
    public void Damage(float damage)
    {
	PlayerPrefs.SetInt ("Damage", PlayerPrefs.GetInt ("Damage") + 1);
        barDisplay = barDisplay - (damage / totalHealth);
        if (!fadeIn)
        {
            fadeOut = true;
        }
    }
    public void Ammo(int ammunition, int mag)
    {
        ammo = ammunition;
        magazine = mag;
    }
    void Update()
    {
        if (barDisplay <= 0 && alive)
        {
            GetComponent<Actions>().enabled = false;
            GetComponent<FirstPersonController>().enabled = false;
            alive = false;
            fadeOut = true;

        }
        if (fadeOut)
        {
            hitBackground = Color.Lerp(Color.clear, Color.red, t);
            t += Time.deltaTime*2;
            if (t >= 1)
            {
                fadeOut = false;
                t = 1;
                fadeIn = true;
            }
        }
        if (fadeIn && alive)
        {
            hitBackground = Color.Lerp(Color.clear, Color.red, t);
            t -= Time.deltaTime*2;
            if(t <= 0)
            {
                fadeIn = false;
                t = 0;
            }
        }
        if (Input.GetButtonDown("Cancel")) {
            //foward to next questionnaire
            //Application.OpenURL("https://tamu.qualtrics.com/jfe/form/SV_abK1vb7SEOiRhJP?MTurkID=" + PlayerPrefs.GetString("PlayerID"));
        }
        if (!alive && Input.anyKeyDown)
        {
			PlayerPrefs.SetInt ("Deaths", PlayerPrefs.GetInt ("Deaths") + 1);
            barDisplay = 1f;
            GetComponent<Actions>().enabled = true;
            GetComponent<FirstPersonController>().enabled = true;
            alive = true;
        }
    }
}
