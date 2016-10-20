using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class CameraContoller : MonoBehaviour
{
    [SerializeField]
    Transform targetObject = null;
    [SerializeField]
    float distance = 7.0f;    //カメラとターゲットの距離
    [SerializeField]
    float rotationSpeedX = 150.0f;
    [SerializeField]
    float rotationSpeedY = 100.0f;

    /// <summary>
    /// 緯度
    /// </summary>
    [SerializeField]
    float latitude = 15.0f;
    /// <summary>
    /// 経度
    /// </summary>
    [SerializeField]
    float longitude = 180.0f;

    /// <summary>
    /// 緯度の最大値
    /// </summary>
    [SerializeField]
    float maxLatitude = 89.0f;
    /// <summary>
    /// 緯度の最小値
    /// </summary>
    [SerializeField]
    float minLatitude = -85.0f;
    /// <summary>
    /// Slerpを開始する緯度の値
    /// </summary>
    [SerializeField]
    float startSlerpLatitude = 30.0f;

    //カメラとプレイヤーの間にあるオブジェクト
    List<GameObject> lineHitObjects = new List<GameObject>();

    //カメラを制御するか？
    public bool IsWork = true;

    void Start()
    {
        //ターゲットが入ってなかったらプレイヤーを探す
        if (targetObject == null)
        {
            targetObject = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if(targetObject == null)
        {
            targetObject = GameObject.Find("Player").transform;
        }
    }

    void Update()
    {
        if (targetObject == null) return;

        BetweenPlayerAndCamera();
        if (!IsWork) return;
        //右スティックで回転
        Vector2 rightStick = MyInputManager.GetAxis(MyInputManager.Axis.RightStick);
        longitude += rightStick.x * rotationSpeedX * Time.deltaTime;
        // - はお好み
        latitude -= rightStick.y * rotationSpeedY * Time.deltaTime;

        //経度には制限を掛ける
        latitude = Mathf.Clamp(latitude, minLatitude, maxLatitude);
        longitude = longitude % 360.0f;
        
        SphereCameraControl();
    }

    void SphereCameraControl()
    {
        if (latitude < startSlerpLatitude)
        {
            //リープ開始
            Vector3 vec1 = SphereCoordinate(longitude, startSlerpLatitude, distance);
            //リープ終了時の座標
            Vector3 vec2 = SphereCoordinate(longitude, minLatitude, distance);
            vec2.y = 0.0f;
            
            float t;
            //(開始位置からの移動量) / (全体の移動量) = 0 ～ 1
            if (latitude >= 0.0f)
                t = (startSlerpLatitude - latitude) / (-minLatitude + startSlerpLatitude);
            else
                t = ((-latitude) + startSlerpLatitude) / (-minLatitude + startSlerpLatitude);
            
            transform.position = targetObject.position + Vector3.Slerp(vec1, vec2, t);
        }
        else
        {
            //カメラが地面にめり込まない場合は球体座標をそのまま使う
            transform.position = targetObject.position + SphereCoordinate(longitude, latitude, distance);
        }

        transform.LookAt(targetObject.position + (Vector3.up * 2.0f));
    }

    /// <summary>
    /// 指定した角度の球体座標を返します
    /// </summary>
    /// <param name="longitude">経度</param>
    /// <param name="latitude">緯度</param>
    /// <returns></returns>
    public static Vector3 SphereCoordinate(float longitude, float latitude, float distance)
    {
        Vector3 temp = Vector3.zero;

        //重複した計算
        float deg2Rad = Mathf.Deg2Rad;
        float t = distance * Mathf.Cos(latitude * deg2Rad);

        temp.x = t * Mathf.Sin(longitude * deg2Rad);
        temp.y = distance * Mathf.Sin(latitude * deg2Rad);
        temp.z = t * Mathf.Cos(longitude * deg2Rad);

        return temp;
    }

    /// <summary>
    /// プレイヤーとカメラの間にオブジェクトがあったら非表示にします
    /// </summary>
    void BetweenPlayerAndCamera()
    {
        Vector3 direction = (targetObject.position + Vector3.up) - transform.position;
        Ray ray = new Ray(transform.position, direction);


        //rayにあたったオブジェクトをリストに格納
        List<GameObject> hitList = Physics.RaycastAll(ray, direction.magnitude).Select(n => n.transform.gameObject).ToList();

        if (hitList.Count == 0) return;

        //containsでlinehitに無くてtagがBoxのものを判定しwhereで無かったものをlistに格納
        lineHitObjects.AddRange(hitList.Where(n => (!lineHitObjects.Contains(n)) && (n.tag == "Wall")));

        //半透明にする
        foreach (GameObject n in lineHitObjects)
        {
            string parentName = n.transform.parent.name;
            SetAlpha(n, 0.3f);
        }

        //今回ヒットしなかったものは透明度をリセットし、リムーブする。
        lineHitObjects.RemoveAll(n =>
        {
            if (hitList.Contains(n)) return false;

            string parentName = n.transform.parent.name;
            ResetAlpha(n);
            return true;
        });
    }

    void SetAlpha(GameObject obj, float alpha)
    {
        Material mat = obj.GetComponent<Renderer>().material;
        Color color = mat.color;
        mat.SetFloat("_Mode", 2.0f);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        mat.color = new Color(color.r, color.g, color.b, alpha);
    }

    void ResetAlpha(GameObject obj)
    {
        Material mat = obj.GetComponent<Renderer>().material;
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1;
    }

    public IEnumerator SeeFellPlayer()
    {
        IsWork = false;
        float time = 0;
        while(time < 1.0f)
        {
            transform.LookAt(targetObject.transform);
            time += Time.deltaTime;
            yield return null;
        }
        IsWork = true;
        if (targetObject == null) yield return null;
        PlayerOverlap overlap = targetObject.GetComponent<PlayerOverlap>();

        //Hp分のダメージを与える=即死
        overlap.Damage(overlap.HP);
    }
}
