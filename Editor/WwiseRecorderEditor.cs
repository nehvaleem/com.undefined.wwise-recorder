using UnityEditor;
using UnityEditor.Recorder;
using UnityEngine;

namespace Undefined.WwiseRecorder.Editor
{
    [CustomEditor(typeof(WwiseRecorderSettings))]
    class WwiseRecorderEditor : RecorderEditor
    {
        SerializedProperty m_OutputFormat;

        static class Styles
        {
            internal static readonly GUIContent FormatLabel = new GUIContent("Format");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            m_OutputFormat = serializedObject.FindProperty("outputFormat");
        }

        protected override void FileTypeAndFormatGUI()
        {
            EditorGUILayout.PropertyField(m_OutputFormat, Styles.FormatLabel);
        }

        protected override void ImageRenderOptionsGUI()
        {
        }
    }
}