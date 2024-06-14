using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class IntoTimelineHandler : MonoBehaviour
{
    [SerializeField] private string animatorTag;
    [SerializeField] PlayableDirector playableDirector;

    public void Play(Animator animator, AnimationClip animationClip)
    {
        PlayIntroCutscene (animator, animationClip);
      //  Bind (playableDirector, animatorTag, animator);
       // playableDirector.Play();
    }

    public static void Bind(PlayableDirector director, string trackName, Animator animator)
    {
        var timeline = director.playableAsset as TimelineAsset;
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track.name == trackName)
            {
                director.SetGenericBinding(track, animator);
                break;
            }
        }
    }

    public void PlayIntroCutscene(Animator animator, AnimationClip firstAnim)
    {
        TimelineAsset timelineAsset = (TimelineAsset)playableDirector.playableAsset;
        foreach (var track in timelineAsset.GetOutputTracks())
        {
            if (track is AnimationTrack animationTrack)
            {
                playableDirector.SetGenericBinding(track, animator);

                foreach (TimelineClip timelineClip in animationTrack.GetClips())
                {               
                    AnimationPlayableAsset animAsset = (AnimationPlayableAsset)timelineClip.asset;
                    if (animAsset)
                    {
                        animAsset.clip = firstAnim;
                    }
              
                }
                break;
            }
        }

        playableDirector.Play();
    }

    public void PlayIntroCutscene(Animator animator)
    {
        TimelineAsset timelineAsset = (TimelineAsset)playableDirector.playableAsset;


        //playableDirector.

        foreach (var track in timelineAsset.GetOutputTracks())
        {
            if (track is AnimationTrack animationTrack)
            {
                playableDirector.SetGenericBinding(track, animator);
                break;
            }
        }

        playableDirector.Play();
    }
}
