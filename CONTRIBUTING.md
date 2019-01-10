# Introduction

First off, thank you for considering contributing to *Beat the Rhythm - Core*. 
It's people like you that make this a fun community to be with and that create
the future of Virtual Reality rhythm gaming.

Following these guidelines helps to communicate that you respect the time of 
the developers managing and developing this open source project. In return, 
they should reciprocate that respect in addressing your issue, assessing changes, 
and helping you finalize your pull requests.

## What contributions are we looking for?

*Beat the Rhythm - Core* is an open source project and we love to receive 
contributions from our community — you! There are many ways to contribute, from 
writing tutorials or blog posts, creating example projects, improving the documentation, 
submitting bug reports and feature requests or writing code which can be incorporated 
into *Beat the Rhythm VR* itself or any other rhythm game that uses this framework.

## What contributions are we NOT looking for?

Please, don't use the issue tracker for [support questions]. For that, we have various channels
including [Discord](https://discord.gg/8w2z4Wn), 
[Steam Forums](https://steamcommunity.com/app/781200/discussions/), Twitter. Check out 
[social page](https://beat-the-rhythm-vr.com/Home/Social) for an up-to-date list.

# Ground Rules

We welcome all contributions, but keep in mind that we are using this as the core of our own games. 
Before working on anything that you would like to contribute, please open an issue for discussion 
and we'll enter the conversation. If you want to make changes to the design, we recommend you open 
an issue with your intentions before spending too much time, to ensure no effort is wasted.

Contributions can be made via pull requests to this repository. If you need an inspiration for what 
can help with, check out the [list of open issues](https://github.com/narayana-games/Beat-the-Rhythm.Core/issues).

Responsibilities
* Be welcoming to newcomers and encourage diverse new contributors from all backgrounds. See our [Code of Conduct](https://github.com/narayana-games/Beat-the-Rhythm.Core/blob/master/CODE_OF_CONDUCT.md).
* Ensure cross-platform compatibility for every change that's accepted. Windows, Mac, Debian & Ubuntu Linux, PlayStation.

# Your First Contribution

Working on your first Pull Request? You can learn how from this *free* series, 
[How to Contribute to an Open Source Project on GitHub](https://egghead.io/series/how-to-contribute-to-an-open-source-project-on-github).

At this point, you're ready to make your changes! Feel free to ask for help; everyone 
is a beginner at first :smile_cat:

If a maintainer asks you to "rebase" your PR, they're saying that a lot of code has changed, 
and that you need to update your branch so it's easier to merge.

# Getting started

For something that is bigger than a one or two line fix:

1. Create your own fork of the code
2. Do the changes in your fork
3. If you like the change and think the project could use it:
    * Be sure you have followed the code style for the project.
    * Make sure to understand and agree with the [LICENSE](https://github.com/narayana-games/Beat-the-Rhythm.Core/blob/master/LICENSE)
    * Make sure to understand and agree with the [CODE_OF_CONDUCT](https://github.com/narayana-games/Beat-the-Rhythm.Core/blob/master/CODE_OF_CONDUCT.md)
    * Send a pull request indicating that you have a agreed to the [LICENSE](https://github.com/narayana-games/Beat-the-Rhythm.Core/blob/master/LICENSE).

Small contributions such as fixing spelling errors, where the content is small enough 
to not be considered intellectual property, can be submitted by a contributor as a patch.

As a rule of thumb, changes are obvious fixes if they do not introduce any new functionality 
or creative thinking. As long as the change does not affect functionality, some likely examples include the following:
* Spelling / grammar fixes
* Typo correction, white space and formatting changes
* Comment clean up
* Bug fixes that change default return values or error codes stored in constants
* Adding logging messages or debugging output
* Changes to ‘metadata’ files like Gemfile, .gitignore, build scripts, etc.
* Moving source files from one directory or package to another

# How to report a bug

Before you submit an issue, search the issues archive. Maybe the issue has already 
been submitted or considered. If the issue appears to be a bug, and hasn't been 
reported, open a new issue.

If you find a security vulnerability, like a bug that could be used to cheat, 
do NOT open an issue. Email info@narayana-games.net instead.

In order to determine whether you are dealing with a security issue, ask yourself these two questions:
* Can I access something that's not mine, or something I shouldn't have access to?
* Can I disable something for other people?
If the answer to either of those two questions are "yes", then you're probably dealing with a security issue.
Note that even if you answer "no" to both questions, you may still be dealing with a security issue, 
so if you're unsure, just email us at info@narayana-games.net.

When filing an issue, please make sure to properly use the template. 
Delete anything that is not relevant.

# How to suggest a feature or enhancement

You feel *Beat the Rhythm - Core* is missing something that would be useful for any 
VR rhythm game? File a feature request and use the template. Like with bug reports,
please make sure to clean up your feature request if you don't need any parts of
the template.

# Code review process

The core team looks at Pull Requests on a regular basis and reviews them based on the
following criteria:
* Is it likely to break anything in our existing products?
* Does it add value to framework, for us and/or anyone else using the framework?
* Is the coding style consistent with the existing code-base?

We may give feedback and if so we expect responses within two weeks. 
After two weeks we may close the pull request if it isn't showing any activity.

# Code conventions

We use 
[C# Coding Conventions (C# Programming Guide)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
with a few noteworthy exceptions:
* No new lines for braces, see [1TBS (OTBS)](https://en.wikipedia.org/wiki/Indentation_style#Variant:_1TBS_(OTBS)). Exception: Blocks for the sake of organizing code start with just the opening brace, in its own line.
* Do not use implicit typing ("var") unless you absolutely have to. There is a whole class of really nasty bugs due to not fully understanding the types you are using. Avoid this by avoiding *var*.

# Credits

This Contributing Guide was created using 
[Contributing Guides: A Template](https://github.com/nayafia/contributing-template) 
by [@nayafia](https://twitter.com/nayafia). We used many of the examples listed there
to avoid reinventing the wheel. The first two paragraphs under *Ground Rules* are 
based on the [Contributing section of osu!lazer](https://github.com/ppy/osu/blob/master/README.md#contributing).

