# CI/CD Workflow Architecture

## Overview Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│  Pull Request / Push to main                                    │
└─────────────────┬───────────────────────────────────────────────┘
                  │
                  ├──────────────────────┬─────────────────────────┐
                  ▼                      ▼                         ▼
    ┌──────────────────────┐ ┌────────────────────┐   ┌─────────────────┐
    │   Linux Checks       │ │  Windows Build     │   │  All Checks     │
    │   (Ubuntu)           │ │  (Windows)         │   │  (Status)       │
    │                      │ │                    │   │                 │
    │  • YAML validation   │ │  • Debug build     │   │  Waits for both │
    │  • File structure    │ │  • Release build   │   │  to complete    │
    │  • XML validation    │ │  • NuGet restore   │   │                 │
    │  • Basic syntax      │ │  • Output verify   │   │  Single ✅      │
    │  • Documentation     │ │  • Upload artifacts│   │                 │
    │                      │ │                    │   │                 │
    │  ⏱️ ~1-2 minutes     │ │  ⏱️ ~3-5 minutes   │   │                 │
    │  💰 Cheap (Linux)    │ │  💰 2x cost        │   │                 │
    └──────────┬───────────┘ └─────────┬──────────┘   └─────────────────┘
               │                       │
               └───────────┬───────────┘
                           ▼
                    ┌─────────────┐
                    │  PR Status  │
                    │     ✅      │
                    └─────────────┘
```

## Workflow Triggers

### Automatic Triggers
- **Pull Requests** to `main` branch
- **Pushes** to `main` branch

### Manual Triggers
- Via GitHub Actions UI → "Run workflow"

## Workflow Details

### 1. Linux Code Checks (`ci-linux.yml`)
**Runner**: `ubuntu-latest` (free tier, fast)

**Steps**:
1. Checkout code
2. Check trailing whitespace (warning only)
3. Validate YAML with yamllint
4. Check file structure
5. Basic C# syntax validation
6. Verify NuGet packages.config
7. Check documentation

**Success Criteria**: All checks pass (warnings allowed)
**Artifacts**: None
**Duration**: 1-2 minutes

### 2. Windows Build (`ci-windows.yml`)
**Runner**: `windows-latest` (2x cost)

**Build Matrix**:
- Debug configuration
- Release configuration

**Steps**:
1. Checkout code
2. Setup MSBuild + NuGet
3. Restore NuGet packages
4. Build solution
5. Verify output files
6. Upload artifacts (Release only)

**Success Criteria**: Both configurations build successfully
**Artifacts**: Release build (7 days)
**Duration**: 3-5 minutes per configuration

### 3. All Checks (`ci.yml`)
**Runner**: `ubuntu-latest`

**Purpose**: Combined status check
**Dependencies**: None (runs independently)
**Duration**: < 1 minute

## Cost Analysis

### GitHub Actions Minutes Usage

**Per PR**:
- Linux checks: ~2 minutes
- Windows Debug: ~4 minutes × 2 = 8 minutes (Windows = 2× multiplier)
- Windows Release: ~4 minutes × 2 = 8 minutes (Windows = 2× multiplier)

**Total per PR**: ~18 Linux-equivalent minutes

### Monthly Free Tier
- Free plan: 2,000 Linux minutes/month
- Allows ~111 PRs/month within free tier

### Cost Optimization Strategies
1. ✅ **Linux checks run first**: Cheap, fast feedback
2. ✅ **Parallel Windows builds**: Faster, but uses 2 slots
3. ✅ **Artifact retention**: Only 7 days
4. ✅ **Conditional uploads**: Only Release artifacts
5. ✅ **Non-blocking warnings**: Don't fail on cosmetic issues

## Failure Handling

### Linux Checks Fail
- **Impact**: PR blocked
- **Common causes**:
  - Invalid YAML syntax
  - Missing required files
  - XML parse errors
- **Resolution**: Fix locally, push changes

### Windows Build Fails
- **Impact**: PR blocked
- **Common causes**:
  - Compilation errors
  - Missing dependencies
  - NuGet restore failures
- **Resolution**: Build locally with MSBuild, fix errors

### Both Pass, PR Rejected
- **Cause**: Manual review required
- **Action**: Address review comments

## Accessing Build Artifacts

1. Go to Actions tab
2. Select workflow run
3. Scroll to Artifacts section
4. Download `windows-build-Release.zip`
5. Extract and test executable

## Adding New Checks

### Linux Workflow
Add steps to `.github/workflows/ci-linux.yml`:

```yaml
- name: New Check
  run: |
    echo "Running new check..."
    # Your check here
```

### Windows Workflow
Add steps to `.github/workflows/ci-windows.yml`:

```yaml
- name: New Build Step
  run: |
    # Your build step here
```

## Maintenance

### Update Runners
Edit workflow files to use newer runner versions:
```yaml
runs-on: ubuntu-24.04  # Update version
```

### Update Actions
Keep GitHub Actions up to date:
```yaml
- uses: actions/checkout@v4  # Check for v5
- uses: microsoft/setup-msbuild@v2
```

### Adjust Retention
Modify artifact retention period:
```yaml
retention-days: 14  # Increase if needed
```

## Related Documentation
- [CI_CD.md](CI_CD.md) - Detailed CI/CD documentation
- [README.md](README.md) - Project overview with CI badges
- [.github/workflows/](.github/workflows/) - Workflow source files
