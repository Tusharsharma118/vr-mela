using System;
using JMRSDK;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterDemoExample : MonoBehaviour
{
    private static MasterDemoExample instance;

    private void Awake()
    {
        if (instance) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadScene("_JMRMasterDemo");
        }
    }

    public void LoadScene(string scene)
    {
        Debug.Log("Loading scene " + scene);

        Destroy(FindObjectOfType<JMRRigManager>().gameObject);
        SceneManager.LoadScene(scene);
    }
}