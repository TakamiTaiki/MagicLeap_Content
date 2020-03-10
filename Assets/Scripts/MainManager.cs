using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using State;
using UnityEngine.Events;
using Utils;

public class MainManager : MonoBehaviour
{
    #region フィールド
    StateProcessor stateProcessor = new StateProcessor();

    [SerializeField] Transform controllerTip;

    public float distance = 10f;

    [SerializeField] Text explainText;

    [SerializeField] RectTransform indicater;

    [SerializeField] GameObject nodeIndicater;

    [SerializeField] GameObject nodeSelectEffect;


    public float shrinkTime = 0.2f;
    public float smallScale = 0.9f;
    public float largeScale = 1.2f;
    public float largeScale_Max = 1.5f;
    public float panelSpeed = 0.07f;

    [SerializeField] Transform panel_UI;

    [SerializeField] AudioSource audio_SE;
    [SerializeField] AudioSource audio_Voice;
    [SerializeField] AudioClip select;
    [SerializeField] AudioClip preselect;
    [SerializeField] AudioClip initVoice;

    private string selectName;

    [Header("城")]
    public GameObject[] castles;
    [Header("板")]
    public GameObject[] planes;

    #region 時代の構造体
    [System.Serializable]
    public struct Node
    {
        [Header("時代")]
        public int era;
        [Header("説明"), TextArea(1, 3)]
        public string explain;
        [Header("ボイス")]
        public AudioClip vc;
        [Header("サウンドエフェクト")]
        public AudioClip se;
        [Header("Nodeのトランスフォーム")]
        public Transform transform;
        [Header("ディゾルブ用のパネル")]
        public Transform panel;
        [Header("パネルのターゲット位置")]
        public Transform destination;
        [Header("アセット")]
        public GameObject[] assets;
        [NonSerialized]
        public int myIndex;
        public UnityEvent process;
    }
    public Node[] nodes;
    [NonSerialized]
    public Node currentNode;
    #endregion

    private Dictionary<string, int> nodeDictionary = new Dictionary<string, int>();

    private bool isFreeChoice = false;

    #endregion

    /**********************************************************************************/

