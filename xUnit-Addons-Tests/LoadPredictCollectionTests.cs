using WebsiteAIAssistant;
using xUnitAddons;

namespace xUnitAddonsTests
{
    public class LoadPredictCollectionFixture : BaseFixture
    {
        public override Action RunBefore => async () =>
        {
            // Arrange
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

            await PredictionEngine.LoadModelAsync(modelPath);
        };

        public override Action RunAfter => async () =>
        {
            // Clean up resources after the test, if necessary
            await PredictionEngine.UnloadModelAsync();
        };
    }

    [CollectionDefinition("Load Predict Collection")]
    public class LoadPredictCollection : ICollectionFixture<LoadPredictCollectionFixture>
    {
        // This class has no code, it is just the anchor for the attributes
    }

    [Collection("Load Predict Collection")]
    public class LoadPredictCollectionTests
    {
        private readonly LoadPredictCollectionFixture _fixture;

        // The fixture is injected via the constructor
        public LoadPredictCollectionTests(LoadPredictCollectionFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task Load_Predict(string userInput, Scheme expectedResult)
        {
            // Arrange
            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await PredictionEngine.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [Theory]
        [ClassData(typeof(LoadPredictTestClassData))]
        public async Task Load_Predict_ClassData(string userInput, Scheme expectedResult)
        {
            // Arrange
            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await PredictionEngine.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }
    }
}
