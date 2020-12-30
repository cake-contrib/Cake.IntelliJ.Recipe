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
  - [Marketplace-ID](#marketplace-id)
  - [Grade-Verbosity](#grade-verbosity)
  - [Channels](#channels)
- [Changes to the template](#changes-to-the-template)
  - [Readme](#user-content-readme)
  - [Changelog](#user-content-changelog)
  - [Releases and PreReleases](#releases-and-prereleases)
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

### Channels

Settings with regard to publishing channels are:
- `pluginReleaseChannel` with a default of `"Stable"`
- `pluginPreReleaseChannel` with a default of `"Beta"`
- `pluginCiBuildChannel` with a default of `"Alpha"`
- `shouldPublishPluginCiBuilds` with a default of `false`
- `pluginChannelGradleProperty` with a default of `"marketplaceChannel"`

See [Releases and PreReleases](#releases-and-prereleases) for their meaning.

### Marketplace-ID

When publishing is automated, Twitter and Gitter messages can be created. To have them link to the plugin-page in the marketplace,
a setting of `marketplaceId` is needed. The `marketplaceId` can be fetched from the URL in the marketplace, e.g. for `https://plugins.jetbrains.com/plugin/15698-test-rider`, the `marketplaceId` is `15698`.

All other settings for Twitter, Gitter and such follow [Cake.Recipe](https://cake-contrib.github.io/Cake.Recipe/docs/fundamentals/environment-variables#twitter).

### Grade-Verbosity

The verbosity of running gradle has it's own setting: `gradleVerbosity`. (Default is set to `GradleLogLevel.Default`)

Keep in mind, that while setting Cake verbosity to `diagnostic`, secrets will still be secret.
However, setting gradle verbosity to `GradleLogLevel.Debug` will print out all secrets in the logs.

## Changes to the template

When creating a new recipe for a plugin that was created from https://github.com/JetBrains/intellij-platform-plugin-template 
the following changes have to be made:

### Readme

The standard template makes use of the `org.jetbrains.intellij` gradle-plugin to update the `<description>` of the `plugin.xml`
automatically from the content of the `Readme.md` which is perfectly fine.

If the `Readme.md` were to be moved (say one folder up - to fit in with the recipe structure) that would needed fixing.

The default code in `build.gradle.kts` is:
```java
// Extract the <!-- Plugin description --> section from README.md and provide for the plugin's manifest
pluginDescription(
    closure {
        File("./README.md").readText().lines().run {
            val start = "<!-- Plugin description -->"
            val end = "<!-- Plugin description end -->"

            if (!containsAll(listOf(start, end))) {
                throw GradleException("Plugin description section not found in README.md:\n$start ... $end")
            }
            subList(indexOf(start) + 1, indexOf(end))
        }.joinToString("\n").run { markdownToHTML(this) }
    }
)
```

This should point to the new location of the `Readme.md`, e.g.:
```java
// ...
  File("../README.md").readText().lines().run {
// ...
```

*Alternatively, the whole code-block could be removed from `build.gradle.kts` and a `<description>` manually added to the `plugin.xml`. 
Be aware, that the description is `html` with all entities encoded. (Something like `<description>&lt;h3&gt;This is the plugin!&lt;/h3&gt;</description>`).*

### Changelog

The standard template makes use of the `org.jetbrains.changelog` gradle-plugin to keep the `Changelog.md` updated with current version numbers and to copy the latest changes into the `<change-notes>` section of the `plugin.xml` on build.

Currently `Cake.IntellJ.Recipe` does not bridge the gap between release notes in GitHub releases (as preferred and automatically created by `Cake.Recipe`) and having the latest changes shown in the plugin (See [Issue 12](https://github.com/nils-a/Cake.IntelliJ.Recipe/issues/12)).

The suggestion is to place a link to the GitHub releases page inside the `change-notes` of `plugin.xml`.

For that, here are two parts inside `build.gradle.kts` which should be removed:

in `patchPluginXml`:
```java
// Get the latest available change notes from the changelog file
changeNotes(
    closure {
        changelog.getLatest().toHTML()
    }
)
```

and in `publishPlugin`:
```java
dependsOn("patchChangelog")
```

And the `change-notes` in `plugin.xml` have to be added manually. Something like 

```xml
<change-notes>
  &lt;a href="https://github.com/nils-a/test-rider/releases"&gt;&lt;h3&gt;See GitHub Releases&lt;/h3&gt;&lt;/a&gt;
</change-notes>
``` 

is suggested.

### Releases and PreReleases

Original `Cake.Recipe` knows three types of releases:
- Releases (created by adding a tag to the main branch) will publish a release-package to the release package source (typically NuGet).
- Tagged PreReleases (created by adding a tag on a different (i.e. not the main) branch) will publish a preRelease-package to the release package source (typically NuGet).
- PreReleases generated by CI-builds will publish a preRelease-package to the other package sources (e.g. MyGet, Azure, GPR).

JetBrains Marketplace does not have the notion of preReleases per se. Also there are (short of creating a [custom plugin repository](https://jetbrains.org/intellij/sdk/docs/basics/getting_started/update_plugins_format.html)) no alternatives to the JetBrains Marketplace.

The Marketplace however does have the notions of ["channels"](https://plugins.jetbrains.com/docs/marketplace/custom-release-channels.html). Channels are treated as separate repositories for all intents and purposes and only the default channel (named `Stable`) is browsable and searchable.

`Cake.IntelliJ.Recipe` has the following settings prepared for the above mentioned release-types:
- `pluginReleaseChannel` (default: `"Stable"`) as the name of the channel to publish releases to.
- `pluginPreReleaseChannel` (default `"Beta"`) as the name of the channel to publish tagged preReleases to.
- `pluginCiBuildChannel` (default `"Alpha"`) as the name of the channel to publish CI-builds to.

Be aware, that publishes to JetBrains marketplace are moderated so they will not be available to the public instantaneously. 
Also, for the very same reason publishing CI builds is deactivated in the defaults. Use `shouldPublishPluginCiBuilds` (default `false`) to enable it.

To make this work, `Cake.IntelliJ.Recipe` will pass the selected channel to gradle via a project property. 
The name of that property is set in `pluginChannelGradleProperty` (default: `"marketplaceChannel"`) and it has to be picked up by the gradle task `publishPlugin` in `build.gradle.kts`.

The code 
```java
publishPlugin {
    token(System.getenv("PUBLISH_TOKEN"))
    // pluginVersion is based on the SemVer (https://semver.org) and supports pre-release labels, like 2.1.7-alpha.3
    // Specify pre-release label to publish the plugin in a custom Release Channel automatically. Read more:
    // https://jetbrains.org/intellij/sdk/docs/tutorials/build_system/deployment.html#specifying-a-release-channel
    channels(pluginVersion.split('-').getOrElse(1) { "default" }.split('.').first())
}
```

should be replaced with:
```java
publishPlugin {
    token(System.getenv("PUBLISH_TOKEN"))
    channels(marketplaceChannel)
}
```

additonally the line 
```
val marketplaceChannel: String by project
```
has to be added near the text line "`// Import variables from gradle.properties file`".

and also, inside the `gradle.properties` a default has to be supplied:
```ini
marketplaceChannel = development
```


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
