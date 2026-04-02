using Microsoft.Extensions.DependencyInjection;
using WebsiteAIAssistant;
using xUnitAddons;

namespace xUnitAddonsTests
{
    public class LoadPredictServiceCollectionFixture : BaseFixture
    {
        public IServiceProvider? ServiceProvider { get; set; }

        public override Action RunBefore => () =>
        {
            BuildContainer();
        };

        public override Action RunAfter => async () =>
        {
            // Clean up resources after the test, if necessary
            var service = ServiceProvider!.GetRequiredService<IWebsiteAIAssistantService>();

            await service.UnloadModelAsync();
        };

        private void BuildContainer()
        {
            // Build DI container for AI Assistant Service
            var settings = new WebsiteAIAssistantSettings
            {
                AIModelLoadFilePath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model-LoadPredictServiceCollectionTests.zip"),
                NegativeConfidenceThreshold = 0.70f,
                NegativeLabel = -1f
            };

            var services = new ServiceCollection();
            services.AddSingleton(settings);
            services.AddSingleton<IWebsiteAIAssistantService, WebsiteAIAssistantService>();
            var sp = services.BuildServiceProvider();

            this.ServiceProvider = sp;
        }
    }

    [CollectionDefinition("Load Predict Service Collection")]
    public class LoadPredictServiceCollection : ICollectionFixture<LoadPredictServiceCollectionFixture>
    {
        // This class has no code, it is just the anchor for the attributes
    }

    [Collection("Load Predict Service Collection")]
    public class LoadPredictServiceCollectionTests
    {
        private readonly IServiceProvider? _aiAssistantServiceProvider;

        public LoadPredictServiceCollectionTests(LoadPredictServiceCollectionFixture fixture)            
        {
            _aiAssistantServiceProvider = fixture.ServiceProvider;
        }

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
    }
}
