using DigitalProduction.CommandLine;

namespace UnitTests;

public class TestMiscellaneousManagers : TestingBase
{
	public TestMiscellaneousManagers()
	{
	}

	/// <summary>
	/// Tests:
	/// BoolFunction.UsePrefix
	/// </summary>
	[Fact]
	public void TestBoolFunctionUsePrefix()
	{
		BoolPrefixManager arguments = GetArgumentsInstance<BoolPrefixManager>("+v");
		Assert.True(arguments.Value);


//		arguments = GetArgumentsInstance<BoolPrefixManager>("-v");
		//Assert.False(arguments.Value);
	}


}