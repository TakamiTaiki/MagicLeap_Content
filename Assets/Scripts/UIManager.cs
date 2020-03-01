using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

public class UIManager : MonoBehaviour
{
    [SerializeField] Transform root;
    public float distance = 10f;
    [SerializeField] Transform[] nodes;

    [SerializeField] Text explainText;

    [SerializeField] RectTransform indicater;

    [SerializeField] GameObject nodeIndicater;

    [SerializeField] GameObject selectGlow;

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
            state = State.move;
            StartCoroutine(ActiveNextNode(dissolveStage));
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

    enum State
    {
        move,
        stop,
        free
    }
    private State state = State.move;

    [SerializeField] Renderer nonActive, invisible;

    [SerializeField] AudioSource audio_SE;
    [SerializeField] AudioSource audio_Voice;
    [SerializeField] AudioClip select;
    [SerializeField] AudioClip preselect;
    [SerializeField] AudioClip fireSE;
    [SerializeField] AudioClip thunder;
    [SerializeField] AudioClip initSE;
    [SerializeField] AudioClip restore;
    [SerializeField] AudioClip restore2;

    [SerializeField] AudioClip[] voices;


    void Init()
    {
        state = State.move;
        explainText.text = "豊臣秀頼による大阪城天守の完成";
        nodeIndicater.transform.localScale = new Vector3(0.1423f, 0.1423f, 0.1423f);
        dissolvePanel.localPosition = new Vector3(-1f, -0.182f, 0.538f);
        PanelInit();
        DissolveStage++;
        selectGlow.SetActive(false);
        nodeIndicater.SetActive(true);
        explainText.gameObject.SetActive(false);
        fire.SetActive(false);
    }

    void Start()
    {
        Init();
    }


