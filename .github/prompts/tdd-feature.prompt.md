# TDD Feature Implementation

Implement the following feature using strict TDD:

**Feature:** $FEATURE

---

## Steps (do not skip or reorder)

### 1. Write failing unit tests (red)

- Create / update the test class in `tests/DunIt.UnitTests/`
- Follow `ShouldXXX_WhenYYY` method naming
- Use `[Test, AutoMoqData]` — no `[TestFixture]`
- Parameter order: setup values → `[Frozen]` deps → `sut`
- Name mocks by role: `Stub` / `Spy` / `Dummy`
- Use Shouldly assertions
- Confirm tests fail before continuing

### 2. Write failing E2E tests (red)

- Create / update Playwright tests in `tests/DunIt.IntegrationTests/`
- Cover the new UI behaviour end-to-end
- Confirm tests fail before continuing

### 3. Implement (green)

- Write the minimum code to make all tests pass
- C# rules:
  - `namespace` first, then `using` directives
  - No `Async` suffix on method names
  - Two-constructor pattern instead of nullable optionals
  - `Type.Empty` sentinel instead of `Type?`

### 4. Review and refactor

- Remove duplication
- Ensure naming is clear
- Re-run tests to confirm still green

### 5. Commit and push

```bash
git add <changed files>
git commit -m "<message>"
git push
```

Do not skip the push — every iteration must be pushed.
