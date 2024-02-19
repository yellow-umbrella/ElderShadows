using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fade : MonoBehaviour
{
    public float fadeTime;
    private TextMeshProUGUI fadeText;
    // Start is called before the first frame update
    void Start()
    {
        fadeText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( fadeTime > 0 ) 
        {
            fadeTime -= Time.deltaTime;
            fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, fadeTime);
        }
        
    }
}
