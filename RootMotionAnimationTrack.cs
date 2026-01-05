using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FM.Cinematics
{
    /// <summary>
    /// A custom animation track for Timeline playback.
    /// 
    /// NOTE: Root motion application has been disabled. This track only handles animation playback.
    /// If you need root motion, enable "Apply Root Motion" on the Animator component directly.
    /// </summary>
    [Serializable]
    [TrackColor(0.53f, 0.0f, 0.08f)]
    [TrackClipType(typeof(RootMotionAnimationClip))]
    [TrackBindingType(typeof(Animator))]
    [DisplayName("FM/Root Motion Animation Track")]
    public class RootMotionAnimationTrack : TrackAsset
    {
        [SerializeField]
        private AvatarMask m_AvatarMask;

        [SerializeField]
        private bool m_ApplyAvatarMask = false;

        [SerializeField, Tooltip("Apply Foot IK for humanoid characters")]
        private bool m_ApplyFootIK = true;

        /// <summary>
        /// Avatar mask for filtering animation
        /// </summary>
        public AvatarMask avatarMask
        {
            get => m_AvatarMask;
            set => m_AvatarMask = value;
        }

        /// <summary>
        /// Whether to apply the avatar mask
        /// </summary>
        public bool applyAvatarMask
        {
            get => m_ApplyAvatarMask;
            set => m_ApplyAvatarMask = value;
        }

        /// <summary>
        /// Whether to apply foot IK
        /// </summary>
        public bool applyFootIK
        {
            get => m_ApplyFootIK;
            set => m_ApplyFootIK = value;
        }

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            // Configure clips with track settings
            foreach (var clip in GetClips())
            {
                var rmClip = clip.asset as RootMotionAnimationClip;
                if (rmClip != null)
                {
                    rmClip.ApplyFootIK = m_ApplyFootIK;
                }
            }

            // Create the animation mixer playable for blending clips
            AnimationMixerPlayable animMixer = AnimationMixerPlayable.Create(graph, inputCount);

            // If avatar mask is set, use a layer mixer
            Playable outputPlayable = animMixer;
            if (m_ApplyAvatarMask && m_AvatarMask != null)
            {
                var layerMixer = AnimationLayerMixerPlayable.Create(graph, 1);
                graph.Connect(animMixer, 0, layerMixer, 0);
                layerMixer.SetInputWeight(0, 1f);
                layerMixer.SetLayerMaskFromAvatarMask(0, m_AvatarMask);
                outputPlayable = layerMixer;
            }

            return outputPlayable;
        }

        /// <inheritdoc />
        public override IEnumerable<PlayableBinding> outputs
        {
            get { yield return AnimationPlayableBinding.Create(name, this); }
        }

        /// <summary>
        /// Called when a new clip is created
        /// </summary>
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
        }
        
        /// <summary>
        /// Gathers properties for preview
        /// </summary>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            // Gather animation clip properties for preview
            foreach (var clip in GetClips())
            {
                var rmClip = clip.asset as RootMotionAnimationClip;
                if (rmClip != null && rmClip.AnimationClip != null)
                {
                    driver.AddFromClip(rmClip.AnimationClip);
                }
            }
        }
    }
}
