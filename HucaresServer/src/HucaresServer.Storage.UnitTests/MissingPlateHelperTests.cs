using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using HucaresServer.Storage.Helpers;
using FakeItEasy;
using HucaresServer.Storage.Models;
using NUnit.Framework;
using Shouldly;

namespace HucaresServer.Storage.UnitTests
{
    public class MissingLicensePlateTests
    {
        [Test]
        public void InsertMissingPlate_WhenPlateNumberIsValid_ShouldSucceedAndReturnExpected()
        {
            //Arrange
            var fakeIQueryable = new List<MissingLicensePlate>().AsQueryable();
            var fakeDbSet = A.Fake<DbSet<MissingLicensePlate>>();

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingLicensePlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            //Act
            var expectedPlate = "JBA:555";
            var expectedDate = new DateTime(2017, 08, 15);
            var result = missingLicensePlateHelper.InsertPlateRecord(expectedPlate, expectedDate);

            //Assert
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeDbSet.Add(result))
                .MustHaveHappened();

            A.CallTo(() => fakeHucaresContext.SaveChanges())
                .MustHaveHappenedOnceExactly();

            result.PlateNumber.ShouldBe(expectedPlate);
            result.SearchStartDateTime.ShouldBe(expectedDate);
        }

        [Test]
        public void InsertPlateRecord_WhenPlateNumberIsNotValid_ShouldThrowAnError()
        {
            //Arrange
            var fakeDbSet = A.Fake<DbSet<MissingLicensePlate>>();

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            var dateInput = "Jan 1, 2018";
            DateTime expectedDate = DateTime.Parse(dateInput);
            
            //Act & Assert
            //TODO
            //Assert.ThrowsException<ArgumentException>(() => missingPlateHelper.InsertPlateRecord("5555555", expectedDate));

            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .MustNotHaveHappened();
        }

        [Test]
        public void GetAllPlateRecords_WhenDbIsEmpty_ShouldThrowEmptyDB()
        {
            //Arrange
            var fakeIQueryable = new List<MissingLicensePlate>().AsQueryable();

            var fakeDbSet = StorageTestsUtil.SetupFakeDbSet(fakeIQueryable);

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            //Act
            var result = missingPlateHelper.GetAllPlateRecords();

            //Assert
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeHucaresContext.SaveChanges())
                .MustNotHaveHappened();