    #region イニシャライズ
    public void Init()
    {
        //ノードが時系列になるようにソート
        Array.Sort(nodes, (a, b) => a.era - b.era);
        //タグと索引の初期化
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].transform.tag = i == 0 ? "NextWaitNode" : "InActiveNode";
            nodes[i].myIndex = i;
            nodeDictionary[nodes[i].transform.name] = nodes[i].myIndex;
        }
        currentNode = nodes[0];

        //パネルUIのイニシャライズ
        PanelInit();
        //初期舞台セット
        Utility.SetStage(currentNode.era, castles, planes);

        //ノードインディケーターを初期位置まで移動
        StartCoroutine(ActiveNextNode(currentNode, panel_UI));

        //初期コンテンツを開始
        stateProcessor.SetState(ST_Start);

        nodeIndicater.SetActive(true);
        nodeSelectEffect.SetActive(false);
    }
    public void PanelInit()
    {
        //パネルUIの位置の初期化
        panel_UI.localPosition = new Vector3(-0.8f, -0.182f, 0.538f);
    }
    #endregion

    void Start()
    {
        Init();
    }

    void Update()
    {
        stateProcessor.Update();
        //説明の赤印の回転
        indicater.Rotate(new Vector3(2f, 0, 0));
    }

    #region スタートステート
    public void ST_Start(bool isFirst)
    {
        //初期処理
        if (isFirst)
        {
            isFreeChoice = false;
        }
        //継続処理
        else
        {
            stateProcessor.SetState(ST_Play);
        }
    }
    #endregion

    #region プレイステート
    public void ST_Play(bool isFirst)
    {
        //初期処理
        if (isFirst)
        {

        }
        //継続処理
        else
        {
        }
    }
    #endregion

    #region フリーチョイスステート
    public void ST_FreeChoice(bool isFirst)
    {
        //初期処理
        if (isFirst)
        {
            isFreeChoice = true;
        }
        //継続処理
        else
        {
            //Rayはコントローラの先端から照射
            Ray ray = new Ray(controllerTip.position, controllerTip.forward);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, distance))
            {
                string tag = hitInfo.collider.tag;

                if (tag == "FreeChoiceNode")
                {
                    Utility.Alignment(nodeSelectEffect, hitInfo.collider.gameObject);
                    if (selectName != hitInfo.collider.gameObject.tag)
                    {
                        Utility.ChangeObjColor(nodeSelectEffect, Color.white);
                        audio_SE.PlayOneShot(preselect);
                    }
                    selectName = hitInfo.collider.gameObject.tag;
                }
            }
        }
    }
    #endregion

    #region 1585年
    public void Event_1585() { StartCoroutine(Process_1585(currentNode)); }
    IEnumerator Process_1585(Node node)
    {
        Utility.SetStage(node.era, castles, planes);
        UpdateAudioAndUI(node);
        yield return PanelLiftDown(node, 0.2f);
        if (!isFreeChoice) UpdateNode();
    }
    #endregion

    #region 1615年
    public void Event_1615() { StartCoroutine(Process_1615(currentNode)); }
    IEnumerator Process_1615(Node node)
    {
        Utility.SetStage(node.era, castles, planes);
        UpdateAudioAndUI(node);
        //着火
        node.assets[0].SetActive(true);
        yield return PanelLiftUp(node, 0.2f);
        //鎮火
        node.assets[0].SetActive(false);
        //城の非アクティブ化
        castles[1].SetActive(false);
        castles[2].SetActive(false);
        if (!isFreeChoice) UpdateNode();
    }
    #endregion

    #region 1626年
    public void Event_1626() { StartCoroutine(Process_1626(currentNode)); }
    IEnumerator Process_1626(Node node)
    {
        Utility.SetStage(node.era, castles, planes);
        UpdateAudioAndUI(node);
        yield return PanelLiftUp(node, 0.2f);
        if (!isFreeChoice) UpdateNode();
    }
    #endregion

    #region 1665年
    public void Event_1665() { StartCoroutine(Process_1665(currentNode)); }
    IEnumerator Process_1665(Node node)
    {
        Utility.SetStage(node.era, castles, planes);
        UpdateAudioAndUI(node);
        node.assets[0].GetComponent<OrbitalBeamLaser>().Start_Thunder();
        yield return new WaitForSeconds(3f);
        node.assets[0].GetComponent<OrbitalBeamLaser>().End_Thunder();
        castles[3].SetActive(false);
        if (!isFreeChoice) UpdateNode();
    }
    #endregion

    #region 1931年
    public void Event_1931() { StartCoroutine(Process_1931(currentNode)); }
    IEnumerator Process_1931(Node node)
    {
        Utility.SetStage(node.era, castles, planes);
        UpdateAudioAndUI(node);
        yield return PanelLiftUp(node, 0.2f);
        if (!isFreeChoice) UpdateNode();
    }
    #endregion

    #region サブルーチン
    IEnumerator PanelLiftUp(Node node, float speed)
    {
        while (node.panel.position.y < node.destination.position.y)
        {
            node.panel.position += new Vector3(0, Time.deltaTime * speed, 0);
            yield return null;
        }
    }

    IEnumerator PanelLiftDown(Node node, float speed)
    {
        while (node.panel.position.y > node.destination.position.y)
        {
            node.panel.position -= new Vector3(0, Time.deltaTime * speed, 0);
            yield return null;
        }
    }

    IEnumerator ActiveNextNode(Node node, Transform panel)
    {
        //ノードインディケーターを縮小化
        yield return Shrink(shrinkTime);
        //移動
        while (panel.position.x < node.transform.position.x)
        {
            var pos = panel.position;
            pos.x += Time.deltaTime * panelSpeed;
            panel.position = pos;
            yield return null;
        }
        //拡大化
        yield return EnLarge();
    }

    IEnumerator Shrink(float interval)
    {
        float startTime = Time.realtimeSinceStartup;
        while (startTime + interval > Time.realtimeSinceStartup)
        {
            nodeIndicater.transform.localScale *= smallScale;
            yield return null;
        }
    }

    IEnumerator EnLarge()
    {
        while (nodeIndicater.transform.localScale.x < largeScale_Max)
        {
            nodeIndicater.transform.localScale *= largeScale;
            yield return null;
        }
        nodeIndicater.transform.localScale = Vector3.one * largeScale_Max;
    }
    #endregion

    #region コントローラ関連
    /// <summary>
    /// マジックリープのトリガーが引かれた瞬間に処理がされる
    /// </summary>
    public void ML_OnTriggerDown()
    {
        //Rayはコントローラの先端から照射
        Ray ray = new Ray(controllerTip.position, controllerTip.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            string tag = hitInfo.collider.tag;

            if (tag == "NextWaitNode")
            {
                //各ノードに割り振られた処理を開始
                currentNode.process.Invoke();
                //ノードを変えることによる選択できないようにする
                currentNode.transform.tag = "ActiveNode";
            }
            else if (tag == "ActiveNode")
            {
                //何かしらの処理があれば
            }
            else if (tag == "InActiveNode")
            {
                //何かしらの処理があれば
            }
            else if (tag == "FreeChoiceNode")
            {
                //決定のSEを流す
                audio_SE.PlayOneShot(select);
                //セレクトエフェクトのカラーを決定色(Green)にする
                Utility.ChangeObjColor(nodeSelectEffect, Color.green);
                //選択したオブジェクトからノードを検索してcurrentNodeに設定
                currentNode = nodes[nodeDictionary[hitInfo.collider.transform.name]];
                currentNode.process.Invoke();
            }
            else if (tag == "Init")
            {
                audio_Voice.PlayOneShot(initVoice);
                Init();
            }
        }
    }
    #endregion

    #region ノードのアップデート関連
    /// <summary>
    /// 連結されているノードに更新する
    /// 最終ノードの場合はデストラクト処理
    /// </summary>
    public void UpdateNode()
    {
        int nextIndex = currentNode.myIndex + 1;
        if (nextIndex < nodes.Length)
        {
            currentNode = nodes[nextIndex];
            currentNode.transform.tag = "NextWaitNode";
            //ノードインディケーターの移動
            StartCoroutine(ActiveNextNode(currentNode, panel_UI));
        }
        //最後のノードのイベントが終わったときはフリー選択モードとなる
        else
        {
            //パネルずらし
            panel_UI.Translate(new Vector3(1f, 0, 0));
            FreeChoiceAvtivate();
        }
    }

    /// <summary>
    /// オーディと説明テキストのアップデート
    /// </summary>
    /// <param name="node"></param>
    public void UpdateAudioAndUI(Node node)
    {
        audio_SE.PlayOneShot(node.se);
        audio_Voice.PlayOneShot(node.vc);
        explainText.text = node.explain;
    }
    #endregion

    #region その他の関数
    /// <summary>
    /// ストーリーモードからフリーチョイスモードへ
    /// </summary>
    public void FreeChoiceAvtivate()
    {
        //セレクトエフェクトのアクティブ化
        nodeSelectEffect.SetActive(true);
        //インディケーターは非アクティブ化
        nodeIndicater.SetActive(false);
        //アップデートモードの変更
        stateProcessor.SetState(ST_FreeChoice);
        //タグを変える
        for (int i = 0; i < nodes.Length; i++)
            nodes[i].transform.tag = "FreeChoiceNode";
    }
    #endregion
}
