#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;
using FM.Cinematics;

namespace FM.Cinematics.Editor
{
    /// <summary>
    /// Custom clip editor for RootMotionAnimationClip
    /// </summary>
    [CustomTimelineEditor(typeof(RootMotionAnimationClip))]
    public class RootMotionAnimationClipEditor : ClipEditor
    {
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            base.OnCreate(clip, track, clonedFrom);

            var rmClip = clip.asset as RootMotionAnimationClip;
            if (rmClip != null && rmClip.AnimationClip != null)
            {
                clip.displayName = rmClip.AnimationClip.name;
            }
        }

        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var options = base.GetClipOptions(clip);
            
            var rmClip = clip.asset as RootMotionAnimationClip;
            if (rmClip != null && rmClip.AnimationClip != null)
            {
                options.highlightColor = new Color(0.53f, 0.0f, 0.08f); // Track color
            }

            return options;
        }

        public override void OnClipChanged(TimelineClip clip)
        {
            base.OnClipChanged(clip);

            var rmClip = clip.asset as RootMotionAnimationClip;
            if (rmClip != null && rmClip.AnimationClip != null)
            {
                // Update display name if animation clip changed
                if (string.IsNullOrEmpty(clip.displayName) || clip.displayName == "RootMotionAnimationClip")
                {
                    clip.displayName = rmClip.AnimationClip.name;
                }
            }
        }
    }

    /// <summary>
    /// Custom inspector for RootMotionAnimationClip
    /// </summary>
    [CustomEditor(typeof(RootMotionAnimationClip))]
    public class RootMotionAnimationClipInspector : UnityEditor.Editor
    {
        private SerializedProperty m_AnimationClip;
        private SerializedProperty m_ApplyFootIK;
        private SerializedProperty m_Loop;

        private bool m_ShowClipInfo = true;

        private static class Styles
        {
            public static readonly GUIContent AnimationClipLabel = new GUIContent(
                "Animation Clip",
                "The animation clip to play.");

            public static readonly GUIContent ApplyFootIKLabel = new GUIContent(
                "Apply Foot IK",
                "Whether to apply foot IK for humanoid characters.");

            public static readonly GUIContent LoopLabel = new GUIContent(
                "Loop",
                "Whether the animation should loop.");
        }

        private void OnEnable()
        {
            m_AnimationClip = serializedObject.FindProperty("m_AnimationClip");
            m_ApplyFootIK = serializedObject.FindProperty("m_ApplyFootIK");
            m_Loop = serializedObject.FindProperty("m_Loop");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Animation Clip field
            EditorGUILayout.PropertyField(m_AnimationClip, Styles.AnimationClipLabel);

            // Show clip info
            var clip = m_AnimationClip.objectReferenceValue as AnimationClip;
            if (clip != null)
            {
                EditorGUILayout.Space();
                
                m_ShowClipInfo = EditorGUILayout.Foldout(m_ShowClipInfo, "Clip Information", true);
                if (m_ShowClipInfo)
                {
                    EditorGUI.indentLevel++;
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.FloatField("Duration", clip.length);
                        EditorGUILayout.Toggle("Has Root Curves", clip.hasRootCurves);
                        EditorGUILayout.Toggle("Has Motion Curves", clip.hasMotionCurves);
                        EditorGUILayout.Toggle("Is Looping", clip.isLooping);
                        EditorGUILayout.Toggle("Is Humanoid", clip.humanMotion);
                    }
                    EditorGUI.indentLevel--;
                }

                // Root motion info
                if (clip.hasRootCurves || clip.hasMotionCurves)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox(
                        "This animation has root motion curves.\n" +
                        "Enable 'Apply Root Motion' on the Animator component if you want root motion to be applied.",
                        MessageType.Info);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Playback Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_Loop, Styles.LoopLabel);
            EditorGUILayout.PropertyField(m_ApplyFootIK, Styles.ApplyFootIKLabel);

            EditorGUILayout.Space();
            
            // Reset all button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset to Defaults", GUILayout.Width(120)))
            {
                m_Loop.boolValue = false;
                m_ApplyFootIK.boolValue = true;
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
