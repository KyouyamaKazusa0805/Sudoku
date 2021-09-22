# How to install `Microsoft.WindowsAppSDK`

## About doc

Here I found an article that introduces how to install it.

[Click this link](https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/update-existing-projects-to-the-latest-release)

The doc says, you can just use the package console, and input the following code to upgrade the version:

```bash
uninstall-package Microsoft.ProjectReunion -ProjectName "Sudoku.UI"
uninstall-package Microsoft.ProjectReunion.Foundation -ProjectName "Sudoku.UI"
uninstall-package Microsoft.ProjectReunion.WinUI -ProjectName "Sudoku.UI"
install-package Microsoft.WindowsAppSDK -Version 1.0.0-preview1 -ProjectName "Sudoku.UI"
```

To uninstall the old-style `Micorost.ProjectReunion` (i.e. Windows UI) SDK, and then upgrade the version to 1.0.0 preview 1.

Please note that the newer-style SDK is named `Microsoft.WindowsAppSDK` instead of `Microsoft.ProjectReunion`.

## Additional

If you just want to install the SDK, just input the last line command:

```bash
install-package Microsoft.WindowsAppSDK -Version 1.0.0-preview1 -ProjectName "Sudoku.UI"
```

to fetch the latest version of `Microsoft.WindowsAppSDK`.

Enjoy! :D