# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [v0.3.0] - 2020-02-24
### Fixed
- Fix windows never sets isRunning
- Windows build invokes into wrong WebTty.Exec binary on native builds

### Changed
- Calculate client assets server side
- Cache assets for on year when hash is present in query string
- Decouple schema generator from api library

### Added
- Generate native builds

## [v0.2.0] - 2020-02-22
### Added
- Support for multiple command line arguments
- Allow changing address
- Allow changing default port
- Allow binding to unix socket
- Allow passing a theme
- Allow changing socket pty endpoint

### Changed
- Overhauled command line system
- Improved server
- Improved client

## [v0.1.0] - 2019-10-21
Initial release
