using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TransitionManager : MonoBehaviour
{
    public static TransitionManager I;

    [SerializeField]
    Image Panel;

    [SerializeField]
    bool FadeInOnAwake = false;

    void Awake()
    {
        I = this;
    }

	// Use this for initialization
	void Start ()
    {
		if(FadeInOnAwake)
        {
            FadeIn();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FadeIn(float duration = 1.0f)
    {
        StartCoroutine(FadeInCoroutine(duration));
    }

    IEnumerator FadeInCoroutine(float duration)
    {
        float timer = 0.0f;

        float delta = (1.0f / duration);

        while (true)
        {
            timer += Time.deltaTime*delta;

            Panel.color = Color.black * (1.0f - timer);

            if(timer>1.0f)
            {
                break;
            }
            yield return null;
        }
    }

    public void FadeOut(float duration = 1.0f)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    IEnumerator FadeOutCoroutine(float duration)
    {
        float timer = 0.0f;
        float delta = (1.0f / duration);

        while (true)
        {
            timer += Time.deltaTime*delta;

            Panel.color = Color.black *timer;

            if (timer > 1.0f)
            {
                break;
            }
            yield return null;
        }
    }

    void OnDestroy()
    {
        I = null;
    }
}
