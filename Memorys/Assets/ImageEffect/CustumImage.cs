using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class  CustumImage : MonoBehaviour {

    private Material m_Material;

    public abstract string ShaderName { get;}

    protected Material Material { get { return m_Material; } }


    protected virtual void Awake()
    {
        Shader shader = Shader.Find(ShaderName);
        m_Material = new Material(shader);
    }

    protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        UpdateMaterial();

        Graphics.Blit(source, destination, m_Material);
    }

    protected abstract void UpdateMaterial();
}
