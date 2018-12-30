# Voltmeter

A graphical overview of system health status.

Build status: 
[![Build status](https://ci.appveyor.com/api/projects/status/997627b3796vd2hi?svg=true)](https://ci.appveyor.com/project/sandermvanvliet/voltmeter)

This application was built to quickly visualise the current state of services in any particular environment.
It leverages [simple service status endpoints](https://github.com/beamly/SE4/blob/master/SE4.md) as defined by Beamly to provide this information.

Depending on how you can discover environments and services, the overview will be updated automatically when services
appear or disappear in your running landscape.

## Models

| Model | Purpose |
|-------|---------|
| Environment | Contains a set of services particular for that environment |
| Service | A logical service, specific to a particular `environment` |
| ServiceStatus | The state of a `service` in an `environment` |
| ServiceDependency | A pointer to another `service` that this `service` depends on, contains information if this dependency is healthy or not |