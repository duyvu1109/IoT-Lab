using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class SwitchButton : MonoBehaviour
{
        public bool switchState = false;
        public GameObject switchBtn;

        public void OnSwitchButtonClicked()
        {
            switchBtn.transform.DOLocalMoveX(-switchBtn.transform.localPosition.x, 0.2f);
            //switchState = Math.Sign(-switchBtn.transform.localPosition.x);
            Debug.Log("btn State: " + switchState);
        }
        public void setStatus(bool status) {
            if (switchState != status) {
                switchState = status;
                this.OnSwitchButtonClicked();
            }
        }

}