            result.Count().ShouldBe(0);
        }
        
        [Test]
        public void GetAllPlates_WhenDbIsNotEmpty_ShouldReturnExpected()
        {
            var expectedDate = new DateTime(2018, 05, 15);
            
            //Arrange
            var fakeIQueryable = new List<MissingLicensePlate>()
            {
                new MissingLicensePlate() {PlateNumber = "JOR:154", SearchStartDateTime = expectedDate},
                new MissingLicensePlate() {PlateNumber = "JUA:222", SearchStartDateTime = expectedDate}
            }.AsQueryable();

            var fakeDbSet = StorageTestsUtil.SetupFakeDbSet(fakeIQueryable);

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            //Act
            var result = missingPlateHelper.GetAllPlateRecords();

            //Assert
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeHucaresContext.SaveChanges())
                .MustNotHaveHappened();

            result.Count().ShouldBe(fakeIQueryable.Count());
            Assert.IsTrue(result.SequenceEqual(fakeIQueryable.ToList()), "Lists are not equal");
        }

        [Test]
        public void GetPlateRecordByPlateNumber_WhenPlateNumberExist_ShouldReturnExpected()
        {
            //Arrange
            var fakeIQueryable = new List<MissingLicensePlate>()
            {
                new MissingLicensePlate() { PlateNumber = "ZOA:555", SearchStartDateTime = new DateTime(2018, 11, 04)},
                new MissingLicensePlate() { PlateNumber = "DAD:123", SearchStartDateTime = new DateTime(2018, 10, 04) },
                new MissingLicensePlate() { PlateNumber = "FEF:144", SearchStartDateTime = new DateTime(2018, 12, 04) }
            }.AsQueryable();

            var fakeDbSet = StorageTestsUtil.SetupFakeDbSet(fakeIQueryable);

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            var expectedPlateNumber = "ZOA:555";
            //Act
            var result = missingPlateHelper.GetPlateRecordByPlateNumber(expectedPlateNumber);

            //Assert
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeHucaresContext.SaveChanges())
                .MustNotHaveHappened();

            var expectedResult = fakeIQueryable.Where(c => c.PlateNumber == expectedPlateNumber);
            result.Count().ShouldBe(expectedResult.Count());
            Assert.IsTrue(result.SequenceEqual(expectedResult.ToList()), "Lists are not equal");
        }
        
        [Test]
        public void GetPlateRecordByPlateNumber_WhenPlateNumberNotExist_ShouldSucceedAndReturnNull()
        {
            //Arrange
            var fakeIQueryable = new List<MissingLicensePlate>()
            {
                new MissingLicensePlate() { PlateNumber = "OOO:111"}
            }.AsQueryable();

            var fakeDbSet = StorageTestsUtil.SetupFakeDbSet(fakeIQueryable);

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            //Act
            var result = missingPlateHelper.GetPlateRecordByPlateNumber("QQQ:333");

            //Assert
            result.ShouldBeEmpty();
        }

        [Test]
        public void UpdatePlateRecord_WhenRecordWithIdDoesNotExist_ShouldThrow()
        {
            //Arrange
            var fakeIQueryable = new List<MissingLicensePlate>().AsQueryable();
            var fakeDbSet = StorageTestsUtil.SetupFakeDbSet(fakeIQueryable);

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            //Act & Assert
            Assert.Throws<ArgumentException>(() => missingPlateHelper.UpdatePlateRecord(0, "JBA:514", DateTime.Today));
        }

        [Test]
        public void UpdatePlateRecord_WhenRecordWithIdExist_ShouldUpdateRecord()
        {
            //Arrange
            var missingPlateObj = new MissingLicensePlate() { Id = 1, PlateNumber = "FOF:150"};
            var fakeIQueryable = new List<MissingLicensePlate>(){ missingPlateObj }.AsQueryable();

            var fakeDbSet = StorageTestsUtil.SetupFakeDbSet(fakeIQueryable);

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            //Act
            var expectedPlateNumber = "TRE:145";
            var result = missingPlateHelper.UpdatePlateRecord(missingPlateObj.Id, expectedPlateNumber, DateTime.Now);

            //Assert
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeDbSet.Add(A<MissingLicensePlate>.Ignored))
                .MustNotHaveHappened();

            A.CallTo(() => fakeHucaresContext.SaveChanges())
                .MustHaveHappenedOnceExactly();

            result.ShouldBe(missingPlateObj);
            missingPlateObj.PlateNumber.ShouldBe(expectedPlateNumber);
        }
        
        [Test]
        public void MarkFoundPlate_WhenPlateIdExist_ShouldSucceed()
        {
            var missingPlateObj = new MissingLicensePlate() { Id = 1, SearchStartDateTime = new DateTime(2018, 05, 08), LicensePlateFound = false};
            //Arrange
            var fakeIQueryable = new List<MissingLicensePlate>() { missingPlateObj }.AsQueryable();
            var fakeDbSet = StorageTestsUtil.SetupFakeDbSet(fakeIQueryable);

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var expectedSearch = true;
            
            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);
            var result = missingPlateHelper.MarkFoundPlate(missingPlateObj.Id, DateTime.Now, expectedSearch);

            //Act & Assert
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeDbSet.Add(A<MissingLicensePlate>.Ignored))
                .MustNotHaveHappened();

            A.CallTo(() => fakeHucaresContext.SaveChanges())
                .MustHaveHappenedOnceExactly();

            result.ShouldBe(missingPlateObj);
            missingPlateObj.LicensePlateFound.ShouldBe(true);
        }
        
        [Test]
        public void MarkFoundPlate_WhenPlateIdDoesNotExist_ShouldThrow()
        {
            //Arrange
            var fakeIQueryable = new List<MissingLicensePlate>().AsQueryable();
            var fakeDbSet = StorageTestsUtil.SetupFakeDbSet(fakeIQueryable);

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            var expectedId = 5;
            var expectedStartSearchDateTime = new DateTime(2018, 08, 17);
            var expectedSearch = true;

            //Act & Assert
            Assert.Throws<ArgumentException>(() => missingPlateHelper.MarkFoundPlate(expectedId, expectedStartSearchDateTime, expectedSearch));

            A.CallTo(() => fakeHucaresContext.SaveChanges())
                .MustNotHaveHappened();
        }
        
        [Test]
        public void DeletePlateById_WhenPlateIdExist_ShouldSucceed()
        {
            //Arrange
            var missingPlateObj = new MissingLicensePlate() { Id = 1 };
            var fakeIQueryable = new List<MissingLicensePlate>()
            {
                new MissingLicensePlate() { Id = 0 },
                missingPlateObj 
            }.AsQueryable();

            var fakeDbSet = StorageTestsUtil.SetupFakeDbSet(fakeIQueryable);

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            //Act
            var result = missingPlateHelper.DeletePlateById(missingPlateObj.Id);

            //Assert
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeDbSet.Remove(missingPlateObj))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeHucaresContext.SaveChanges())
                .MustHaveHappenedOnceExactly();

            result.ShouldBe(missingPlateObj);  
        }
        
        [Test]
        public void DeletePlateById_WhenPlateWithIdDoesNotExist_ShouldThrow()
        {
            //Arrange
            var fakeIQueryable = new List<MissingLicensePlate>().AsQueryable();
            var fakeDbSet = StorageTestsUtil.SetupFakeDbSet(fakeIQueryable);

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            //Act & Assert
            Assert.Throws<ArgumentException>(() => missingPlateHelper.DeletePlateById(0));

            A.CallTo(() => fakeHucaresContext.SaveChanges())
                .MustNotHaveHappened();
        }
        
        [Test]
        public void DeletePlateByNumber_WhenPlateNumberExist_ShouldSucceed()
        {
            //Arrange
            var missingPlateObj = new MissingLicensePlate() { PlateNumber = "ZOO:555" };
            var fakeIQueryable = new List<MissingLicensePlate>()
            {
                new MissingLicensePlate() { PlateNumber = "ZZZ:123"},
                missingPlateObj 
            }.AsQueryable();

            var fakeDbSet = StorageTestsUtil.SetupFakeDbSet(fakeIQueryable);

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            //Act
            var result = missingPlateHelper.DeletePlateByNumber(missingPlateObj.PlateNumber);

            //Assert
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeDbSet.Remove(missingPlateObj))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeHucaresContext.SaveChanges())
                .MustHaveHappenedOnceExactly();

            result.ShouldBe(missingPlateObj);  
        }
        
        [Test]
        public void DeletePlateByNumber_WhenPlateWithNumberDoesNotExist_ShouldThrow()
        {
            //Arrange
            var fakeIQueryable = new List<MissingLicensePlate>().AsQueryable();
            var fakeDbSet = StorageTestsUtil.SetupFakeDbSet(fakeIQueryable);

            var fakeHucaresContext = A.Fake<HucaresContext>();
            A.CallTo(() => fakeHucaresContext.MissingLicensePlates)
                .Returns(fakeDbSet);

            var fakeDbContextFactory = A.Fake<IDbContextFactory>();
            A.CallTo(() => fakeDbContextFactory.BuildHucaresContext())
                .Returns(fakeHucaresContext);

            var missingPlateHelper = new MissingPlateHelper(fakeDbContextFactory);

            //Act & Assert
            Assert.Throws<ArgumentException>(() => missingPlateHelper.DeletePlateByNumber("TRO:555"));

            A.CallTo(() => fakeHucaresContext.SaveChanges())
                .MustNotHaveHappened();
        }
    }
}