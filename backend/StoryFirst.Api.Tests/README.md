# StoryFirst.Api.Tests

Comprehensive unit test suite for the StoryFirst.Api service layer using xUnit, Moq, and FluentAssertions.

## Overview

This test project provides complete coverage for all service layer components in the StoryFirst.Api application. Tests are organized by functional areas and use mocking to isolate service logic from repository dependencies.

## Test Framework & Libraries

- **xUnit 2.9.2**: Test framework
- **Moq 4.20.72**: Mocking framework for creating test doubles
- **FluentAssertions 7.0.0**: Fluent assertion library for more readable tests
- **Microsoft.NET.Test.Sdk 17.13.0**: Test SDK for running tests

## Test Structure

Tests are organized to mirror the service structure in StoryFirst.Api:

```
StoryFirst.Api.Tests/
├── Services/
│   ├── ProjectManagement/
│   │   └── ProjectServiceTests.cs
│   ├── UserStoryMapping/
│   │   ├── ThemeServiceTests.cs
│   │   ├── EpicServiceTests.cs
│   │   ├── StoryServiceTests.cs
│   │   ├── SpikeServiceTests.cs
│   │   └── TaskServiceTests.cs
│   ├── SprintPlanning/
│   │   ├── SprintServiceTests.cs
│   │   ├── BacklogServiceTests.cs
│   │   ├── TeamServiceTests.cs
│   │   └── ReleaseServiceTests.cs
│   └── Visualization/
│       └── GraphServiceTests.cs
└── StoryFirst.Api.Tests.csproj
```

## Test Coverage

### Priority 1: Core Services ✅
- **ProjectServiceTests** (26 tests): Full CRUD operations and member management
- **ThemeServiceTests** (13 tests): Theme CRUD with project scoping
- **EpicServiceTests** (15 tests): Epic management within themes
- **StoryServiceTests** (19 tests): Story CRUD and field validation

### Priority 2: Additional Services ✅
- **SpikeServiceTests** (9 tests): Spike management for research work items
- **TaskServiceTests** (9 tests): Task item CRUD operations
- **SprintServiceTests** (9 tests): Sprint lifecycle management
- **BacklogServiceTests** (4 tests): Backlog retrieval and filtering

### Priority 3: Remaining Services ✅
- **TeamServiceTests** (5 tests): Team management
- **ReleaseServiceTests** (5 tests): Release planning
- **GraphServiceTests** (2 tests): Graph data visualization

**Total: 116 tests, all passing**

## Test Patterns

All tests follow the AAA (Arrange-Act-Assert) pattern:

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var mockObject = new Mock<IDependency>();
    mockObject.Setup(x => x.Method()).ReturnsAsync(expectedValue);
    var service = new ServiceUnderTest(mockObject.Object);

    // Act
    var result = await service.MethodUnderTest();

    // Assert
    result.Should().NotBeNull();
    result.Should().Be(expectedValue);
    mockObject.Verify(x => x.Method(), Times.Once);
}
```

## Test Categories

Each service test class covers:

1. **Happy Path Tests**: Successful operations with valid data
2. **Validation Tests**: ArgumentException for invalid input
3. **Not Found Tests**: KeyNotFoundException for missing entities
4. **Business Rule Tests**: InvalidOperationException for business logic violations
5. **Repository Verification**: Ensure repository methods are called correctly
6. **Edge Cases**: Null values, empty collections, boundary conditions

## Running Tests

### Run all tests:
```bash
dotnet test
```

### Run specific test class:
```bash
dotnet test --filter "FullyQualifiedName~ProjectServiceTests"
```

### Run tests with detailed output:
```bash
dotnet test --verbosity detailed
```

### Generate code coverage (if configured):
```bash
dotnet test /p:CollectCoverage=true
```

## Example Test

```csharp
[Fact]
public async Task CreateAsync_ValidProject_ReturnsProject()
{
    // Arrange
    var project = new Project { Key = "TEST-1", Name = "Test Project" };
    _mockProjectRepo.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Project, bool>>>()))
        .ReturnsAsync(false);
    _mockProjectRepo.Setup(x => x.AddAsync(It.IsAny<Project>()))
        .Returns(Task.CompletedTask);
    _mockProjectRepo.Setup(x => x.SaveChangesAsync())
        .ReturnsAsync(1);

    // Act
    var result = await _service.CreateAsync(project);

    // Assert
    result.Should().NotBeNull();
    result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    _mockProjectRepo.Verify(x => x.AddAsync(project), Times.Once);
    _mockProjectRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
}
```

## Key Testing Principles

1. **Isolation**: Each test is independent and uses mocks for all dependencies
2. **Clarity**: Test names clearly describe what is being tested
3. **Coverage**: Tests cover success paths, error conditions, and edge cases
4. **Maintainability**: Tests follow consistent patterns and are easy to update
5. **Fast**: Tests run quickly by mocking database operations

## Continuous Integration

These tests are designed to run in CI/CD pipelines. They:
- Execute quickly (< 1 second total)
- Require no external dependencies (database, network)
- Provide clear pass/fail indicators
- Follow standard xUnit conventions for test discovery

## Adding New Tests

When adding new service tests:

1. Create a test class in the appropriate service area folder
2. Follow the existing naming convention: `{ServiceName}Tests`
3. Add constructor to set up mocks and service instance
4. Write tests for each public method
5. Use descriptive test method names: `MethodName_Scenario_ExpectedResult`
6. Follow AAA pattern (Arrange, Act, Assert)
7. Verify repository method calls using Moq's `Verify` method
8. Use FluentAssertions for readable assertions

## Dependencies

All test dependencies are managed via NuGet and specified in `StoryFirst.Api.Tests.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="FluentAssertions" Version="7.0.0" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.13.0" />
  <PackageReference Include="Moq" Version="4.20.72" />
  <PackageReference Include="xunit" Version="2.9.2" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
</ItemGroup>
```

## Contributing

When contributing new tests:
- Maintain existing patterns and conventions
- Ensure all tests pass before submitting
- Add tests for both success and failure scenarios
- Keep test data minimal and focused
- Document any complex test setups

## License

This test project is part of the StoryFirst application and follows the same license.
