﻿using Pdf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Pdf.Helpers
{
    public class DownloadHelper
    {
        public static readonly int BufferSize = 4096;
		
		//public static async Task<byte[]> CreateDownloadTask(string urlToDownload, IProgress<DownloadBytesProgress> progessReporter)
		//{
		//	int receivedBytes = 0;
		//	int totalBytes = 0;
		//	WebClient client = new WebClient();

		//	using (var stream = await client.OpenReadTaskAsync(urlToDownload))
		//	{

		//		byte[] buffer = new byte[BufferSize];
		//		totalBytes = Int32.Parse(client.ResponseHeaders[HttpResponseHeader.ContentLength]);
		//		using (MemoryStream memoryStream = new MemoryStream())
		//		{
		//			for (; ; )
		//			{
		//				int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
		//				memoryStream.Write(buffer, 0, buffer.Length);
		//				if (bytesRead == 0)
		//				{
		//					await Task.Yield();
		//					break;
		//				}

		//				receivedBytes += bytesRead;
		//				if (progessReporter != null)
		//				{
		//					DownloadBytesProgress args = new DownloadBytesProgress(urlToDownload, receivedBytes, totalBytes);
		//					progessReporter.Report(args);
		//				}
		//			}
		//			return memoryStream.ToArray();
		//		}
		//	}

		//}

		public static async Task CreateDownloadTask(string urlToDownload, string fileName, IProgress<DownloadBytesProgress> progessReporter)
		{
			int receivedBytes = 0;
			int totalBytes = 0;
			WebClient client = new WebClient();
			string directoryDownload = (string)global::Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);

			using (Stream fileStream = File.Create(Path.Combine(directoryDownload, fileName)))
			{
				using (var stream = await client.OpenReadTaskAsync(urlToDownload + fileName))
				{

					byte[] buffer = new byte[BufferSize];
					totalBytes = Int32.Parse(client.ResponseHeaders[HttpResponseHeader.ContentLength]);
					for (; ; )
					{
						int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

						if (bytesRead == 0)
						{
							await Task.Yield();
							break;
						}

						fileStream.Write(buffer, 0, bytesRead);

						receivedBytes += bytesRead;
						if (progessReporter != null)
						{
							DownloadBytesProgress args = new DownloadBytesProgress(urlToDownload, receivedBytes, totalBytes);
							progessReporter.Report(args);
						}
					}
				}
			}
		}
	}
}
