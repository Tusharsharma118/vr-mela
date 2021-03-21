// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using Tesseract.Utility;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Animations;
using System.Collections.Generic;
using JMRSDK.InputModule;

namespace JMRSDK.Toolkit.UI
{
    public delegate void DMessageHandler(string command);

    public class JMRVirtualKeyBoard : JMRPersistance<JMRVirtualKeyBoard>, IMessageHandler
    {
        #region SERIALIZED FIELDS
        [SerializeField]
        private Animator anim;
        [SerializeField]
        private TextMeshProUGUI suggestedText;
        [SerializeField]
        private GameObject alphabets, special_characters;
        [SerializeField]
        private Button searchBtn;
        [SerializeField]
        private LookAtConstraint lookAt;
        #endregion

        #region PRIVATE FIELD
        private DMessageHandler j_handler;
        private bool isUpperCase, isTempUpper;
        private float j_counter, j_doubleTapDelay = 0.25f;
        private string j_prevButton;
        private IKeyboardInput j_input;
        private Transform j_ThisTransform;
        private ConstraintSource j_LookAtSource = new ConstraintSource() { weight = 1 };
        #endregion

        #region PUBLIC FIELDS
        public bool isShown;
        #endregion

        #region MONO METHODS

        private void OnEnable()
        {
            if (!j_ThisTransform)
                j_ThisTransform = transform;
        }

        private string prevText = "";
        private void Update()
        {
            if (j_counter < j_doubleTapDelay)
                j_counter += Time.deltaTime;

            if (j_input != null && prevText != suggestedText.text)
            {
                prevText = suggestedText.text;
                j_input.Text = cachedTex + suggestedText.text;
            }
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Show Keyboard.
        /// </summary>
        /// <param name="j_InputField"></param>
        /// 
        private string cachedTex = "";
        public void ShowKeyBoard(IKeyboardInput j_InputField)
        {
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);

            if (j_InputField != null && j_InputField != j_input)
            {
                if (j_input != null)
                {
                    j_input.OnDeselect();
                }

                MonoBehaviour inputField = (MonoBehaviour)j_InputField;
                if (j_InputField.j_KeyboardPosition != null)
                {
                    transform.position = j_InputField.j_KeyboardPosition.position;
                }
                else
                {
                    if (inputField.transform.position.y >= 0)
                    {
                        transform.position = inputField.transform.position + Vector3.down * 0.4f;
                    }
                    else
                    {
                        transform.position = inputField.transform.position + Vector3.up * 0.4f;
                    }
                }

                this.j_input = j_InputField;
                cachedTex = string.IsNullOrEmpty(j_input.Text) ? "" : j_input.Text;
                if (string.IsNullOrEmpty(cachedTex))
                    StartCoroutine(WaitTillEOF());
                suggestedText.text = prevText = "";
                isShown = true;
            }
        }

        /// <summary>
        /// Hide Keyboard.
        /// </summary>
        public void HideKeyBoard()
        {
            if (!gameObject.activeSelf)
                return;


            // anim.SetTrigger("Hide");
            if (j_input != null)
            {
                Constants.SearchString = suggestedText.text;
                j_input.EditEnd();
                j_input.OnDeselect();
            }

            isShown = false;
            this.j_input = null;
            special_characters.SetActive(false);
            alphabets.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Register New Key.
        /// </summary>
        /// <param name="key"></param>
        public void RegisterKey(IMessageHandler Key)
        {
            this.j_handler += Key.HandleMessage;
        }

        /// <summary>
        /// Handle Virtual Keyboard Actions.
        /// </summary>
        /// <param name="command"></param>
        public void HandleMessage(string command)
        {
            switch (command)
            {
                case Constants.CASE_TAP:
                    if (j_prevButton == command && j_counter < j_doubleTapDelay)
                    {
                        j_counter = j_doubleTapDelay + 1;
                        isUpperCase = isTempUpper = true;
                    }
                    else
                    {
                        if (isUpperCase)
                        {
                            isUpperCase = isTempUpper = false;
                        }
                        else
                            isTempUpper = !isTempUpper;
                    }

                    string cntrl = isTempUpper ? Constants.CASE_UPPER : Constants.CASE_LOWER;
                    if (j_handler != null)
                        j_handler(cntrl);
                    break;
                case Constants.BACK_SPACE:
                    if (j_input == null || j_input.Text.Length <= 0)
                        break;
                    if (j_prevButton == command && j_counter < j_doubleTapDelay)
                    {
                        j_input.Text = "";
                        suggestedText.text = cachedTex = "";
                    }
                    else
                    {
                        j_input.Text = j_input.Text.Substring(0, j_input.Text.Length - 1);
                        if (j_input.Text.Length < cachedTex.Length)
                            cachedTex = j_input.Text;
                        else
                            suggestedText.text = j_input.Text.Substring(cachedTex.Length, j_input.Text.Length - cachedTex.Length);
                    }
                    break;
                case Constants.ALPHABETS:
                    if (alphabets.activeInHierarchy)
                        break;
                    special_characters.SetActive(false);
                    alphabets.gameObject.SetActive(true);
                    break;
                case Constants.SPECIAL_CHARACTERS:
                    if (special_characters.activeInHierarchy)
                        break;
                    alphabets.gameObject.SetActive(false);
                    special_characters.SetActive(true);
                    break;
                case Constants.ENTER:
                    j_input.HandleKeyboardEnterKey();
                    HideKeyBoard();
                    break;
                default:
                    if (command == " " && (j_input == null || j_input.Text.Length <= 0))
                        break;
                    suggestedText.text += command;
                    break;
            }

            if (j_prevButton != Constants.CASE_TAP && !isUpperCase)
                j_counter = 0;

            if (isTempUpper && Constants.CASE_TAP != command && !isUpperCase)
            {
                j_handler(Constants.CASE_LOWER);
                isTempUpper = false;
            }
            j_prevButton = command;
        }

        /// <summary>
        /// Handle Virtual Keyboard Actions.
        /// </summary>
        /// <param name="type",name="msg"></param>
        public void HandleMessage(string type, string msg)
        {
        }

        /// <summary>
        /// Handle On Search Action.
        /// </summary>
        /// <param name="listener"></param>
        public void OnSearch(UnityAction listener)
        {
        }

        #endregion        

        #region ENUMERATORS

        System.Collections.IEnumerator WaitTillEOF()
        {
            yield return new WaitForEndOfFrame();
            if (string.IsNullOrEmpty(suggestedText.text))
                HandleMessage(Constants.CASE_TAP);
            // HideKeyBoard();
            while (JMRCameraUtility.Main == null)
            {
                yield return null;
            }

            // Set the parameters for LookAtConstraint for the Keyboard to look at Camera.
            j_LookAtSource.sourceTransform = JMRCameraUtility.Main.transform;
            lookAt.SetSource(0, j_LookAtSource);
            lookAt.constraintActive = false;
        }

        #endregion
    }

}
