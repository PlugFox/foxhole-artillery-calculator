# CI/CD Documentation

## Overview

This repository uses GitHub Actions for continuous integration and deployment. The CI/CD system provides fast feedback on code quality and ensures the project builds correctly on Windows.

## Workflows

### 1. Linux Code Checks (`ci-linux.yml`)

**Purpose**: Fast, cheap validation on Linux runners for immediate feedback.

**Triggers**:
- Pull requests to `main`
- Pushes to `main` branch
- Manual dispatch

**Checks Performed**:
1. **Trailing Whitespace Check**: Ensures no trailing whitespace in code files
2. **YAML Validation**: Validates all workflow files and configuration examples
3. **File Structure**: Verifies all required project files are present
4. **C# Syntax**: Basic syntax validation (brace matching)
5. **NuGet Configuration**: Validates packages.config XML structure
6. **Documentation**: Checks README completeness and configuration docs

**Runtime**: ~1-2 minutes  
**Cost**: Minimal (uses free Linux minutes)

### 2. Windows Build Verification (`ci-windows.yml`)

**Purpose**: Ensures the project compiles successfully on Windows with all dependencies.

**Triggers**:
- Pull requests to `main`
- Pushes to `main` branch
- Manual dispatch

**Build Matrix**:
- Debug configuration
- Release configuration

**Steps**:
1. Checkout code
2. Setup MSBuild and NuGet
3. Restore NuGet packages
4. Build solution
5. Verify build output (exe, DLLs)
6. Upload Release build artifacts (retained for 7 days)

**Runtime**: ~3-5 minutes per configuration  
**Cost**: Uses Windows runner minutes (2x Linux cost)

### 3. Combined Status Check (`ci.yml`)

**Purpose**: Provides a single status check for PR requirements.

**Triggers**: Same as above workflows

## Artifacts

Build artifacts from the Release configuration are automatically uploaded and available for 7 days:
- Navigate to Actions → Select workflow run → Artifacts section
- Download `windows-build-Release.zip`

## Local Testing

### Test Linux Checks Locally

```bash
# Install dependencies
sudo apt-get install yamllint libxml2-utils

# Check trailing whitespace
git grep -I --line-number --extended-regexp '\s+$' -- '*.cs' '*.xaml' '*.yml' '*.yaml' '*.md'

# Validate YAML
yamllint .github/workflows/*.yml

# Validate XML
xmllint --noout foxhole-artillery-calculator/packages.config
```

### Test Windows Build Locally

```powershell
# Restore packages
nuget restore foxhole-artillery-calculator/foxhole-artillery-calculator.sln

# Build Debug
msbuild foxhole-artillery-calculator/foxhole-artillery-calculator.sln /p:Configuration=Debug /p:Platform="Any CPU"

# Build Release
msbuild foxhole-artillery-calculator/foxhole-artillery-calculator.sln /p:Configuration=Release /p:Platform="Any CPU"
```

## Workflow Status Badges

Add these badges to README.md to show CI status:

```markdown
![Linux Checks](https://github.com/PlugFox/foxhole-artillery-calculator/workflows/CI%20-%20Linux%20Code%20Checks/badge.svg)
![Windows Build](https://github.com/PlugFox/foxhole-artillery-calculator/workflows/CI%20-%20Windows%20Build/badge.svg)
```

## Cost Optimization

The CI/CD setup is optimized for cost:
- **Linux checks first**: Fast feedback with minimal cost
- **Windows builds in parallel**: Both Debug and Release build simultaneously
- **Artifact retention**: Only 7 days to minimize storage costs
- **Conditional artifact upload**: Only Release builds are saved

## Troubleshooting

### Workflow fails on "Restore NuGet packages"
- Check `packages.config` has correct package references
- Verify package versions are available on NuGet.org

### Build fails with missing assembly references
- Ensure `.csproj` file has correct `<Reference>` elements
- Check `HintPath` points to correct NuGet package location

### YAML validation fails
- Use yamllint locally to debug: `yamllint file.yml`
- Check indentation (use spaces, not tabs)
- Validate online: https://www.yamllint.com/

### Trailing whitespace check fails
- Run locally: `git grep -I '\s+$' -- '*.cs'`
- Remove trailing spaces: Many editors have "trim trailing whitespace" option

## Future Improvements

Potential enhancements for the CI/CD pipeline:
- [ ] Add code style/linting with StyleCop
- [ ] Add security scanning with CodeQL
- [ ] Add automated testing when tests are added
- [ ] Add code coverage reports
- [ ] Add dependency vulnerability scanning
- [ ] Cache NuGet packages for faster builds
- [ ] Add PR size checks
- [ ] Add commit message validation
