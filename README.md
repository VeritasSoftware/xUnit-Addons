# xUnit Addons

xUnit allows you to run code before each test using the `BeforeAfterTestAttribute`.

But, you cannot run `asynchronous code` using this attribute. This is needed in many situations.

To solve this problem, I have created a custom abstract xUnit attribute `BeforeAsyncAfterSyncTestAttribute`,

inheriting from `BeforeAfterTestAttribute`, that allows you to run asynchronous code before each test or group of tests.

First, inherit from this attribute and create a class for each test.

```csharp
public class LoadModelBeforeTestAttribute : BeforeAsyncAfterSyncTestAttribute
{
    public LoadModelBeforeTestAttribute(Type specificAttributeType, string stamp) : base(specificAttributeType, stamp)
    {
    }

    public override void After (MethodInfo methodUnderTest)
    {
        // This method runs synchronously after the test. You can use it to clean up resources after the test, if necessary.
    }
}

public class SetModelPathBeforeTestAttribute : BeforeAsyncAfterSyncTestAttribute
{
    public SetModelPathBeforeTestAttribute(Type specificAttributeType, string stamp) : base(specificAttributeType, stamp)
    {
    }

    public override void After(MethodInfo methodUnderTest)
    {
        // This method runs synchronously after the test. You can use it to clean up resources after the test, if necessary.
    }
}

```

There is an interface your specific Test has to implement.

```csharp
public interface IRunBeforeTest
{
    public Action Run { get; }
}
```

In the interface implementation, specific to each test, put your code specific to the Test in the Run Action, as shown below.

The code in the Run Action will run asynchronously before the test or group of tests decorated with the custom attribute.

```csharp
public class LoadAIModel : IRunBeforeTest
{
    public Action Run => async () =>
    {
        // Arrange
        // Path to load model
        string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

        await PredictionEngine.LoadModelAsync(modelPath);
    };
}

public class SetAIModelPath : IRunBeforeTest
{
    public Action Run => async () =>
    {
        // Arrange
        // Path to load model
        string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");
        // Provide the path to the AI model
        PredictionEngine.AIModelLoadFilePath = modelPath;
    };
}
```

Then, you can decorate those specific tests with the inherited attributes.

Provide a Guid (as a string) as a parameter. This Guid must be unique to the test.

```csharp
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
```

Run all the tests in the class.

Your specific code will run **ONLY ONCE** before each group of Theory Tests.

So, for example, your specific code in `LoadAIModel` will run asynchronously only once before the 3 Tests in the Theory group.