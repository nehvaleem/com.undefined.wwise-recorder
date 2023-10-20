using System;
using System.ComponentModel;
using UnityEditor.Recorder;

namespace Undefined.WwiseRecorder.Editor
{
    /// <summary>
    /// Use this class to manage all the information required to record audio from a Scene.
    /// </summary>
    
    [DisplayName("Audio"), Serializable]
    public class WwiseAudioInputSettings : RecorderInputSettings
    {
        protected override Type InputType => typeof(WwiseAudioInput);
    }
}