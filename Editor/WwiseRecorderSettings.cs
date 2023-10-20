using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEngine;

namespace Undefined.WwiseRecorder.Editor
{
    internal enum AudioRecorderOutputFormat
    {
        WAV
    }

    [RecorderSettings(typeof(WwiseRecorder), "Wwise Audio")]
    class WwiseRecorderSettings : RecorderSettings
    {
        public AudioRecorderOutputFormat  outputFormat = AudioRecorderOutputFormat.WAV;

        [SerializeField] WwiseAudioInputSettings m_AudioInputSettings = new WwiseAudioInputSettings();

        protected override string Extension
        {
            get { return outputFormat.ToString().ToLower(); }
        }

        public WwiseAudioInputSettings audioInputSettings
        {
            get { return m_AudioInputSettings; }
        }

        public override IEnumerable<RecorderInputSettings> InputsSettings
        {
            get { yield return m_AudioInputSettings; }
        }


        public WwiseRecorderSettings()
        {
            FileNameGenerator.FileName = "wwise_" + DefaultWildcard.Take;
        }
    }
}