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
    [SerializeField] Transform[] oldNodes;

    [SerializeField] Text explainText;

    [SerializeField] RectTransform indicater;

    [SerializeField] GameObject nodeIndicater;

    [SerializeField] GameObject nodeSelectEffect;

    [SerializeField] GameObject[] targets;

    [SerializeField] GameObject fire;

    string tmp;

    public float smallTime = 0.2f;

    private int dissolveStage = 0;
    public int DissolveStage
    {
        get { return this.dissolveStage; }
        set
        {
            state = oldState.move;
            //StartCoroutine(ActiveNextNode(dissolveStage));
            this.dissolveStage = value;
        }
    }

    [SerializeField] Transform dissolvePanel;
    [SerializeField] Transform dissolvePanel_B;
    [SerializeField] Transform dissolvePanel_B2;
    [SerializeField] Transform dissolvePanel_B3;

    [SerializeField] GameObject catle1;
    [SerializeField] GameObject catle2;
    [SerializeField] GameObject catle3;
    [SerializeField] GameObject catle4;

    public float[] speedParams;

    enum oldState
    {
        move,
        stop,
        free
    }
    private oldState state = oldState.move;

    [SerializeField] Renderer nonActive, invisible;

    [SerializeField] AudioSource audio_SE;
    [SerializeField] AudioSource audio_Voice;
    [SerializeField] AudioClip select;
    [SerializeField] AudioClip preselect;

    [SerializeField] AudioClip[] voices;

    private State.StateProcessor.StateUpdate[] Iprocess;

    #region 時代の構造体
    [System.Serializable]
    public struct Node
    {
        [Header("時代")]
        public int era;
        [Header("説明")]
        public string explain;
        [Header("ボイス")]
        public AudioClip vc;
        [Header("サウンドエフェクト")]
        public AudioClip se;
        [Header("トランスフォーム")]
        public Transform transform;
        [NonSerialized]
        public int nextIndex;
        public UnityEvent process;
    }
    public Node[] nodes;
    [NonSerialized]
    public Node currentNode;
    #endregion

    void Start()
    {
        for (int i = 0; i < nodes.Length; i++)
            nodes[i].nextIndex = i + 1;

        currentNode = nodes[0];

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
    public void Event_1585() { StartCoroutine(Process_1585()); }
    IEnumerator Process_1585()
    {
        audio_SE.PlayOneShot(currentNode.se);
        audio_Voice.PlayOneShot(currentNode.vc);
        yield return null;
    }
    #endregion

    #region 1615年
    public void Event_1615() { StartCoroutine(Process_1615()); }
    IEnumerator Process_1615()
    {
        yield return null;
    }
    #endregion

    #region 1626年
    public void Event_1626() { StartCoroutine(Process_1626()); }
    IEnumerator Process_1626()
    {
        yield return null;
    }
    #endregion

    #region 1665年
    public void Event_1665() { StartCoroutine(Process_1665()); }
    IEnumerator Process_1665()
    {
        yield return null;
    }
    #endregion

    #region 1931年
    public void Event_1931() { StartCoroutine(Process_1931()); }
    IEnumerator Process_1931()
    {
        yield return null;
    }
    #endregion

    IEnumerator ActiveNextNode(int i)
    {
        yield return null;
    }

    public void ML_OnTriggerDown()
    {
        Ray ray = new Ray(controllerTip.position, controllerTip.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            string tag = hitInfo.collider.tag;

            if (tag == "NextWaitNode")
            {
                currentNode = nodes[currentNode.nextIndex];
                currentNode.process.Invoke();
                nodes[currentNode.nextIndex].transform.tag = "NextWaitNode";
            }
            else if (tag == "ActiveNode")
            {

            }
            else if (tag == "InActive")
            {

            }
        }
    }
}
