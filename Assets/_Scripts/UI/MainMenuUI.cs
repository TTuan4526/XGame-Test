using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button endlessBtn;

    // Start is called before the first frame update
    void Start()
    {
        endlessBtn.onClick.AddListener(OnClickEndlessBtn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickEndlessBtn()
    { 
        SceneManager.LoadScene("Endless");
    }
}
