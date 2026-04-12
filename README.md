# Battery Advisor

## Build Status

[![Backend Testing](https://github.com/bram2202/BatteryAdvisor/actions/workflows/backend-testing.yaml/badge.svg)](https://github.com/bram2202/BatteryAdvisor/actions/workflows/backend-testing.yaml)


**work in progress**


BatteryAdvisor helps homeowners decide whether investing in a home battery is financially worthwhile. 

It connects to a local Home Assistant instance to retrieve real energy consumption and solar (PV) generation data, and uses that data to simulate different battery scenarios.

Configure battery capacity, purchase price, and your local energy tariff to see a clear comparison of potential savings, payback period, and self-sufficiency improvement, based on your actual energy usage.


# Development
## Backend

A C# .net10 backend with REST API.

How to start:
```
cd BatteryAdvisor.Host
dotnet run
```

How to build
```
dotnet build BatteryAdvisor.slnx
```

How to test
```
dotnet test BatteryAdvisor.slnx
```


## Frontend

A Angular 21 application using TailwindCSS and PrimeNG

How to start:
```
corepack pnpm start
```