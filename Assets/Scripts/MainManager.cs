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
    #endregion
}
