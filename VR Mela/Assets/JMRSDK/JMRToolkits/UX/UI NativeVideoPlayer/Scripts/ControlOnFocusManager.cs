using JMRSDK.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMRSDK.Toolkit.UI
{
    public class ControlOnFocusManager : MonoBehaviour, IFocusable
    {
        [SerializeField] 
        private JMRNativeVideoPlayer videoPlayer;

        private void Start()
        {
            
        }

        public void OnFocusEnter()
        {
            if (videoPlayer != null)
            {
                videoPlayer.PointerOverControl(true);
            }
        }

        public void OnFocusExit()
        {
            if (videoPlayer != null)
            {
                videoPlayer.PointerOverControl(false);
            }
        }
    }
}