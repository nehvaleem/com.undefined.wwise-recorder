using System;
using System.IO;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEngine;

namespace Undefined.WwiseRecorder.Editor
{
    class WwiseRecorder : GenericRecorder<WwiseRecorderSettings>
    {
        protected ulong OutputDeviceId = 0;
        
        private uint _sampleRate;
        private uint _channels;
        
        private WavEncoder _wavEncoder;
        
        protected override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session))
                return false;

            try
            {
                Settings.FileNameGenerator.CreateDirectory(session);
            }
            catch (Exception)
            {
                ConsoleLogMessage($"Unable to create the output directory \"{Settings.FileNameGenerator.BuildAbsolutePath(session)}\".", LogType.Error);
                Recording = false;
                return false;
            }
            
            OutputDeviceId = AkSoundEngine.GetOutputID(AkSoundEngine.AK_INVALID_UNIQUE_ID, 0);
            AkSoundEngineController.Instance.DisableEditorLateUpdate();
            
            _sampleRate = AkSoundEngine.GetSampleRate();
            _channels = new AkChannelConfig().uNumChannels;
            
            AkSoundEngine.ClearCaptureData();
            AkSoundEngine.StartDeviceCapture(OutputDeviceId);
            
            try
            {
                var path =  Settings.FileNameGenerator.BuildAbsolutePath(session);
                _wavEncoder = new WavEncoder(path, (int)_sampleRate, (int)_channels);

                return true;
            }
            catch (Exception ex)
            {
                if (RecorderOptions.VerboseMode)
                    ConsoleLogMessage($"Unable to create encoder: '{ex.Message}'", LogType.Error);
            }

            return false;
        }
        

        protected override void RecordFrame(RecordingSession ctx)
        {
            var frameTime = 1.0f / Settings.FrameRate;
            
            AkSoundEngine.SetOfflineRenderingFrameTime(frameTime);
            AkSoundEngine.SetOfflineRendering(true);
            
            var sampleCount = AkSoundEngine.UpdateCaptureSampleCount(OutputDeviceId);
            if (sampleCount <= 0)
                return;

            var frameBuffer = new float[sampleCount];

            var count = AkSoundEngine.GetCaptureSamples(OutputDeviceId, frameBuffer, (uint)frameBuffer.Length);
            if (count > 0)
            {
                _wavEncoder.AddSamples(frameBuffer);
            }
        }
        
        protected override void EndRecording(RecordingSession session)
        {
            AkSoundEngine.StopDeviceCapture(OutputDeviceId);
            AkSoundEngine.SetOfflineRendering(false);
            AkSoundEngineController.Instance.EnableEditorLateUpdate();

            if (_wavEncoder != null)
            {
                _wavEncoder.Dispose();
                _wavEncoder = null;
            }

            // When adding a file to Unity's assets directory, trigger a refresh so it is detected.
            if (Settings.FileNameGenerator.Root is OutputPath.Root.AssetsFolder or OutputPath.Root.StreamingAssets)
                AssetDatabase.Refresh();
            
            base.EndRecording(session);
        }
    }
    
    internal class WavEncoder
    {
        BinaryWriter _binwriter;
        private int _sampleRate;
        private int _channels;

        public WavEncoder(string filename, int sampleRate, int channels)
        {
            _sampleRate = sampleRate;
            _channels = channels;
            var stream = new FileStream(filename, FileMode.Create);
            _binwriter = new BinaryWriter(stream);
            for (int n = 0; n < 44; n++)
                _binwriter.Write((byte)0);
        }

        public void Stop()
        {
            var closewriter = _binwriter;
            _binwriter = null;
            int subformat = 3; // float
            int numchannels = AudioSettings.speakerMode == AudioSpeakerMode.Mono ? 1 : 2;
            int numbits = 32;
            int samplerate = AudioSettings.outputSampleRate;

            long pos = closewriter.BaseStream.Length;
            closewriter.Seek(0, SeekOrigin.Begin);
            closewriter.Write((byte) 'R');
            closewriter.Write((byte) 'I');
            closewriter.Write((byte) 'F');
            closewriter.Write((byte) 'F');
            closewriter.Write((uint) (pos - 8));
            closewriter.Write((byte) 'W');
            closewriter.Write((byte) 'A');
            closewriter.Write((byte) 'V');
            closewriter.Write((byte) 'E');
            closewriter.Write((byte) 'f');
            closewriter.Write((byte) 'm');
            closewriter.Write((byte) 't');
            closewriter.Write((byte) ' ');
            closewriter.Write((uint) 16);
            closewriter.Write((ushort) subformat);
            closewriter.Write((ushort) numchannels);
            closewriter.Write((uint) samplerate);
            closewriter.Write((uint) ((samplerate * numchannels * numbits) / 8));
            closewriter.Write((ushort) ((numchannels * numbits) / 8));
            closewriter.Write((ushort) numbits);
            closewriter.Write((byte) 'd');
            closewriter.Write((byte) 'a');
            closewriter.Write((byte) 't');
            closewriter.Write((byte) 'a');
            closewriter.Write((uint) (pos - 36));
            closewriter.Seek((int) pos, SeekOrigin.Begin);
            closewriter.Flush();
            closewriter.Close();
        }

        public void AddSamples(float[] data)
        {
            if (RecorderOptions.VerboseMode)
                Debug.Log("Writing wav chunk " + data.Length);

            if (_binwriter == null)
                return;

            for (var n = 0; n < data.Length; n++)
                _binwriter.Write(data[n]);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}