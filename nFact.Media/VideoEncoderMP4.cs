using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using HandBrake.Interop;
using HandBrake.Interop.EventArgs;
using HandBrake.Interop.Model;
using HandBrake.Interop.Model.Encoding;
using nFact.Shared;

namespace nFact.Media
{
    public class VideoEncoderMP4
    {
        private readonly HandBrakeInstance instance;
        private string _sourceFile;
        private string _outputFile;
        private volatile bool _running;

        private readonly Queue<ConvertJob> _mediaQueue = new Queue<ConvertJob>();

        private ScriptLogger _logger = new ScriptLogger();

        public VideoEncoderMP4()
        {
            instance = new HandBrakeInstance();
            instance.Initialize(verbosity: 0);
            instance.ScanCompleted += OnScanCompleted;
            instance.EncodeProgress += OnProgress;
            instance.EncodeCompleted += OnEncodeCompleted;
        }

        private void OnProgress(object sender, EncodeProgressEventArgs e)
        {
            _logger.LogProgress("...");
        }

        public void Reset()
        {
            _mediaQueue.Clear();
        }

        public void Encode(string sourceFile, string outputFile)
        {
            var job = new ConvertJob(sourceFile, outputFile);
            if (_running)
            {
                _mediaQueue.Enqueue(job);
                return;
            }

            _logger.Log(string.Format("Encoding {0} ...", _sourceFile));
            _running = true;
            _sourceFile = sourceFile;
            _outputFile = outputFile;
            instance.StartScan(_sourceFile, previewCount: 10);
        }

        void OnScanCompleted(object sender, EventArgs e)
        {
            _logger.Log(string.Format("Scan complete of {0}", _sourceFile));

            var serializer = new XmlSerializer(typeof(EncodingProfile));
            EncodingProfile profile;
            using (var stream = new FileStream("VideoEncoding.xml", FileMode.Open, FileAccess.Read))
            {
                profile = serializer.Deserialize(stream) as EncodingProfile;
            }

            var job = new EncodeJob
            {
                EncodingProfile = profile,
                RangeType = VideoRangeType.All,
                Title = 1,
                SourcePath = _sourceFile,
                OutputPath = _outputFile,
                ChosenAudioTracks = new List<int> { 1 },
                Subtitles = new Subtitles
                {
                    SourceSubtitles = new List<SourceSubtitle>(),
                    SrtSubtitles = new List<SrtSubtitle>()
                }
            };
            instance.StartEncode(job);
        }

        void OnEncodeCompleted(object sender, HandBrake.Interop.EventArgs.EncodeCompletedEventArgs e)
        {
            _logger.Log("Encoding complete");
            _running = false;
            if (_mediaQueue.Count > 0)
            {
                var job = _mediaQueue.Dequeue();
                Encode(job.SourceFile, job.DestFile);
            }
        }
    }
}
