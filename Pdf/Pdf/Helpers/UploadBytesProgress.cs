using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Helpers
{
    public class UploadBytesProgress
    {
		public UploadBytesProgress(string fileName, int bytesSended, int totalBytes)
		{
			Filename = fileName;
			BytesSended = bytesSended;
			TotalBytes = totalBytes;
		}

		public int TotalBytes { get; private set; }

		public int BytesSended { get; private set; }

		public float PercentComplete { get { return (float)BytesSended / TotalBytes; } }

		public string Filename { get; private set; }

		public bool IsFinished { get { return BytesSended == TotalBytes; } }
	}
}
