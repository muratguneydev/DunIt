# New Unit Test

Scaffold a unit test method for the following scenario:

**Class under test:** $CLASS  
**Scenario:** $SCENARIO

---

## Template

```csharp
[Test, AutoMoqData]
public async Task Should_WhenYYY(
    // 1. setup values (plain AutoFixture values)
    // 2. [Frozen] Mock<T> dependencies — use Stub/Spy/Dummy suffix
    // 3. sut last
)
{
    // Arrange

    // Act

    // Assert
}
```

## Checklist

- [ ] Method name follows `ShouldXXX_WhenYYY`
- [ ] No `[TestFixture]` on the class
- [ ] `[Frozen]` deps use `using AutoFixture.NUnit4;` (not just `using AutoFixture;`)
- [ ] Parameters in order: data → frozen deps → sut
- [ ] Mock named by role: `Stub` / `Spy` / `Dummy`
- [ ] Assertions use Shouldly
- [ ] `// Arrange / Act / Assert` comments present
- [ ] Test fails before implementation
