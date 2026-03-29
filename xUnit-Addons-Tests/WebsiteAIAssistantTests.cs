using Microsoft.Extensions.DependencyInjection;
using WebsiteAIAssistant;
using xUnitAddonsTests.Helpers;

namespace xUnitAddonsTests
{
    public class WebsiteAIAssistantTests
    {
        private static IServiceProvider? _aiAssistantServiceProvider;
        private static IServiceProvider? _createModelServiceProvider;

        [LoadModelBeforeTest(typeof(LoadAIModel), "5bb02c70-01d1-4987-8a6e-ab7fc8b1dcc4")]
        [Theory]
        [InlineData("What are the requisites for carbon credits?", Scheme.ACCU)]
        [InlineData("How do I calculate net emissions?", Scheme.SafeguardMechanism)]
        [InlineData("What is the colour of a rose?", Scheme.None)]
        public async Task Load_Predict(string userInput, Scheme expectedResult)
        {
            var input = new ModelInput { Feature = userInput };

            // Act
            var prediction = await PredictionEngine.PredictAsync(input);

            // Assert
            Assert.NotNull(prediction);
            Assert.Equal(expectedResult, (Scheme)prediction.PredictedLabel);
        }

        [SetModelPathBeforeTest(typeof(SetAIModelPath), "d54e2920-ad42-4acc-a6e2-37aad8e9ac3f")]
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

        [LoadModelBeforeTest(typeof(LoadAIListModel), "1761b894-e972-4c2f-ab01-1c07b4867bd1")]
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

        [BuildLoadPredictDIContainer(typeof(BuildLoadPredictContainer), typeof(WebsiteAIAssistantTests),
                                    $"{nameof(BuildLoadPredictDIContainerReturn)}", "5bb02c70-01d1-4987-8a6e-ab7fc8b1dcc4")]
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

        [BuildLoadPredictDIContainer(typeof(BuildLoadPredictContainer), typeof(WebsiteAIAssistantTests),
                                    $"{nameof(BuildLoadPredictDIContainerReturn)}", "ec94f239-86b9-4563-8b1d-2e85c65fb9d2")]
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

        //[Fact]
        //public async Task UnloadModel()
        //{
        //    // Arrange
        //    // Path to load model
        //    string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

        //    await PredictionEngine.LoadModelAsync(modelPath);

        //    var unloaded = await PredictionEngine.UnloadModelAsync();

        //    // Assert
        //    Assert.True(unloaded);
        //}

        //[BuildLoadPredictDIContainer(typeof(BuildLoadPredictContainer), typeof(WebsiteAIAssistantTests),
        //                            $"{nameof(BuildLoadPredictDIContainerReturn)}", "ea1d6f3b-6fc2-462e-af85-eb90014414e8")]
        //[Fact]
        //public async Task UnloadModel_Service()
        //{
        //    var service = _aiAssistantServiceProvider!.GetRequiredService<IWebsiteAIAssistantService>();

        //    var unloaded = await service.UnloadModelAsync();
        //    // Assert
        //    Assert.True(unloaded);
        //}

        private static void BuildLoadPredictDIContainerReturn(object o)
        {
            _aiAssistantServiceProvider = (IServiceProvider)o;
        }

        private static void BuildCreateModelDIContainerReturn(object o)
        {
            _createModelServiceProvider = (IServiceProvider)o;
        }

        private static IEnumerable<ModelInput> LoadListFromFile(string filePath)
        {
            var data = new List<ModelInput>();
            using var reader = new StreamReader(filePath);
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                var parts = line.Split('\t');
                if (parts.Length == 2 && float.TryParse(parts[0], out float label))
                {
                    data.Add(new ModelInput
                    {
                        Label = label,
                        Feature = parts[1]
                    });
                }
            }
            return data;
        }
    }
}
