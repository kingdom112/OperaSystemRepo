using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using JAnimationSystem;

public class TrunToAnimClipTest : MonoBehaviour
{
    public Transform t_root;
   public JAnimation_AutoSetBoneMatchData autoData1;
    public JAnimationData animData1;
    public Animation anim1;
    public Transform mover;
    public Transform hip;
    public Transform hipParent;
    Vector3 lastPos = Vector3.zero;
    float lastRoa = 0f;
    Vector3 oPos = Vector3.zero;

    public Transform ttttt;

    // Start is called before the first frame update
    void Start()
    { 


        BoneMatch match1 = new BoneMatch();
        match1.StartAutoMatch(t_root, autoData1);
        BoneMatch_Player player1 = new BoneMatch_Player(match1);
        player1.SetJAnimationData(animData1);
        AnimationClip clip1 = player1.TurnToAnimClip(t_root, true);
        anim1.AddClip(clip1, clip1.name);
        anim1.clip = clip1;
        anim1.Play();
        anim1.Sample();
        oPos = mover.localPosition;
        lastPos = mover.localPosition;
        lastRoa = mover.localEulerAngles.y;
        //PlayableGraph m_Graph =  PlayableGraph.Create();
        //var animOutput = PlayableOutput
    }

    private void Update()
    {
      
    }

    private void LateUpdate()
    {
        if (anim1.isPlaying)
        {

            hipParent.localEulerAngles = new Vector3(hipParent.localEulerAngles.x, -hip.localEulerAngles.y, hipParent.localEulerAngles.z);


            if (Vector3.Distance(mover.localPosition, oPos) <= 0.001f)
            {
                lastPos = mover.localPosition;
                lastRoa = mover.localEulerAngles.y;
            }
            else
            {
                Vector3 posOffset = mover.localPosition - lastPos;
                lastPos = mover.localPosition;
                //t_root.position += posOffset;
                //now * offset = new
                //offset = 1/now * new


                //Quaternion roaOffset = Quaternion.Inverse(lastRoa) * mover.localRotation;
                float roaOffset = mover.localEulerAngles.y - lastRoa;
                lastRoa = mover.localEulerAngles.y;
                t_root.eulerAngles = new Vector3(t_root.eulerAngles.x, t_root.eulerAngles.y + roaOffset, t_root.eulerAngles.z);
            }
        }
        else
        {
            //anim1.Play(); 
        }
    } 
}
