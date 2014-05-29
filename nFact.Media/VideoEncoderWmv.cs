using System;
using System.Collections.Generic;
using Microsoft.Expression.Encoder;

namespace nFact.Media
{
    public class VideoEncoderWmv
    {
        private volatile bool _running;

        private readonly Queue<ConvertJob> _mediaQueue = new Queue<ConvertJob>();

        public void Reset()
        {
            _mediaQueue.Clear();
        }

        public void Encode(string filePathXesc, string outputPath)
        {
            var job = new ConvertJob(filePathXesc, outputPath);
            if (_running)
            {
                _mediaQueue.Enqueue(job);
                return;
            }

            _running = true;

            Console.WriteLine("Scanning file: '{0}", filePathXesc);
            var mediaItem = new MediaItem(filePathXesc);
            var originalSize = mediaItem.OriginalVideoSize;
            mediaItem.ApplyPreset("preset.xml");

            // verifies encoding of file
            Console.WriteLine("\nEncoding: {0}", filePathXesc);

            // Create a job and the media item for the video we wish
            // to encode.
            var encode = new Job();
            encode.MediaItems.Add(mediaItem);

            // Set up the progress callback function
            encode.EncodeCompleted += OnCompleted;

            // Set the output directory and encode.
            encode.OutputDirectory = outputPath;
            encode.CreateSubfolder = false;

            // encodes job
            encode.Encode();
            encode.Dispose();
        }

        void OnCompleted(object sender, EncodeCompletedEventArgs e)
        {
            Console.WriteLine("Video Convertor complete");
            _running = false;
            if (_mediaQueue.Count > 0)
            {
                var job = _mediaQueue.Dequeue();
                Encode(job.SourceFile, job.DestFile);
            }
        }
    }
}