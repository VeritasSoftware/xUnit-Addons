# xUnit Addons

xUnit allows you to run code before all tests.

Just put the code in the constructor of your test class.

When you run all the tests, the same code will run before each test.

So, you cannot run code specific to just one test before the test.

To solve this problem, I have created custom xUnit attributes.

There is an interface your specific Test has to implement.

```csharp
public interface IRunBeforeTest
{
    public Action Run { get; }
}
```

In the interface implementation, specific to each test, put your code specific to the Test in the Run Action, as shown below.

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

Then, you can decorate those specific tests with the `MyFact` & `MyTheory` attributes.

Provide a Guid (as a string) as a parameter. This Guid must be unique to the test.

```csharp
[MyTheory(typeof(LoadAIModel), "5bb02c70-01d1-4987-8a6e-ab7fc8b1dcc4")]
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


[MyTheory(typeof(SetAIModelPath), "d54e2920-ad42-4acc-a6e2-37aad8e9ac3f")]
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

So, for example, your specific code in `LoadAIModel` will run only once before the 3 Tests in the Theory group.