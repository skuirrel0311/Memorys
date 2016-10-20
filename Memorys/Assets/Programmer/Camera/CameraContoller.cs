using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CameraContoller : MonoBehaviour
{
    [SerializeField]
    GameObject player = null;
    [SerializeField]
    float distance = 7;    //カメラとプレイヤーの距離
    [SerializeField]
    float rotationSpeedX = 150;
    [SerializeField]
    float rotationSpeedY = 100;

    PlayerController controller;

    /// <summary>
    /// 緯度
    /// </summary>
    [SerializeField]
    float latitude = 15;
    /// <summary>
    /// 経度
    /// </summary>
    [SerializeField]
    float longitude = 180;

    //カメラとプレイヤーの間にあるオブジェクト
    List<GameObject> lineHitObjects = new List<GameObject>();

    void Start()
    {
        controller = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        BetweenPlayerAndCamera();

        Vector2 rightStick = MyInputManager.GetAxis(MyInputManager.Axis.RightStick);
        longitude += rightStick.x * rotationSpeedX * Time.deltaTime;
        latitude -= rightStick.y * rotationSpeedY * Time.deltaTime;
        //経度には制限を掛ける
        latitude = Mathf.Clamp(latitude, 10, 80);

        transform.position = player.transform.position + SphereCoordinate(longitude, latitude,distance);
        transform.LookAt(player.transform.position + Vector3.up * 2);
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
        //TwinWall,TwinScaffold,LongScaffold,lowScaffold

        Vector3 direction = (player.transform.position + Vector3.up) - transform.position;
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
        mat.SetFloat("_Mode", 2);
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
}
