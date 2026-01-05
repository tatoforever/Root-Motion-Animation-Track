using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FM.Cinematics
{
    /// <summary>
    /// A timeline clip asset that wraps an AnimationClip for use with RootMotionAnimationTrack.
    /// This is a simple animation clip wrapper for Timeline playback.
    /// 
    /// NOTE: Root motion application has been disabled. If you need root motion,
    /// enable "Apply Root Motion" on the Animator component directly.
    /// </summary>
    [Serializable]
    public class RootMotionAnimationClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField]
        private AnimationClip m_AnimationClip;

        [SerializeField]
        private bool m_ApplyFootIK = true;

        [SerializeField]
        private bool m_Loop = false;

        /// <summary>
        /// The animation clip to play
        /// </summary>
        public AnimationClip AnimationClip
        {
            get => m_AnimationClip;
            set => m_AnimationClip = value;
        }

        /// <summary>
        /// Whether to apply foot IK
        /// </summary>
        public bool ApplyFootIK
        {
            get => m_ApplyFootIK;
            set => m_ApplyFootIK = value;
        }

        /// <summary>
        /// Whether the animation should loop
        /// </summary>
        public bool Loop
        {
            get => m_Loop;
            set => m_Loop = value;
        }

        /// <summary>
        /// Duration of the clip
        /// </summary>
        public override double duration
        {
            get
            {
                if (m_AnimationClip == null)
                    return base.duration;

                double length = m_AnimationClip.length;
                return length > 0.001 ? length : base.duration;
            }
        }

        /// <summary>
        /// Clip capabilities
        /// </summary>
        public ClipCaps clipCaps
        {
            get
            {
                var caps = ClipCaps.Blending | ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | ClipCaps.Extrapolation;
                if (m_Loop || (m_AnimationClip != null && m_AnimationClip.isLooping))
                    caps |= ClipCaps.Looping;
                return caps;
            }
        }

        /// <summary>
        /// Creates the playable for this clip
        /// </summary>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            if (m_AnimationClip == null || m_AnimationClip.legacy)
                return Playable.Null;

            // Create the animation clip playable
            var clipPlayable = AnimationClipPlayable.Create(graph, m_AnimationClip);
            
            // Configure foot IK
            clipPlayable.SetApplyFootIK(m_ApplyFootIK);

            return clipPlayable;
        }

        /// <summary>
        /// Outputs for this playable
        /// </summary>
        public override IEnumerable<PlayableBinding> outputs
        {
            get { yield return AnimationPlayableBinding.Create(name, this); }
        }

        /// <summary>
        /// Reset all settings to defaults
        /// </summary>
        public void ResetToDefaults()
        {
            m_Loop = false;
            m_ApplyFootIK = true;
        }
    }
}
