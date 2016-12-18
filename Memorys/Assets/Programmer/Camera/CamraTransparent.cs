using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamraTransparent : MonoBehaviour
{
    private struct LineHitObject
    {
        public GameObject gameObject;
        public Coroutine coroutine;
        public bool IsEndSetAlpha;
    }

    //カメラとプレイヤーの間にあるオブジェクト
    List<LineHitObject> lineHitObjectList = new List<LineHitObject>();
    List<LineHitObject> removeObjectList = new List<LineHitObject>();

    CameraContoller contoller;

    [SerializeField]
    LayerMask objectLayerMask = 0;

    [SerializeField]
    string[] ignoreTagList = null;

    [SerializeField]
    Vector3 targetOffset = Vector3.up;

    void Start()
    {
        contoller = GetComponent<CameraContoller>();
    }

    void Update()
    {
        BetweenPlayerAndCamera();
    }

    /// <summary>
    /// プレイヤーとカメラの間にオブジェクトがあったら非表示にします
    /// </summary>
    void BetweenPlayerAndCamera()
    {
        Vector3 direction = (contoller.targetObject.position + targetOffset) - transform.position;
        Ray ray = new Ray(transform.position, direction);

        //rayにあたったオブジェクトをリストに格納
        RaycastHit[] hitArray = Physics.RaycastAll(ray, direction.magnitude, objectLayerMask);
        List<GameObject> hitList = new List<GameObject>();

        for (int i = 0; i < hitArray.Length; i++)
        {
            hitList.Add(hitArray[i].transform.gameObject);
        }

        if (hitList.Count == 0)
        {
            if (lineHitObjectList.Count != 0)
            {
                for (int i = 0; i < lineHitObjectList.Count; i++)
                {
                    StopCoroutine(lineHitObjectList[i].coroutine);
                    LineHitObject obj = new LineHitObject();
                    obj.gameObject = lineHitObjectList[i].gameObject;
                    obj.coroutine = StartCoroutine(ResetAlpha(obj));
                    removeObjectList.Add(obj);
                }
                lineHitObjectList.Clear();
            }
            return;
        }

        for (int i = 0; i < hitList.Count; i++)
        {
            //既に追加されている
            if (IsExistInHitList(hitList[i])) continue;

            //タグが指定されたものでない
            if (IsIgnoreTag(hitList[i].tag)) continue;

            LineHitObject obj = GetObjectInRemoveList(hitList[i]);
            if(obj.coroutine != null)
            {
                StopCoroutine(obj.coroutine);
            }
            
            obj.gameObject = hitList[i];
            obj.coroutine = StartCoroutine(SetAlpha(obj, 0.3f));
            lineHitObjectList.Add(obj);
        }

        for (int i = lineHitObjectList.Count - 1; i > 0; i--)
        {
            if (!lineHitObjectList[i].IsEndSetAlpha)
            {
                StopCoroutine(lineHitObjectList[i].coroutine);
            }
            LineHitObject obj = new LineHitObject();
            obj.gameObject = lineHitObjectList[i].gameObject;
            obj.coroutine = StartCoroutine(ResetAlpha(obj));
            removeObjectList.Add(obj);

            lineHitObjectList.Remove(lineHitObjectList[i]);
        }
    }

    IEnumerator SetAlpha(LineHitObject obj, float alpha)
    {
        Material mat = obj.gameObject.GetComponent<Renderer>().material;
        Color color = mat.color;
        mat.SetFloat("_Mode", 2.0f);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        float t = 0.0f;
        float limitTime = 1.0f;
        while (true)
        {
            t += Time.deltaTime;
            mat.color = new Color(color.r, color.g, color.b, FloatLerp(color.a, alpha, (t / limitTime)));
            if (t > limitTime) break;
            yield return null;
        }

        obj.IsEndSetAlpha = true;
    }

    IEnumerator ResetAlpha(LineHitObject obj)
    {
        Material mat = obj.gameObject.GetComponent<Renderer>().material;
        Color color = mat.color;

        float t = 0.0f;
        float limitTime = 1.0f;
        while (true)
        {
            t += Time.deltaTime;
            mat.color = new Color(color.r, color.g, color.b, FloatLerp(color.a, 1.0f, (t / limitTime)));
            if (t > limitTime) break;
            yield return null;
        }

        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1;

        obj.IsEndSetAlpha = true;
    }

    bool IsIgnoreTag(string tag)
    {
        //リストがないということは無視するタグではないということ
        if (ignoreTagList == null) return false;

        for (int i = 0; i < ignoreTagList.Length; i++)
        {
            if (tag == ignoreTagList[i]) return true;
        }

        return false;
    }

    float FloatLerp(float a, float b, float t)
    {
        //tは0～1のはず
        return a + ((b - a) * t);
    }

    //オブジェクトがリストに存在するか
    bool IsExistInHitList(GameObject obj)
    {
        for(int i = 0;i< lineHitObjectList.Count;i++)
        {
            if (lineHitObjectList[i].gameObject.Equals(obj)) return true;
        }
        return false;
    }
    
    LineHitObject GetObjectInRemoveList(GameObject obj)
    {
        for (int i = 0; i < removeObjectList.Count; i++)
        {
            if (removeObjectList[i].gameObject.Equals(obj)) return removeObjectList[i];
        }
        return new LineHitObject();
    }
}
