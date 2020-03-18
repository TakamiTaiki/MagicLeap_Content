using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class DynamicBeam : MonoBehaviour
{
    [SerializeField, TooltipAttribute("MLのコントローラーオブジェクトをアタッチ")]
    private GameObject controller;
    [Header("Color")]
    [SerializeField, TooltipAttribute("レイの始まりの色を設定")]
    private Color startColor;
    [SerializeField, TooltipAttribute("レイの終わりの色を設定")]
    private Color endColor;

    // 可視化するためのレンダラー
    private LineRenderer lineRenderer;


    // Start is called before the first frame update
    void Start()
    {
        // 自分自身のLineRendererコンポーネントを取得して色を設定
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
    }

    // Update is called once per frame
    void Update()
    {
        // コントローラーの位置と角度を取得
        transform.position = controller.transform.position;
        transform.rotation = controller.transform.rotation;

        //コントローラーから正面に向かってレイを飛ばす
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            // ワールド座標系でレンダリング
            // コントローラーの位置からレイがぶつかった場所までレンダリング
            lineRenderer.useWorldSpace = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            // ローカル座標系でレンダリング
            // コントローラーの位置から所定の距離までレンダリング
            //lineRenderer.useWorldSpace = false;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.forward * 5);
        }
    }
}