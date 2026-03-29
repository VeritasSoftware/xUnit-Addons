using Microsoft.Extensions.DependencyInjection;
using WebsiteAIAssistant;
using xUnitAddons;
using xUnitAddonsTests.Helpers;

namespace xUnitAddonsTests
{
    public class WebsiteAIAssistantTests
    {
        private static IServiceProvider? _aiAssistantServiceProvider;

        [MyBeforeAfterAsyncTest(typeof(LoadAIModel), "5bb02c70-01d1-4987-8a6e-ab7fc8b1dcc4", 3)]
        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task Load_Predict(string userInput, Scheme expectedResult)
        {
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

            await PredictionEngine.LoadModelAsync(modelPath);

            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await PredictionEngine.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [MyBeforeAfterAsyncTest(typeof(LoadAIModel), "67721fe6-cb27-4a6e-9f67-324291367706")]
        [Fact]
        public async Task Load_Predict_Fail()
        {
            var userInput = "What is the colour of a rose?";
            var expectedResult = Scheme.None;

            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

            await PredictionEngine.LoadModelAsync(modelPath);

            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await PredictionEngine.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [MyBeforeAfterAsyncTest(typeof(SetAIModelPath), "d54e2920-ad42-4acc-a6e2-37aad8e9ac3f", 3)]
        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task AutoLoad_Predict(string userInput, Scheme expectedResult)
        {
            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await PredictionEngine.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [MyBeforeAfterAsyncTest(typeof(LoadAIListModel), "1761b894-e972-4c2f-ab01-1c07b4867bd1", 3)]
        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task Load_Predict_List(string userInput, Scheme expectedResult)
        {
            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await PredictionEngine.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [MyBeforeAfterAsyncTest(typeof(BuildLoadPredictContainer), typeof(WebsiteAIAssistantTests),
                                    $"{nameof(BuildLoadPredictDIContainerReturn)}", "5bb02c70-01d1-4987-8a6e-ab7fc8b1dcc4", 3)]
        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task Load_Predict_Service(string userInput, Scheme expectedResult)
        {
            // Arrange                      
            var aiAssistantService = _aiAssistantServiceProvider!.GetRequiredService<IWebsiteAIAssistantService>();

            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await aiAssistantService.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [MyBeforeAfterAsyncTest(typeof(BuildLoadPredictContainer), typeof(WebsiteAIAssistantTests),
                                    $"{nameof(BuildLoadPredictDIContainerReturn)}", "ec94f239-86b9-4563-8b1d-2e85c65fb9d2", 3)]
        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task AutoLoad_Predict_Service(string userInput, Scheme expectedResult)
        {
            // Arrange                      
            var aiAssistantService = _aiAssistantServiceProvider!.GetRequiredService<IWebsiteAIAssistantService>();

            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await aiAssistantService.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        private static void BuildLoadPredictDIContainerReturn(object o)
        {
            _aiAssistantServiceProvider = (IServiceProvider)o;
        }
    }
}
