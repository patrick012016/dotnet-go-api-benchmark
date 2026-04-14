<div align="center">
  
# API performance comparison: .NET (JIT & AOT) vs. Go

</div>

This repository provides an empirical, data-driven analysis of modern backend frameworks. A comparative performance evaluation is conducted between .NET 10 - utilizing two compilation variants (JIT with Dynamic PGO and Native AOT) and the Go ecosystem. Focused on standard cloud and serverless environments, the testing methodology eliminates framework overhead to assess core runtime metrics. The analysis specifically measures basic I/O operations, thread pool management, cold start latency, and high-concurrency throughput.

<div align="center">

[![.NET JIT](https://img.shields.io/badge/.NET_10-JIT-512BD4?logo=dotnet&style=for-the-badge)](https://dotnet.microsoft.com/)
[![.NET AOT](https://img.shields.io/badge/.NET_10-Native_AOT-512BD4?logo=dotnet&style=for-the-badge)](https://dotnet.microsoft.com/)
[![Go](https://img.shields.io/badge/Go-1.26-00ADD8?logo=go&logoColor=white&style=for-the-badge)](https://go.dev/)
[![k6](https://img.shields.io/badge/k6-Load_Testing-7D64FF?logo=k6&logoColor=white&style=for-the-badge)](https://k6.io/)

</div>


## Table of contents

* [Architecture and methodology](#architecture-and-methodology)
* [Test scenarios](#test-scenarios)
* [Repository structure](#repository-structure)
* [Project status](#project-status)
* [License](#license)

## Architecture and methodology

To ensure measurement accuracy and evaluate framework performance rather than architectural overhead, the standard Layered/MVC (Model-View-Controller) architecture was intentionally omitted. The REPR (Request-Endpoint-Response) pattern, based on Vertical Slice Architecture, was applied.

Key architectural decisions:

  * **Minimal APIs:** Each endpoint is implemented as an optimized function, bypassing standard controllers.
  * **Overhead Reduction:** Virtual dispatch (interfaces) on the main request paths and heap allocations associated with global Dependency Injection containers have been eliminated. Dependencies are injected directly at the method level.
  * **Domain Isolation:** Each endpoint contains its own set of models, ensuring maximum cohesion and preventing structural conflicts.

## Test scenarios

The implemented test scenarios have been divided into several main categories:

### Scenario 1: Basic operations

  * **`01_BasicOperation` (Get User Profile):** Simulates asynchronous fetching of a single record. Verifies routing overhead and asynchronous thread pool management.
  * **`02_AggregationBff` (User Summary BFF):** Implements the Backend-For-Frontend pattern. The endpoint concurrently queries mock external services using `Task.WhenAll`, aggregates data, and applies conditional business logic.
  * **`03_PartialUpdate` (Update Order):** Validates complex JSON modification operations. Includes real-time input format validation and type casting.

### Scenario 2: Serverless

  * **`01_ServerlessColdStart` (Ping):** A lightweight endpoint. Used to measure, memory consumption footprint, and cold start initialization time.
  * **`02_ThreadStarvation` (Concurrency):** Simulates thread delays during external resource querying. Evaluates concurrency efficiency and potential thread pool starvation under high load conditions.

##  Repository structure

```text
/
├── load-tests/                  # k6 load testing scripts isolated from the source code
│
├── src/                         # Application source code
│   ├── Api.Aot/                 # .NET 10 Minimal API - native AOT profile
│   ├── Api.Jit/                 # .NET 10 Minimal API - JIT profile
│   ├── go-api/                  # Go API
│   └── docker-compose.yml       # Runtime infrastructure for isolated environments
│
├── test/                        # API tests for the .NET and Go projects
└── results/                     # Raw test logs and generated charts
```

## Project status

**Completed:**
- [x] CI/CD pipeline configuration
- [x] Basic unit tests project implementation
- [x] .NET 10 (JIT & Native AOT) project setup

**To Do:**
- [ ] Finish tests project implementation
- [ ] Go API implementation
- [ ] k6 load testing scripts
- [ ] Finalizing and refining test scenarios

## License

This project is licensed under the Apache 2.0 License - see the [LICENSE](LICENSE) file for details.
