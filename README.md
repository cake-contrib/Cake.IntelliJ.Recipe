# Cake.IntelliJ.Recipe

[![standard-readme compliant][]][standard-readme]
[![All Contributors][all-contributorsimage]](#contributors)
[![Appveyor build][appveyorimage]][appveyor]
[![NuGet package][nugetimage]][nuget]

Convention based Cake scripts for building IntelliJ plugins.

## Table of Contents

- [Install](#install)
- [Usage](#usage)
- [Settings](#settings)
  - [Publishing](#publishing)
- [Maintainer](#maintainer)
- [Contributing](#contributing)
  - [Contributors](#contributors)
- [License](#license)

## Install

```cs
#load nuget:?package=Cake.IntelliJ.Recipe
```

## Usage

```cs
#load nuget:?package=Cake.IntelliJ.Recipe

Environment.SetVariableNames();

BuildParameters.SetParameters(
  context: Context,
  buildSystem: BuildSystem,
  sourceDirectoryPath: "./src",
  title: "Best-Plugin-Ever",
  repositoryOwner: "nils-a");

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context);

Build.Run();

```

Please be aware, that `Cake.IntelliJ.Recipe` wraps a `gradle` build
and uses tasks from `org.jetbrains.intellij` gradle plugin.
It is advised to create the plugin from https://github.com/JetBrains/intellij-platform-plugin-template.

## Settings

### Publishing

To publish the plugin to the [JetBrains Marketplace](https://plugins.jetbrains.com/) a [token](https://plugins.jetbrains.com/author/me/tokens) is required.
The token must be supplied in an environment variable and then picked up in the `gradle` build.
Default for plugins created from https://github.com/JetBrains/intellij-platform-plugin-template is to use the `PUBLISH_TOKEN` variable name.

Also, as with the "normal" gradle-based publishing, the first publish of the plugin must be made manually.

### Marketplace-ID

When publishing is automated, Twitter and Gitter messages can be created. To have them link to the plugin-page in the marketplace,
a setting of `marketplaceId` is needed. The `marketplaceId` can be fetched from the URL in the marketplace, e.g. for `https://plugins.jetbrains.com/plugin/15698-test-rider`, the `marketplaceId` is `15698`.

All other settings for Twitter, Gitter and such follow [Cake.Recipe](https://cake-contrib.github.io/Cake.Recipe/docs/fundamentals/environment-variables#twitter).

### Grade-Verbosity

The verbosity of running gradle has it's own setting: `gradleVerbosity`. (Default is set to `GradleLogLevel.Default`)

Keep in mind, that while setting Cake verbosity to `diagnostic`, secrets will still be secret.
However, setting gradle verbosity to `GradleLogLevel.Debug` will print out all secrets in the logs.

## Maintainer

[Nils Andresen @nils-a][maintainer]

## Contributing

Cake.IntelliJ.Recipe follows the [Contributor Covenant][contrib-covenant] Code of Conduct.

We accept Pull Requests.
Please see [the contributing file][contributing] for how to contribute to Cake.IntelliJ.Recipe.

Small note: If editing the Readme, please conform to the [standard-readme][] specification.

This project follows the [all-contributors][] specification. Contributions of any kind welcome!

### Contributors

Thanks goes to these wonderful people ([emoji key][emoji-key]):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/nils-a"><img src="https://avatars3.githubusercontent.com/u/349188?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Nils Andresen</b></sub></a><br /><a href="https://github.com/nils-a/Cake.IntelliJ.Recipe/commits?author=nils-a" title="Code">💻</a></td>
  </tr>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

## License

[MIT License © Nils Andresen][license]

[all-contributors]: https://github.com/all-contributors/all-contributors
[all-contributorsimage]: https://img.shields.io/github/all-contributors/nils-a/Cake.IntelliJ.Recipe.svg?color=orange&style=flat-square
[appveyor]: https://ci.appveyor.com/project/nils-a/cake-intellij-recipe
[appveyorimage]: https://img.shields.io/appveyor/ci/nils-a/cake-intellij-recipe.svg?logo=appveyor&style=flat-square
[contrib-covenant]: https://www.contributor-covenant.org/version/1/4/code-of-conduct
[contributing]: CONTRIBUTING.md
[emoji-key]: https://allcontributors.org/docs/en/emoji-key
[maintainer]: https://github.com/nils-a
[nuget]: https://nuget.org/packages/Cake.IntelliJ.Recipe
[nugetimage]: https://img.shields.io/nuget/v/Cake.IntelliJ.Recipe.svg?logo=nuget&style=flat-square
[license]: LICENSE.txt
[standard-readme]: https://github.com/RichardLitt/standard-readme
[standard-readme compliant]: https://img.shields.io/badge/readme%20style-standard-brightgreen.svg?style=flat-square
