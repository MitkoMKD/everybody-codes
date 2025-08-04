using CameraSearch.Repository;
using everybody_codes.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CameraSearch.UnitTests {
    public class CameraRepositoryTests : IDisposable {
        private readonly string _testDataPath;
        private readonly string _testFilePath;
        private readonly Mock<ILogger<CameraRepository>> _mockLogger;
        private readonly CameraRepository _repository;

        public CameraRepositoryTests() {
            _testDataPath = Path.Combine(AppContext.BaseDirectory, "TestData");
            Directory.CreateDirectory(_testDataPath);

            _testFilePath = Path.Combine(_testDataPath, "test.csv");

            var mockOptions = new Mock<IOptions<DataSettings>>();
            mockOptions.Setup(o => o.Value).Returns(new DataSettings {
                BaseDataPath = "TestData",
                FileName = "test.csv"
            });

            _mockLogger = new Mock<ILogger<CameraRepository>>();
            _repository = new CameraRepository(mockOptions.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_Returns_Valid_CamerasAsync() {
            File.WriteAllText(_testFilePath,
                 "name;lat;lon\n" +
                 "501 Neude;52.0907;5.1214\n" +
                 "Camera 502: Market;52.1;5.12\n");

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal(501, result[0].Number);
            Assert.Equal(502, result[1].Number);
        }

        [Fact]
        public async Task GetAll_Skips_Invalid_LinesAsync() {
            File.WriteAllText(_testFilePath,
    "name;lat;lon\n" +
    "Invalid\n" +
    "Camera 503: SomePlace;abc;5.1\n" +
    "Camera 504: AnotherPlace;52.1;xyz\n" +
    "Camera 505: OKPlace;52.1;5.1\n");

            var result = await _repository.GetAllAsync();

            Assert.Single(result);
            Assert.Equal(505, result[0].Number);
        }

        [Fact]
        public async Task GetAll_Logs_Error_When_File_MissingAsync() {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);

            var result = await _repository.GetAllAsync();

            Assert.Empty(result);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CSV file not found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExtractCameraNumber_Parses_CorrectlyAsync() {
            File.WriteAllText(_testFilePath,
    "name;lat;lon\n" +
    "Camera 777: Central;52.2;5.2\n");

            var result = await _repository.GetAllAsync();

            Assert.Single(result);
            Assert.Equal(777, result[0].Number);
        }

        public void Dispose() {
            if (Directory.Exists(_testDataPath))
                Directory.Delete(_testDataPath, true);
        }
    }
}
