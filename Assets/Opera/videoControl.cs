using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class videoControl : MonoBehaviour
{
    public Canvas canva1;
    public VideoPlayer player;

    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }
    public bool isShowing
    {
        get
        {
            if (player == null)
            {
                return false;
            }
            return canva1.gameObject.activeSelf;
        }
    }

    public void Hide()
    {
        if (player == null) return;
        player.Pause();
        canva1.gameObject.SetActive(false);
    }
    public void Play()
    {
        if (player == null) return;
        if (player.isPrepared)  player.Play(); 
    }
    public void PreOne(VideoClip _clip)
    {
        if (player == null) return;
        Debug.Log("Pre video: " + _clip.name);
        canva1.gameObject.SetActive(true);
        player.clip = _clip;
        player.Prepare();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
