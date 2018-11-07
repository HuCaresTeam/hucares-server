﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HucaresServer.Storage.Helpers;

namespace HucaresServer.DataAcquisition
{
    public class CameraImageDownloading : ICameraImageDownloading
    {
        // TODO: Take this value from a configuration file or somewhere else
        private const string TemporaryStorageUrl = "/temporaryImages";

        private ICameraInfoHelper _cameraInfoHelper;
        private IImageSaver _imageSaver;
        private IWebClientFactory _webClientFactory;

        public CameraImageDownloading(ICameraInfoHelper cameraInfoHelper = null, IImageSaver imageSaver = null,
            IWebClientFactory webClientFactory = null)
        {
            _cameraInfoHelper = cameraInfoHelper ?? new CameraInfoHelper();
            _imageSaver = imageSaver ?? new LocalImageSaver(TemporaryStorageUrl);
            _webClientFactory = webClientFactory ?? new CustomWebClientFactory();
        }

        public int DownloadImagesFromCameraInfoSources(bool? isTrusted = null, DateTime? downloadDateTime = null)
        {
            var cameraDataToDownload = _cameraInfoHelper.GetActiveCameras(isTrusted).ToList();
            var imageSavingTasks = new List<Task>();

            var datetime = downloadDateTime ?? DateTime.Now;

            foreach (var cameraData in cameraDataToDownload)
            {
                imageSavingTasks.Add(Task.Factory.StartNew(
                    () => DownloadAndSaveImage(cameraData.HostUrl, cameraData.Id, downloadDateTime)));
            }

            Task.WaitAll(imageSavingTasks.ToArray());

            return cameraDataToDownload.Count;
        }

        private void DownloadAndSaveImage(string imageUrl, int cameraId, DateTime? captureDateTime)
        {
            using (var webClient = _webClientFactory.BuildWebClient())
            {
                var imageData = webClient.DownloadData(imageUrl);

                using (var memoryStream = new MemoryStream(imageData))
                {
                    _imageSaver.SaveImage(new Bitmap(memoryStream), cameraId, captureDateTime);
                }
            }
        }
    }
}