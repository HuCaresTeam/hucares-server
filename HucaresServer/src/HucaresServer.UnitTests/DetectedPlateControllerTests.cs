﻿using FakeItEasy;
using HucaresServer.Controllers;
using HucaresServer.Storage.Helpers;
using HucaresServer.Storage.Models;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HucaresServer.UnitTests
{
    public class DetectedPlateControllerTests
    {

        [Test]
        public async Task GetAllDetectedMissingPlates_WhenCalled_ShouldCallHelper()
        {
            //Arrange
            var fakeDetectedPlateHelper = A.Fake<IDetectedPlateHelper>();
            
            var expectedDLPList = new List<DetectedLicensePlate>() { new DetectedLicensePlate() { Id = 0 } };
            A.CallTo(() => fakeDetectedPlateHelper.GetAllDetectedMissingPlates(null))
                .Returns(expectedDLPList);

            var detectedPlateController = new DetectedPlateController() { DetectedPlateHelper = fakeDetectedPlateHelper, Request = new HttpRequestMessage() };

            //Act
            var result = detectedPlateController.GetAllDetectedMissingPlates();

            //Assert
            A.CallTo(() => fakeDetectedPlateHelper.GetAllDetectedMissingPlates(null))
                .MustHaveHappenedOnceExactly();

            var httpResponse = await result.ExecuteAsync(new CancellationToken());
            var jsonContent = await httpResponse.Content.ReadAsStringAsync();

            var expectedJson = JsonConvert.SerializeObject(expectedDLPList);

            jsonContent.ShouldBe(expectedJson);
        }

        [Test]
        public async Task GetAllDetectedPlatesByPlateNumber_WhenCalled_ShouldCallHelper()
        {
            //Arrange
            var fakeDetectedPlateHelper = A.Fake<IDetectedPlateHelper>();

            var expectedDLPList = new List<DetectedLicensePlate>() { new DetectedLicensePlate() { Id = 0 } };
            var expectedPlateNumber = "TRV456";
            var expectedDateStart = new DateTime(2016, 02, 02);
            var expectedDateEnd = new DateTime(2017, 02, 05);
            A.CallTo(() => fakeDetectedPlateHelper.GetAllActiveDetectedPlatesByPlateNumber(expectedPlateNumber, expectedDateStart, expectedDateEnd))
                .Returns(expectedDLPList);

            var detectedPlateController = new DetectedPlateController() { DetectedPlateHelper = fakeDetectedPlateHelper, Request = new HttpRequestMessage() };

            //Act
            var result = detectedPlateController.GetAllDetectedPlatesByPlateNumber(expectedPlateNumber, expectedDateStart, expectedDateEnd);

            //Assert
            A.CallTo(() => fakeDetectedPlateHelper.GetAllActiveDetectedPlatesByPlateNumber(expectedPlateNumber, expectedDateStart, expectedDateEnd))
                .MustHaveHappenedOnceExactly();

            var httpResponse = await result.ExecuteAsync(new CancellationToken());
            var jsonContent = await httpResponse.Content.ReadAsStringAsync();

            var expectedJson = JsonConvert.SerializeObject(expectedDLPList);

            jsonContent.ShouldBe(expectedJson);
        }

        [Test]
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

            var detectedPlateController = new DetectedPlateController() { DetectedPlateHelper = fakeDetectedPlateHelper, Request = new HttpRequestMessage() };

            //Act
            var result = detectedPlateController.GetAllDetectedPlatesByCamera(expectedCamId, expectedDateStart, expectedDateEnd);

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
