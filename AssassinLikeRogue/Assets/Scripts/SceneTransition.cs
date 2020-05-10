using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public Animator transitionAnim;
    public string sceneName;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            transitionAnim.SetTrigger("start");
        }
        else if (Input.GetKeyDown(KeyCode.Quote))
        {
            transitionAnim.SetTrigger("end");
            //StartCoroutine(LoadScene());
        }
        else if(Input.GetKeyDown(KeyCode.Semicolon))
        {
            transitionAnim.SetTrigger("idle");
        }
    }
    
    /*
    IEnumerator LoadScene()
    {
        transitionAnim.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);
        //SceneManager.LoadScene(sceneName);
    }
    */
}
