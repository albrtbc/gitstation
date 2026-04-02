# Contributing to GitStation

Thanks for your interest in contributing! This document will guide you through the process.

## 🚀 Quick Start

```bash
# Fork and clone the repository
git clone https://github.com/<your-username>/gitstation.git
cd gitstation

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run --project src/GitStation.csproj
```

> See `global.json` for the required .NET SDK version.

## 📂 Project Structure

```
src/
  ├── Commands/        # Git command wrappers
  ├── Converters/      # XAML value converters
  ├── Models/          # Data models and services (IAIService, OpenAIService, CLIAIService, etc.)
  ├── Native/          # Platform-specific code (Windows, macOS, Linux)
  ├── Resources/
  │   ├── Locales/     # Localization files (en_US.axaml, etc.)
  │   ├── Fonts/       # Bundled fonts
  │   └── Images/      # Icons and images
  ├── ViewModels/      # MVVM ViewModels
  ├── Views/           # Avalonia XAML views and code-behind
  └── GitStation.csproj
```

## 📝 Commits

We use [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: add new feature
feat!: add feature with breaking changes
fix: fix bug in X
docs: update documentation
test: add tests for Y
chore: update dependencies
refactor: reorganize code for Z
style: formatting changes without logic change
revert: undo a previous commit
```

The `!` suffix denotes a **breaking change** (e.g. `feat!:`, `fix!:`, `refactor!:`).

Keep commit messages under 50 characters. Use the imperative mood (`add`, `fix`, not `added`, `fixed`).

## 🔄 Contribution Workflow

Every contribution must follow this flow:

1. **Open an Issue first** — describe the bug or feature. Wait for feedback before implementing.
2. **Fork the repository** — work on your own fork, never directly on `main`.
3. **Create a branch** from `main` with a descriptive name:
   - `fix/short-description` for bug fixes
   - `feat/short-description` for new features
   - `docs/short-description` for documentation
4. **Submit a Pull Request** linked to the issue (e.g. `Closes #42`).
5. **One PR per issue.** Keep changes focused — no unrelated tweaks.

## 🐛 Reporting Bugs

1. Check if the bug has already been reported.
2. Include:
   - OS and version
   - Git version
   - Steps to reproduce
   - Expected vs actual behavior
   - Screenshots or logs if applicable

## 💡 Proposing Features

1. Open an Issue describing the feature and the use case.
2. Wait for feedback before implementing.
3. Reference the Issue in your PR.

## 🎨 Code Style

- Follow the existing conventions in the codebase. When in doubt, match the surrounding code.
- Do not add comments that describe what the code does — use clear naming instead. Only add comments for non-obvious reasoning.
- Do not add new dependencies without discussing it in the issue first.
- Do not include style/formatting changes outside the scope of your PR.

## 🔧 Useful Commands

| Command | Description |
|---------|-------------|
| `dotnet restore` | Restore NuGet packages |
| `dotnet build` | Build the project |
| `dotnet run --project src/GitStation.csproj` | Run the app |
| `dotnet build -r win-x64 --no-self-contained` | Build for Windows |
| `dotnet build -r osx-arm64 --no-self-contained` | Build for macOS (Apple Silicon) |
| `dotnet build -r linux-x64 --no-self-contained` | Build for Linux |

## 📄 License

By contributing, you agree that your contributions will be licensed under the [MIT License](LICENSE).
