using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using State;
using UnityEngine.Events;

public class MainManager : MonoBehaviour
{
    State.StateProcessor stateProcessor = new StateProcessor();

    [SerializeField] Transform controllerTip;
    public float distance = 10f;
    [SerializeField] Text explainText;

    [SerializeField] RectTransform indicater;

    [SerializeField] GameObject nodeIndicater;

    [SerializeField] GameObject nodeSelectEffect;

    public float smallTime = 0.2f;

    private int dissolveStage = 0;
    public int DissolveStage
    {
        get { return this.dissolveStage; }
        set
        {
            //StartCoroutine(ActiveNextNode(dissolveStage));
            this.dissolveStage = value;
        }
    }

    [SerializeField] Transform panel_UI;

    [SerializeField] GameObject catle1;
    [SerializeField] GameObject catle2;
    [SerializeField] GameObject catle3;
    [SerializeField] GameObject catle4;

    public float[] speedParams;

    [SerializeField] Renderer nonActive, invisible;

    [SerializeField] AudioSource audio_SE;
    [SerializeField] AudioSource audio_Voice;
    [SerializeField] AudioClip select;
    [SerializeField] AudioClip preselect;
    [SerializeField] AudioClip initVoice;

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

    void Start()
    {
        //ノードが時系列になるようにソート
        Array.Sort(nodes, (a, b) => a.era - b.era);
        //タグと索引の初期化
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].transform.tag = i == 0 ? "NextWaitNode" : "InActiveNode";
            nodes[i].myIndex = i;
        }

        currentNode = nodes[0];

        //初期コンテンツを開始
        stateProcessor.SetState(ST_Start);
    }

    void Update()
    {
        stateProcessor.Update();
    }

    public void ST_Start(bool isFirst)
    {
        if (isFirst)
        {

        }
        else
        {
        }
    }

    #region 1585年
    public void Event_1585() { StartCoroutine(Process_1585(currentNode)); }
    IEnumerator Process_1585(Node node)
    {
        UpdateAudioAndUI(node);
        while (node.panel.position.y > node.destination.position.y)
        {
            node.panel.position -= new Vector3(0, Time.deltaTime * 0.2f, 0);
            yield return null;
        }
        //モダンお城の非アクティブ化
        node.assets[0].SetActive(false);
        UpdateNode();
    }
    #endregion

    #region 1615年
    public void Event_1615() { StartCoroutine(Process_1615(currentNode)); }
    IEnumerator Process_1615(Node node)
    {
        UpdateAudioAndUI(node);
        //着火
        node.assets[0].SetActive(true);
        while (node.panel.position.y < node.destination.position.y)
        {
            node.panel.position += new Vector3(0, Time.deltaTime * 0.2f, 0);
            yield return null;
        }
        //鎮火
        node.assets[0].SetActive(false);
        //城の非アクティブ化
        node.assets[1].SetActive(false);
        node.assets[2].SetActive(false);
        UpdateNode();
    }
    #endregion

    #region 1626年
    public void Event_1626() { StartCoroutine(Process_1626(currentNode)); }
    IEnumerator Process_1626(Node node)
    {
        UpdateAudioAndUI(node);
        while (node.panel.position.y < node.destination.position.y)
        {
            node.panel.position += new Vector3(0, Time.deltaTime * 0.2f, 0);
            yield return null;
        }
        UpdateNode();
    }
    #endregion

    #region 1665年
    public void Event_1665() { StartCoroutine(Process_1665(currentNode)); }
    IEnumerator Process_1665(Node node)
    {
        UpdateAudioAndUI(node);
        node.assets[0].GetComponent<OrbitalBeamLaser>().AAA();
        yield return new WaitForSeconds(3f);
        node.assets[0].GetComponent<OrbitalBeamLaser>().BBB();
        node.assets[1].SetActive(false);
        UpdateNode();
    }
    #endregion

    #region 1931年
    public void Event_1931() { StartCoroutine(Process_1931(currentNode)); }
    IEnumerator Process_1931(Node node)
    {
        UpdateAudioAndUI(node);
        //パネルを戻す、お城のアクティブ化
        node.panel.transform.position = new Vector3(-0.018f, -0.628f, 1.568f);
        node.assets[0].SetActive(true);
        while (node.panel.position.y < node.destination.position.y)
        {
            node.panel.position += new Vector3(0, Time.deltaTime * 0.2f, 0);
            yield return null;
        }
        UpdateNode();
    }
    #endregion

    IEnumerator NodeRiftUp(Node node, float speed)
    {
        while (node.panel.position.y < node.destination.position.y)
        {
            node.panel.position += new Vector3(0, Time.deltaTime * speed, 0);
            yield return null;
        }
    }

    IEnumerator NodeRiftDown(Node node, float speed)
    {
        while (node.panel.position.y > node.destination.position.y)
        {
            node.panel.position -= new Vector3(0, Time.deltaTime * speed, 0);
            yield return null;
        }
    }

    IEnumerator ActiveNextNode(int i)
    {
        yield return null;
    }

    IEnumerator BeSmall(float interval)
    {
        float startTime = Time.realtimeSinceStartup;
        while (startTime + interval > Time.realtimeSinceStartup)
        {
            nodeIndicater.transform.localScale *= 0.9f;
            yield return null;
        }
    }

    IEnumerator EnLarge()
    {
        while (nodeIndicater.transform.localScale.x < 1.5f)
        {
            nodeIndicater.transform.localScale *= 1.2f;
            yield return null;
        }
    }

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
                currentNode.process.Invoke();
                currentNode.transform.tag = "ActiveNode";
            }
            else if (tag == "ActiveNode")
            {
                //何かしらの処理
            }
            else if (tag == "InActiveNode")
            {
                //何かしらの処理
            }
        }
    }

    public void UpdateNode()
    {
        int nextIndex = currentNode.myIndex + 1;
        if (nextIndex < nodes.Length)
        {
            nodes[nextIndex].transform.tag = "NextWaitNode";
            currentNode = nodes[nextIndex];
        }
    }

    public void UpdateAudioAndUI(Node node)
    {
        audio_SE.PlayOneShot(node.se);
        audio_Voice.PlayOneShot(node.vc);
        explainText.text = node.explain;
    }

    public void MLPJInit()
    {
        Ray ray = new Ray(controllerTip.position, controllerTip.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            string tag = hitInfo.collider.tag;

            if (tag == "Init")
            {
                audio_Voice.PlayOneShot(initVoice);
                dissolveStage = 0;
                //Init();
            }

        }
    }

    public void PanelInit()
    {

    }
}
