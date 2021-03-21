using JMRSDK.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMRSDK.Toolkit.UI
{
    public class ControlOnFocusManagerSpatial : MonoBehaviour, IFocusable
    {
        [SerializeField] 
        private JMRSpatialVideoPlayer videoPlayer;

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