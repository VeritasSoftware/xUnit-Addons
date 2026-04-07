using Microsoft.Extensions.DependencyInjection;
using WebsiteAIAssistant;
using xUnitAddons;

namespace xUnitAddonsTests.Helpers
{
    public class LoadAIModel : IRunBeforeAsync, IRunAfterAsync
    {
        public Action RunBefore => async () =>
        {
            // Arrange
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model.zip");

            await PredictionEngine.LoadModelAsync(modelPath);
        };

        public Action RunAfter => async () =>
        {
            // Clean up resources after the test, if necessary
            await PredictionEngine.UnloadModelAsync();
        };
    }

    public class LoadAIListModel : IRunBeforeAsync, IRunAfterAsync
    {
        public Action RunBefore => async () =>
        {
            // Arrange
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-CreateModel-List.zip");

            await PredictionEngine.LoadModelAsync(modelPath);
        };

        public Action RunAfter => async () =>
        {
            // Clean up resources after the test, if necessary
            await PredictionEngine.UnloadModelAsync();
        };
    }

    public class SetAIModelPath : IRunBeforeAsync, IRunAfterAsync
    {
        public Action RunBefore => async () =>
        {
            // Arrange
            // Path to load model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-Autoload-Predict.zip");
            // Provide the path to the AI model
            PredictionEngine.AIModelLoadFilePath = modelPath;
        };

        public Action RunAfter => async () =>
        {
            // Clean up resources after the test, if necessary
            await PredictionEngine.UnloadModelAsync();
        };
    }

    public class BuildLoadPredictContainer : IRunBeforeAsyncWithReturn
    {
        public Action RunBefore => async () =>
        {
            var sp = await BuildContainerAsync();

            // Set the return value
            this.ReturnValue = sp;
        };

        public object? ReturnValue { get; set; }

        private async Task<IServiceProvider> BuildContainerAsync()
        {
            // Build DI container for AI Assistant Service
            var services = new ServiceCollection();
            services.AddWebsiteAIAssistantCore(settings =>
            {
                settings.AIModelLoadFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "SampleWebsite-AI-Model-Autoload-Service.zip");
                settings.NegativeConfidenceThreshold = 0.70f;
                settings.NegativeLabel = -1f;
            });
            var sp = services.BuildServiceProvider();

            return await Task.FromResult(sp);
        }
    }
}
