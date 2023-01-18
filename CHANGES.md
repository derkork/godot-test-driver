# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0pre1] - 2023-01-18
### Breaking Changes

- Version 2.0.0 has been updated for Godot 4. It is no longer compatible with Godot 3. 
- `TweenDriver` has been removed as `Tween` is no longer a node.
- The experimental screenshot extension has been removed.
- .net targets have been updated to match Godot 4. Currently only `net6.0` is supported.

### Added

- A new `WindowDriver` class has been added to use as base class for driving `Window` nodes.


## [1.0.0] - 2022-07-14
### Breaking Changes
- `GraphEditDriver` will now throw an exception when trying to get connections from it and the underlying `GraphEdit` does not exist in the tree. Previously it would return an empty list of connections. This is to make it consistent with the behaviour of the other drivers.

### Added
- `GraphEditDriver` now has two new methods `HasConnectionFrom` and `HasConnectionTo` which allow you to check whether a certain port is connected without having to know which other port it is connected to.

## [0.1.0] - 2022-07-08
### Added
- Support for multiple .net targets. Now targets `net472` and `netstandard2.1`. 
- `GraphNodeDriver` now has a function `GetPortType` to to inspect the node's port types.

## [0.0.30] - 2022-06-24
- Initial public release