#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;
using FM.Cinematics;

namespace FM.Cinematics.Editor
{
    /// <summary>
    /// Custom editor for RootMotionAnimationTrack
    /// </summary>
    [CustomTimelineEditor(typeof(RootMotionAnimationTrack))]
    public class RootMotionAnimationTrackEditor : TrackEditor
    {
        private static readonly Color s_TrackColor = new Color(0.53f, 0.0f, 0.08f);

        public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
        {
            var options = base.GetTrackOptions(track, binding);
            options.trackColor = s_TrackColor;
            return options;
        }

        public override void OnCreate(TrackAsset track, TrackAsset copiedFrom)
        {
            base.OnCreate(track, copiedFrom);
        }
    }

    /// <summary>
    /// Custom inspector for RootMotionAnimationTrack
    /// </summary>
    [CustomEditor(typeof(RootMotionAnimationTrack))]
    public class RootMotionAnimationTrackInspector : UnityEditor.Editor
    {
        private SerializedProperty m_AvatarMask;
        private SerializedProperty m_ApplyAvatarMask;
        private SerializedProperty m_ApplyFootIK;

        private static class Styles
        {
            public static readonly GUIContent AvatarMaskLabel = new GUIContent(
                "Avatar Mask",
                "Optional avatar mask to filter which parts of the animation are applied.");

            public static readonly GUIContent ApplyAvatarMaskLabel = new GUIContent(
                "Apply Avatar Mask",
                "Whether to apply the avatar mask to this track.");

            public static readonly GUIContent ApplyFootIKLabel = new GUIContent(
                "Apply Foot IK",
                "Whether to apply foot IK for humanoid characters.");
        }

        private void OnEnable()
        {
            m_AvatarMask = serializedObject.FindProperty("m_AvatarMask");
            m_ApplyAvatarMask = serializedObject.FindProperty("m_ApplyAvatarMask");
            m_ApplyFootIK = serializedObject.FindProperty("m_ApplyFootIK");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(m_ApplyFootIK, Styles.ApplyFootIKLabel);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Avatar Mask", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(m_ApplyAvatarMask, Styles.ApplyAvatarMaskLabel);
            
            if (m_ApplyAvatarMask.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_AvatarMask, Styles.AvatarMaskLabel);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            
            // Info about root motion
            EditorGUILayout.HelpBox(
                "This track plays animations via Timeline.\n\n" +
                "Root motion is NOT applied by this track. If you need root motion, " +
                "enable 'Apply Root Motion' on the Animator component directly.",
                MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
