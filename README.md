# xUnit Addons
# Run asynchronous code specific to test, before/after test

xUnit allows you to run code before each test using the [`BeforeAfterTestAttribute`](https://api.xunit.net/v3/2.0.1/Xunit.v3.BeforeAfterTestAttribute.html).

But, you cannot run `asynchronous code` using this attribute. This is needed in many situations.

To solve this problem, I have created a custom abstract xUnit attribute `BeforeAfterAsyncTestAttribute`,

inheriting from `BeforeAfterTestAttribute`, that allows you to run asynchronous code before each test or group of tests.

First, inherit from this attribute and create an attribute for each test.

You can re-use the attribute in multiples tests too. Just pass in a different Guid in the `stamp` parameter.

```csharp
public class MyBeforeAfterAsyncTestAttribute : BeforeAfterAsyncTestAttribute
{
    public MyBeforeAfterAsyncTestAttribute(Type specificAttributeType, string stamp, int noOfTests = 1) : base(specificAttributeType, stamp, noOfTests)
    {
    }

    public MyBeforeAfterAsyncTestAttribute(Type specificAttribute, Type returnFunctionClassType,
                                                string returnFunctionName, string stamp, int noOfTests = 1)
                                                : base(specificAttribute, returnFunctionClassType, returnFunctionName, stamp, noOfTests)
    {
    }
}
```

There are interfaces your specific Test has to implement.

If you want to run async code before the test, implement `IRunBeforeAsync`.

If you want to run async code after the test, implement `IRunAfterAsync`.

If you want the pre-test method `Run` to return a value implement `IRunBeforeAsyncWithReturn`.

```csharp
public interface IRunBeforeAsync : IRunAsync
{
    Action? RunBefore { get; }
}

public interface IRunAfterAsync : IRunAsync
{
    Action? RunAfter { get; }
}

public interface IRunBeforeAsyncWithReturn : IRunBeforeAsync
{
    object? ReturnValue { get; set; }
}
```

In the interface implementation, specific to each test, put your code specific to the Test in the `RunBefore` & `RunAfter` Actions, as shown below.

The code in the `RunBefore` Action will run asynchronously before the test or group of tests decorated with the custom attribute.

The code in the `RunAfter` Action will run asynchronously after the test or group of tests decorated with the custom attribute.

When you want to return a value from the pre-test method, assign the value to the `ReturnValue` property.

```csharp
public class LoadAIModel : IRunBeforeAsync, IRunAfterAsync
{
    public Action RunBefore => async () =>
    {
        // Arrange
        // Path to load model
        string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

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
        string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
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

        this.ReturnValue = sp;
    };

    public object? ReturnValue { get; set; }

    private async Task<IServiceProvider> BuildContainerAsync()
    {
        // Build DI container for AI Assistant Service
        var settings = new WebsiteAIAssistantSettings
        {
            AIModelLoadFilePath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip"),
            NegativeConfidenceThreshold = 0.70f,
            NegativeLabel = -1f
        };

        var services = new ServiceCollection();
        services.AddSingleton(settings);
        services.AddSingleton<IWebsiteAIAssistantService, WebsiteAIAssistantService>();
        var sp = services.BuildServiceProvider();

        return await Task.FromResult(sp);
    }
}
```

Then, you can decorate those specific tests with the inherited attributes.

Provide a Guid (as a string) as a parameter. This Guid must be unique to the test.

When you want to return a value from your pre-test method, create a static method and pass the `Type` of the class (containing the method) & the method name to the constructor.

```csharp
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

        private static void BuildLoadPredictDIContainerReturn(object o)
        {
            _aiAssistantServiceProvider = (IServiceProvider)o;
        }
    }
```

Run all the tests in the class.

Your specific code will run **ONLY ONCE** before & after each group of Theory Tests.

So, for example, your specific code in `LoadAIModel` will run asynchronously only once before the 3 Tests in the Theory group 

and once after all 3 tests have completed.