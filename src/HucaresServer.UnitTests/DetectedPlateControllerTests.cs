﻿using FakeItEasy;
using HucaresServer.Controllers;
using HucaresServer.Storage.Helpers;
using HucaresServer.Storage.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HucaresServer.Models.CameraInfoDataModels;

namespace HucaresServer.Storage.UnitTests
{
    [TestClass]
    public class DetectedPlateControllerTests
    {

        [TestMethod]
        public async Task GetAllDetectedMissingPlates_WhenCalled_ShouldCallHelper()
        {
            //Arrange
            var fakeDetectedPlateHelper = A.Fake<IDetectedPlateHelper>();

            var expectedDLPList = new List<DetectedLicensePlate>() { new DetectedLicensePlate() { Id = 0 } };
            A.CallTo(() => fakeDetectedPlateHelper.GetAllDetectedMissingPlates())
                .Returns(expectedDLPList);

            var cameraController = new DetectedPlateController() { DetectedPlateHelper = fakeDetectedPlateHelper, Request = new HttpRequestMessage() };

            //Act
            var result = cameraController.GetAllDetectedMissingPlates();

            //Assert
            A.CallTo(() => fakeDetectedPlateHelper.GetAllDetectedMissingPlates())
                .MustHaveHappenedOnceExactly();

            var httpResponse = await result.ExecuteAsync(new CancellationToken());
            var jsonContent = await httpResponse.Content.ReadAsStringAsync();

            var expectedJson = JsonConvert.SerializeObject(expectedDLPList);

            jsonContent.ShouldBe(expectedJson);
        }

        [TestMethod]
        public async Task GetAllDetectedPlatesByPlateNumber_WhenCalled_ShouldCallHelper()
        {
            //Arrange
            var fakeDetectedPlateHelper = A.Fake<IDetectedPlateHelper>();

            var expectedDLPList = new List<DetectedLicensePlate>() { new DetectedLicensePlate() { Id = 0 } };
            var expectedPlateNumber = "123456";
            var expectedDateStart = new DateTime(2016, 02, 02);
            var expectedDateEnd = new DateTime(2017, 02, 05);
            A.CallTo(() => fakeDetectedPlateHelper.GetAllDetectedPlatesByPlateNumber(expectedPlateNumber, expectedDateStart, expectedDateEnd))
                .Returns(expectedDLPList);

            var cameraController = new DetectedPlateController() { DetectedPlateHelper = fakeDetectedPlateHelper, Request = new HttpRequestMessage() };

            //Act
            var result = cameraController.GetAllDetectedPlatesByPlateNumber(expectedPlateNumber, expectedDateStart, expectedDateEnd);

            //Assert
            A.CallTo(() => fakeDetectedPlateHelper.GetAllDetectedPlatesByPlateNumber(expectedPlateNumber, expectedDateStart, expectedDateEnd))
                .MustHaveHappenedOnceExactly();

            var httpResponse = await result.ExecuteAsync(new CancellationToken());
            var jsonContent = await httpResponse.Content.ReadAsStringAsync();

            var expectedJson = JsonConvert.SerializeObject(expectedDLPList);

            jsonContent.ShouldBe(expectedJson);
        }

        [TestMethod]
        public async Task GetAllDetectedPlatesByCamera_WhenCalled_ShouldCallHelper()
        {
            //Arrange
            var fakeDetectedPlateHelper = A.Fake<IDetectedPlateHelper>();

            var expectedDLPList = new List<DetectedLicensePlate>() { new DetectedLicensePlate() { Id = 0 } };
            var expectedCamId = 0;
            var expectedDateStart = new DateTime(2016, 02, 02);
            var expectedDateEnd = new DateTime(2017, 02, 05);
            A.CallTo(() => fakeDetectedPlateHelper.GetAllDetectedPlatesByCamera(expectedCamId, expectedDateStart, expectedDateEnd))
                .Returns(expectedDLPList);

            var cameraController = new DetectedPlateController() { DetectedPlateHelper = fakeDetectedPlateHelper, Request = new HttpRequestMessage() };

            //Act
            var result = cameraController.GetAllDetectedPlatesByCamera(expectedCamId, expectedDateStart, expectedDateEnd);

            //Assert
            A.CallTo(() => fakeDetectedPlateHelper.GetAllDetectedPlatesByCamera(expectedCamId, expectedDateStart, expectedDateEnd))
                .MustHaveHappenedOnceExactly();

            var httpResponse = await result.ExecuteAsync(new CancellationToken());
            var jsonContent = await httpResponse.Content.ReadAsStringAsync();

            var expectedJson = JsonConvert.SerializeObject(expectedDLPList);

            jsonContent.ShouldBe(expectedJson);
        }
    }
}
