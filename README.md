![Razor Logo](https://imgur.com/jTtHLVF.png)

# Razor: An Ultima Online Assistant

Razor is a free tool designed to help with simple tasks while playing Ultima Online.

## Outlands Community Edition

The goal of this project is to build a suitable replacement for the Steam assistant. The work is based on the excellent [updates to Razor made for UO:R](https://github.com/markdwags/razor), replacing "razor script" with steam-compatible scripting instead.

It's recommended that you use the [ClassicUO](https://github.com/andreakarasho/ClassicUO) client with this version of Razor, however it will work with the original 5.x and 7.x clients.

## Building

This repository contains submodules. First, clone it as follows:

```
git clone --recurse-submodules https://github.com/jaedan/razor
```

Then open the solution file in Visual Studio 2019 and build.

## Installation & Help

To use with ClassicUO, simply update ClassicUO's settings.json file to contain a plugin that points at the Razor.exe produced when building this project. Ensure that you match the target CPU architecture correctly, which likely means doing x64 builds for both ClassicUO and for Razor.

## Contributing & Code of Conduct

Please read [CONTRIBUTING](CONTRIBUTING.md) for more information on how to contribute to this project.

Please note we have a [Code of Conduct](CODE_OF_CONDUCT.md), please follow it in all your interactions with this project.

## License

This work is released under the GPLv3 license. This project does not distribute any copyrighted game assets. In order to run this application you'll need to legally obtain a copy of the Ultima Online Classic Client. See the [LICENSE](LICENSE.md) file for details.
