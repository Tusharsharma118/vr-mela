//=====================================================================
// Copyright Tesseract Imaging Limited 2020. All Rights Reserved.
// Node module: Media Player
// Author: Sagar Ahirrao
//=====================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MediaServices
{
    public class DRMConfiguration 
    {
        public AndroidJavaObject m_DrmConfiguration;
        public AndroidJavaObject _drmUUUID { get; set; }
        public string _licenseURL { get; set; }
        public string _jsonHeaders { get; set; }

        public DRMConfiguration(AndroidJavaObject uuidObject, string licenseURL = null, string jsonHeader= null)
        {
            _drmUUUID = uuidObject;
            _licenseURL = licenseURL;
            _jsonHeaders = jsonHeader;

            m_DrmConfiguration = new AndroidJavaObject(MediaPlayerConstants.DRM_CONFIGURATION_BUILDER).
               Call<AndroidJavaObject>("setUUID", _drmUUUID).
               Call<AndroidJavaObject>("setLicenseUrl", _licenseURL).
               Call<AndroidJavaObject>("setHeaders", _jsonHeaders).
               Call<AndroidJavaObject>("build");
        }
        public static DRMConfiguration GetDRMObject(AndroidJavaObject _drmObject)
        {
            if (_drmObject != null)
                return new DRMConfiguration(
                    _drmObject.Get<AndroidJavaObject>("drmEnum"),
                    _drmObject.Get<string>("licenseUrl"),
                    _drmObject.Get<string>("jsonHeaders")
                    );
            else
                return null;
        }

    }

    [System.Serializable]
    public class HeadersList
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    [System.Serializable]
    public class MediaHeader
    {
        public List<HeadersList> headersList = new List<HeadersList>();
    }


}
