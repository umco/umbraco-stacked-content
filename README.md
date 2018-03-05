# Stacked Content

[![Build status](https://img.shields.io/appveyor/ci/UMCO/umbraco-stacked-content.svg)](https://ci.appveyor.com/project/UMCO/umbraco-stacked-content)
[![NuGet release](https://img.shields.io/nuget/v/Our.Umbraco.StackedContent.svg)](https://www.nuget.org/packages/Our.Umbraco.StackedContent)
[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/backoffice-extensions/stacked-content)

An Umbraco property editor for creating stacked content blocks.

## Getting Started

### Installation

> *Note:* Stacked Content has been developed against **Umbraco v7.4.0** and will support that version and above.

Stacked Content can be installed from either Our Umbraco or NuGet package repositories, or build manually from the source-code:

#### Our Umbraco package repository

To install from Our Umbraco, please download the package from:

> <https://our.umbraco.org/projects/backoffice-extensions/stacked-content>

#### NuGet package repository

To [install from NuGet](https://www.nuget.org/packages/Our.Umbraco.StackedContent), you can run the following command from within Visual Studio:

	PM> Install-Package Our.Umbraco.StackedContent

We also have a [MyGet package repository](https://www.myget.org/gallery/umbraco-packages) - for bleeding-edge / development releases.

#### Manual build

If you prefer, you can compile Stacked Content yourself, you'll need:

* Visual Studio 2015 (or above)

To clone it locally click the "Clone in Windows" button above or run the following git commands.

	git clone https://github.com/umco/umbraco-stacked-content.git umbraco-stacked-content
	cd umbraco-stacked-content
	.\build.cmd

---

## Known Issues

Please be aware that not all property-editors will work within Stacked Content. The following Umbraco core property-editors are known to have compatibility issues:

* Upload

---

## Contributing to this project

Anyone and everyone is welcome to contribute. Please take a moment to review the [guidelines for contributing](CONTRIBUTING.md).

* [Bug reports](CONTRIBUTING.md#bugs)
* [Feature requests](CONTRIBUTING.md#features)
* [Pull requests](CONTRIBUTING.md#pull-requests)

---

## Contact

Have a question?

* [Stacked Content Forum](https://our.umbraco.org/projects/backoffice-extensions/stacked-content/stacked-content-feedback) on Our Umbraco
* [Raise an issue](https://github.com/umco/umbraco-stacked-content/issues) on GitHub

## Dev Team

* [Matt Brailsford](https://github.com/mattbrailsford)
* [Lee Kelleher](https://github.com/leekelleher)

## License

Copyright &copy; 2016 UMCO, Our Umbraco and [other contributors](https://github.com/umco/umbraco-stacked-content/graphs/contributors)

Licensed under the [MIT License](LICENSE.md)
