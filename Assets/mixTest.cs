using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JAnimationSystem;

public class mixTest : MonoBehaviour
{
    public Transform t1;
    public JAnimation_AutoSetBoneMatchData autoData1;
    public List<JAnimationData> datas = new List<JAnimationData>();
    List<BoneMatch_Player> players = new List<BoneMatch_Player>();
    BoneMatch match1 = null;
    [Range(0, 0.5f)]
    public float mixTime = 0.2f;
    public AnimationCurve mixCurve = new AnimationCurve();
    void Start()
    {
        InitPlayers();
    }

    void InitPlayers()
    {
        match1 = new BoneMatch();
        match1.StartAutoMatch(t1, autoData1);
        for(int i=0; i< datas.Count; i++)
        { 
            BoneMatch_Player player1 = new BoneMatch_Player(match1);
            player1.SetJAnimationData(datas[i]);
            players.Add(player1);
        }
        nowPlaying = 0;
        time = datas[0].jAD.TimeLength;
        timer = 0f;
    }

    bool isMix = false;
    int nowPlaying = 0;
    int wantSwitchTo = 0;
    float timer = 0f;
    float time = 0f;
    float mixTimer = 0f;

    public void SwitchTo(int num)
    {
        if(num >= datas.Count)
        {
            num = 0;
        }
        wantSwitchTo = num;
        //mixTimer = 0f;
    }

    void Update()
    {
        SwitchTo(nowPlaying + 1);
        if (Input.GetMouseButtonUp(0))
        {
            SwitchTo(nowPlaying + 1); 
        }

        if(isMix == false)
        {
            players[nowPlaying].PlayDataInThisFrame_Curve(timer, false);
            timer += Time.deltaTime;
            if(timer > time)
            {
                timer = 0f;
            }
            if(wantSwitchTo != nowPlaying)
            {
                isMix = true;
                mixTimer = 0f;
                time = datas[wantSwitchTo].jAD.TimeLength;
                timer = 0f;
            }
        }
        else
        {
            if(mixTimer <= mixTime)
            {
                players[wantSwitchTo].PlayDataInThisFrame_Curve(timer, false, mixCurve.Evaluate(mixTimer / mixTime));
                mixTimer += Time.deltaTime; 
            }
            timer += Time.deltaTime;
            if(mixTimer > mixTime)
            {
                nowPlaying = wantSwitchTo;
                isMix = false;
            }
        }
    }
}
