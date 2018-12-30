# Voltmeter

A graphical overview of system health status.

Build status: 
[![Build status](https://ci.appveyor.com/api/projects/status/997627b3796vd2hi?svg=true)](https://ci.appveyor.com/project/sandermvanvliet/voltmeter)

This application was built to quickly visualise the current state of services in any particular environment.
It leverages [simple service status endpoints](https://github.com/beamly/SE4/blob/master/SE4.md) as defined by Beamly to provide this information.

Depending on how you can discover environments and services, the overview will be updated automatically when services
appear or disappear in your running landscape.

Currently this has a hard coded set of providers that will just generate services and dependencies that can be
used as a demonstration version of Voltmeter. To actually to make this work you will need to impement the
ports you will need to make it work for your use case.

## Implementation

The Voltmeter UI needs an adapter that must be configured to provide evironments, services and dependencies that
will be used to visualise your landscape.

Start with by adding a new class library to contain the implementation you will created. You will create a
class to register you port implementations to wire up the ports. 

Create a class that you supply a `IServiceCollection` to register the ports. You will need to register the store
as a singleton to share state in the application.

Then the fun part starts, you will need to implement the following ports:

* `IEnvironmentProvider` to discover environments
* `IServiceDiscovery` to discover services in a particular environment
* `IServiceStatusProvider` to get status of the service
* `IServiceDependenciesProvider` to determine the dependencies of a service

## Models

| Model | Purpose |
|-------|---------|
| Environment | Contains a set of services particular for that environment |
| Service | A logical service, specific to a particular `environment` |
| ServiceStatus | The state of a `service` in an `environment` |
| Dependency | The name of a dependency of a `service` |
| DependencyStatus | The state of a `dependency` of a `service` |

