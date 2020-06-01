using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using State;
using UnityEngine.Events;
using Utils;
using UnityEngine.XR.MagicLeap;

public class MainManager : MonoBehaviour
{
    #region フィールド
    StateProcessor stateProcessor = new StateProcessor();

    [SerializeField, Tooltip("KeyPose to track.")]
    private MLHandKeyPose _keyPoseToTrack = MLHandKeyPose.NoPose;
    [Space, SerializeField, Tooltip("Flag to specify if left hand should be tracked.")]
    private bool _trackLeftHand = true;

    [SerializeField, Tooltip("Flag to specify id right hand should be tracked.")]
    private bool _trackRightHand = true;

    [SerializeField] Text timeText;
    [SerializeField] Text dateText;

    #endregion

    /**********************************************************************************/

    #region イニシャライズ
    public void Init()
    {
        //初期コンテンツを開始
        stateProcessor.SetState(ST_Start);
    }
    #endregion

    void Start()
    {
        Init();
    }

    void Update()
    {
        stateProcessor.Update();

        dateText.text = DateTime.Now.ToString("MM/dd");
        timeText.text = DateTime.Now.ToString("HH:mm");

        if (!MLHands.IsStarted) return;

        float confidenceLeft = _trackLeftHand ? GetKeyPoseConfidence(MLHands.Left) : 0.0f;
        float confidenceRight = _trackRightHand ? GetKeyPoseConfidence(MLHands.Right) : 0.0f;
        float confidenceValue = Mathf.Max(confidenceLeft, confidenceRight);
    }

    #region スタートステート
    public void ST_Start(bool isFirst)
    {
        //初期処理
        if (isFirst)
        {
        }
        //継続処理
        else
        {
            //他に何かあれば
        }
    }

    private float GetKeyPoseConfidence(MLHand hand)
    {
        if (hand != null)
        {
            if (hand.KeyPose == _keyPoseToTrack)
            {
                return hand.KeyPoseConfidence;
            }
        }
        return 0.0f;
    }
    #endregion
}