    void Update()
    {
        switch (state)
        {
            case State.move:
                break;
            case State.stop:
                explainText.gameObject.SetActive(true);
                indicater.Rotate(new Vector3(2f, 0, 0));
                break;
            case State.free:
                explainText.gameObject.SetActive(true);
                indicater.Rotate(new Vector3(2f, 0, 0));
                Ray ray = new Ray(root.position, root.forward);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, distance))
                {
                    selectGlow.transform.position = hitInfo.collider.gameObject.transform.position;
                    if (tmp != hitInfo.collider.gameObject.tag)
                    {
                        selectGlow.GetComponent<Renderer>().material.color = Color.white;
                        audio_SE.PlayOneShot(preselect);
                    }
                        

                    tmp = hitInfo.collider.gameObject.tag;
                }
                    break;
                
        }
    }

    IEnumerator ActiveNextNode(int index)
    {
        if (DissolveStage == nodes.Length)
        {
            nodeIndicater.SetActive(false);
        }
        while (dissolvePanel.position.x < nodes[index].position.x)
        {
            var pos = dissolvePanel.position;
            pos.x += speedParams[0] * Time.deltaTime;
            dissolvePanel.position = pos;
            yield return null;
        }
        state = State.stop;
        StartCoroutine(EnLarge());
        if (DissolveStage == nodes.Length)
        {
            state = State.free;
            selectGlow.SetActive(true);
            nodeIndicater.SetActive(false);
            //PanelInit();
        }
    }

    IEnumerator BePrime()
    {
        audio_SE.PlayOneShot(restore);
        while (dissolvePanel_B.position.y > targets[0].transform.position.y)
        {
            dissolvePanel_B.position -= new Vector3(0, Time.deltaTime * speedParams[1], 0);
            yield return null;
        }
    }

    IEnumerator BeCharred()
    {
        audio_SE.PlayOneShot(fireSE);
        fire.SetActive(true);
        while (dissolvePanel_B2.position.y < targets[1].transform.position.y)
        {
            dissolvePanel_B2.position += new Vector3(0, Time.deltaTime * speedParams[1], 0);
            yield return null;
        }
        fire.SetActive(false);
        catle1.SetActive(false);
        catle2.SetActive(false);
        catle3.SetActive(false);
    }

    IEnumerator Restore()
    {
        audio_SE.PlayOneShot(restore);
        while (dissolvePanel_B3.position.y < targets[1].transform.position.y)
        {
            dissolvePanel_B3.position += new Vector3(0, Time.deltaTime * speedParams[1], 0);
            yield return null;
        }
    }

    IEnumerator BeLightning()
    {
        audio_SE.PlayOneShot(thunder);
        FindObjectOfType<OrbitalBeamLaser>().AAA();
        yield return new WaitForSeconds(3f);
        catle4.SetActive(false);
        FindObjectOfType<OrbitalBeamLaser>().BBB();
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

    public void ChangeMLCtrl()
    {
        Ray ray = new Ray(root.position, root.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            string tag = hitInfo.collider.tag;
            audio_SE.PlayOneShot(select);

            if (state == State.free)
            {
                selectGlow.GetComponent<Renderer>().material.color = Color.green;

                switch (tag)
                {
                    case "1585Node":
                        StartCoroutine(BePrime());
                        audio_Voice.PlayOneShot(voices[0]);
                        explainText.text = "豊臣秀頼による大阪城天守の完成";
                            FindObjectOfType<OrbitalBeamLaser>().BBB();
                            break;
                    case "1615Node":
                        audio_Voice.PlayOneShot(voices[1]);
                        StartCoroutine(BeCharred());
                            explainText.text = "大坂夏の陣にて焼失";
                            StartCoroutine(BeCharred());
                            FindObjectOfType<OrbitalBeamLaser>().BBB();
                            break;
                    case "1626Node":
                        audio_Voice.PlayOneShot(voices[2]);
                        StartCoroutine(Restore());
                            explainText.text = "徳川による大阪城天守の再建";
                            FindObjectOfType<OrbitalBeamLaser>().BBB();
                            break;
                    case "1665Node":
                        audio_Voice.PlayOneShot(voices[3]);
                        StartCoroutine(BeLightning());
                            explainText.text = "落雷により焼失";

                        break;
                    case "1931Node":
                        audio_Voice.PlayOneShot(voices[4]);
                        dissolvePanel_B3.localPosition = new Vector3(-0.018f, -0.628f, 1.568f);
                        catle4.SetActive(true);
                        StartCoroutine(Restore());
                            explainText.text = "3代目天守閣の再建";
                            FindObjectOfType<OrbitalBeamLaser>().BBB();
                        Invoke("PanelInit", 6f);
                            break;
                    default:
                            break;
                }
                return;

            }

            switch (tag)
            {
                case "1585Node":
                    if (DissolveStage == 1)
                    {
                        audio_Voice.PlayOneShot(voices[0]);
                        StartCoroutine(BePrime());
                        DissolveStage++;
                        explainText.text = "大坂夏の陣にて焼失";
                        explainText.gameObject.SetActive(false);
                        StartCoroutine(BeSmall(smallTime));
                    }
                    break;
                case "1615Node":
                    if (DissolveStage == 2)
                    {
                        audio_Voice.PlayOneShot(voices[1]);
                        StartCoroutine(BeCharred());
                        DissolveStage++;
                        explainText.text = "徳川による大阪城天守の再建";
                        explainText.gameObject.SetActive(false);
                        StartCoroutine(BeSmall(smallTime));
                    }
                    break;
                case "1626Node":
                    if (DissolveStage == 3)
                    {
                        audio_Voice.PlayOneShot(voices[2]);
                        StartCoroutine(Restore());
                        DissolveStage++;
                        explainText.text = "落雷により焼失";
                        explainText.gameObject.SetActive(false);
                        StartCoroutine(BeSmall(smallTime));

                    }
                    break;
                case "1665Node":
                    if (DissolveStage == 4)
                    {
                        audio_Voice.PlayOneShot(voices[3]);
                        StartCoroutine(BeLightning());
                        DissolveStage++;
                        explainText.text = "3代目天守閣の再建";
                        explainText.gameObject.SetActive(false);
                        StartCoroutine(BeSmall(smallTime));
                    }
                    break;
                case "1931Node":
                    if (DissolveStage == 5)
                    {
                        audio_Voice.PlayOneShot(voices[4]);
                        dissolvePanel_B3.localPosition = new Vector3(-0.018f, -0.628f, 1.568f);
                        catle4.SetActive(true);
                        StartCoroutine(Restore());
                        FindObjectOfType<OrbitalBeamLaser>().BBB();
                        DissolveStage++;
                        explainText.text = "現在";
                        explainText.gameObject.SetActive(false);
                        StartCoroutine(BeSmall(smallTime));
                        Invoke("PanelInit", 6f);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void MLPJInit()
    {
        Ray ray = new Ray(root.position, root.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            string tag = hitInfo.collider.tag;

            if (tag == "Init")
            {
                audio_Voice.PlayOneShot(voices[6]);
                dissolveStage = 0;
                Init();
            }

        }
    }

    public void PanelInit()
    {
        dissolvePanel_B.localPosition = new Vector3(-0.018f, 0.3f, 1.568f);
        dissolvePanel_B2.localPosition = new Vector3(-0.018f, -0.636f, 1.568f);
        dissolvePanel_B3.localPosition = new Vector3(-0.018f, -0.628f, 1.568f);
        catle1.SetActive(true);
        catle2.SetActive(true);
        catle3.SetActive(true);
        catle4.SetActive(true);
    }
    
}
