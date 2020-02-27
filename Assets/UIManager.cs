using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Transform root;
    public float distance = 10f;
    [SerializeField] Transform[] nodes;

    [SerializeField] Text debugText;

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

    public float speed = 1f;

    enum State
    {
        move,
        stop
    }
    private State state = State.move;

    [SerializeField] Renderer nonActive, invisible;

    void Start()
    {
        dissolvePanel.position = new Vector3(-0.9f, dissolvePanel.position.y, dissolvePanel.position.z);
        DissolveStage++;
    }


    void Update()
    {
        Ray ray = new Ray(root.position, root.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            string tag = hitInfo.collider.tag;

            debugText.text = tag;

            switch(tag)
            {
                case "1585Node":
                    if (DissolveStage == 1)
                        DissolveStage++;
                    break;
                case "1615Node":
                    if (DissolveStage == 2)
                        DissolveStage++;
                    break;
                case "1626Node":
                    if (DissolveStage == 3)
                        DissolveStage++;
                    break;
                case "1665Node":
                    if (DissolveStage == 4)
                        DissolveStage++;
                    break;
                case "1931Node":
                    if (DissolveStage == 5)
                        DissolveStage++;
                    break;
                default:
                    break;
            }
        }

        debugText.text = string.Empty;
        switch (state)
        {
            case State.move:
                nonActive.material.SetFloat("_DissolveNoiseStrength", 0.025f);
                invisible.material.SetFloat("_DissolveNoiseStrength", 0.025f);
                break;
            case State.stop:
                nonActive.material.SetFloat("_DissolveNoiseStrength", 0.025f + 0.01f * Mathf.Sin(Time.time));
                invisible.material.SetFloat("_DissolveNoiseStrength", 0.025f + 0.01f * Mathf.Sin(Time.time));
                break;
        }
    }

    IEnumerator ActiveNextNode(int index)
    {
        while (dissolvePanel.position.x < nodes[index].position.x)
        {
            var pos = dissolvePanel.position;
            pos.x += speed * Time.deltaTime;
            dissolvePanel.position = pos;
            yield return null;
        }
        state = State.stop;
    }
}
