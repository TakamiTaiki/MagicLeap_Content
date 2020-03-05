using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
namespace State
{
    public class StateProcessor
    {
        public delegate void StateUpdate(bool isFirst);

        private StateUpdate currentUpdate, nextUpdate;

        public void Update()
        {
            if (nextUpdate != null)
            {
                currentUpdate = nextUpdate;
                nextUpdate = null;
                currentUpdate(true);
            }
            else
                currentUpdate(false);
        }

        public void SetState(StateUpdate nextUpdate)
        {
            this.nextUpdate = nextUpdate;            
        }
    }
}
