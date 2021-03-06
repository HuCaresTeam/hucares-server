﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using HucaresServer.Storage.Helpers;
using HucaresServer.Storage.Models;
using NUnit.Framework;
using Shouldly;

namespace HucaresServer.DataAcquisition.UnitTests
{
    [TestFixture]
    public class CameraInfoDownloadingTests
    {
        [Test]
        public async Task DownloadImagesFromCameraInfoSources_WithTrusted_ShouldDownloadAndReturn()
        {
            // Arrange
            var fakeBitmap = new Bitmap(100, 100);
            var url = "https://some.url";

            ImageConverter converter = new ImageConverter();
            var fakeImageArray = (byte[])converter.ConvertTo(fakeBitmap, typeof(byte[]));

            var fakeWebClient = A.Fake<IWebClient>();
            A.CallTo(() => fakeWebClient.DownloadData(url))
                .Returns(fakeImageArray);

            var fakeWebClientFactory = A.Fake<IWebClientFactory>();
            A.CallTo(() => fakeWebClientFactory.BuildWebClient())
                .Returns(fakeWebClient);

            var fakeMissingPlates = new List<CameraInfo>()
            {
                new CameraInfo() {Id = 0, HostUrl = url, IsTrustedSource = true},
            };

            var fakeCameraInfoHelper = A.Fake<ICameraInfoHelper>();
            A.CallTo(() => fakeCameraInfoHelper.GetActiveCameras(true))
                .Returns(fakeMissingPlates);

            var fakeImageSaver = A.Fake<IImageManipulator>();

            var cameraImageDownloading = new CameraImageDownloading(fakeCameraInfoHelper, fakeImageSaver, fakeWebClientFactory);

            // Act
            var resultParsedCount = await cameraImageDownloading.DownloadImagesFromCameraInfoSources(true, new DateTime(2018, 11, 01));

            // Assert
            A.CallTo(() => fakeCameraInfoHelper.GetActiveCameras(true)).MustHaveHappened();
            A.CallTo(() => fakeWebClient.DownloadData(url)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeImageSaver.SaveImage(0, new DateTime(2018, 11, 01), fakeImageArray)).MustHaveHappenedOnceExactly();

            resultParsedCount.ShouldBe(1);
        }
        
        [Test]
        public async Task DownloadImagesFromCameraInfoSources_WhenIsTrustedNull_ShouldDownloadAndReturn()
        {
            // Arrange
            var fakeBitmap = new Bitmap(100, 100);
            var url = "https://some.url";
            
            ImageConverter converter = new ImageConverter();
            var fakeImageArray =  (byte[])converter.ConvertTo(fakeBitmap, typeof(byte[]));
            
            
            var fakeWebClient = A.Fake<IWebClient>();
            A.CallTo(() => fakeWebClient.DownloadData(url))
                .Returns(fakeImageArray);
            
            var fakeWebClientFactory = A.Fake<IWebClientFactory>();
            A.CallTo(() => fakeWebClientFactory.BuildWebClient())
                .Returns(fakeWebClient);
            
            var fakeMissingPlates = new List<CameraInfo>()
            {
                new CameraInfo() {Id = 0, HostUrl = url, IsTrustedSource = true},
                new CameraInfo() {Id = 1, HostUrl = url, IsTrustedSource = false}
            };
            
            var fakeCameraInfoHelper = A.Fake<ICameraInfoHelper>();
            A.CallTo(() => fakeCameraInfoHelper.GetActiveCameras(null))
                .Returns(fakeMissingPlates);

            var fakeImageSaver = A.Fake<IImageManipulator>();

            var cameraImageDownloading = new CameraImageDownloading(fakeCameraInfoHelper, fakeImageSaver, fakeWebClientFactory);
            
            // Act
            var resultParsedCount = await cameraImageDownloading.DownloadImagesFromCameraInfoSources(downloadDateTime: new DateTime(2018, 11, 01));
            
            // Assert
            A.CallTo(() => fakeCameraInfoHelper.GetActiveCameras(null)).MustHaveHappened();
            A.CallTo(() => fakeWebClient.DownloadData(url)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => fakeImageSaver.SaveImage(A<int>.Ignored, new DateTime(2018, 11, 01), fakeImageArray)).MustHaveHappenedTwiceExactly();

            resultParsedCount.ShouldBe(2);
        }
        
        [Test]
        public async Task DownloadImagesFromCameraInfoSources_WhenNoCameras_ShouldDownloadAndReturn()
        {
            // Arrange
            var url = "https://some.url";
            
            var fakeWebClient = A.Fake<IWebClient>();
            
            var fakeWebClientFactory = A.Fake<IWebClientFactory>();
            A.CallTo(() => fakeWebClientFactory.BuildWebClient())
                .Returns(fakeWebClient);
            
            var fakeImageSaver = A.Fake<IImageManipulator>();
            
            var fakeCameras = new List<CameraInfo>();
            
            var fakeCameraInfoHelper = A.Fake<ICameraInfoHelper>();
            A.CallTo(() => fakeCameraInfoHelper.GetActiveCameras(null))
                .Returns(fakeCameras);
            
            var cameraImageDownloading = new CameraImageDownloading(fakeCameraInfoHelper, fakeImageSaver, fakeWebClientFactory);
            
            // Act
            var resultParsedCount = await cameraImageDownloading.DownloadImagesFromCameraInfoSources(downloadDateTime: new DateTime(2018, 11, 01));
            
            // Assert
            A.CallTo(() => fakeCameraInfoHelper.GetActiveCameras(null)).MustHaveHappened();
            A.CallTo(() => fakeWebClient.DownloadData(url)).MustNotHaveHappened();
            A.CallTo(() => fakeImageSaver.SaveImage(0, new DateTime(2018, 11, 01), A<Byte[]>.Ignored)).MustNotHaveHappened();

            resultParsedCount.ShouldBe(0);
        }
    }
}