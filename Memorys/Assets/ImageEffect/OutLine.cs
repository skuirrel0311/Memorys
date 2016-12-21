using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLine : CustumImage {

    [SerializeField]
    Color LineColor = Color.black;
    [SerializeField]
    float thickness = 1;

    public override string ShaderName
    {
        get { return "Custum/ImageOutLine"; }
    }
    protected override void UpdateMaterial()
    {
        Material.SetColor("_LineColor", LineColor);
        Material.SetFloat("_Thickness",thickness);
    }
}
