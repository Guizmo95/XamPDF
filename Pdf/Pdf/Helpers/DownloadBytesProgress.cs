using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Helpers
{
    public class DownloadBytesProgress
    {
		public DownloadBytesProgress(string fileName, int bytesReceived, int totalBytes)
		{
			Filename = fileName;
			BytesReceived = bytesReceived;
			TotalBytes = totalBytes;
		}

		public int TotalBytes { get; private set; }

		public int BytesReceived { get; private set; }

		public float PercentComplete { get { return ((float)BytesReceived / TotalBytes)/2; } }

		//public float PercentComplete { get { return ((float)BytesReceived / TotalBytes)/2; } }

		public string Filename { get; private set; }

		public bool IsFinished { get { return BytesReceived == TotalBytes; } }
	}
}
