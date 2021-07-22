// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;

namespace JMRSDK.Toolkit.UI
{
    public interface IKeyboardInput
    {
        string Text { get; set; }
        Transform j_KeyboardPosition { get; set; }
        void HandleKeyboardEnterKey();
        void OnDeselect();
        bool isMultiLineSupported();

        void EditEnd();
    }
}
