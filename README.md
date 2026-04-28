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

How to see test coverage
```
dotnet tool install --global dotnet-reportgenerator-globaltool
rm -rf ./TestResults
dotnet test BatteryAdvisor.slnx --collect:"XPlat Code Coverage" --settings coverage.runsettings --results-directory ./TestResults
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./TestResults/report" -reporttypes:"Html;Badges" -filefilters:"-*.generated.cs"
xdg-open ./TestResults/report/index.html
```

### SQLite

Install dotnet-ef globally
```
dotnet tool install --global dotnet-ef
```

How to add a migration
```
dotnet ef migrations add <Migration name> --project ./BackEnd/BatteryAdvisor.Core --startup-project ./BackEnd/BatteryAdvisor.Host --context BatteryAdvisorContext
```


Update database
```
dotnet ef database update --project ./BackEnd/BatteryAdvisor.Core --startup-project ./BackEnd/BatteryAdvisor.Host --context BatteryAdvisorContext
```

#### Rollback

Rollback
```
dotnet ef database update <Migration name>> --project ./BackEnd/BatteryAdvisor.Core --startup-project ./BackEnd/BatteryAdvisor.Host --context BatteryAdvisorContext
```

Delete a migration
```
dotnet ef migrations remove --project ./BackEnd/BatteryAdvisor.Core --startup-project ./BackEnd/BatteryAdvisor.Host --context BatteryAdvisorContext
```



## Frontend

A Angular 21 application using TailwindCSS and PrimeNG

How to start:
```
corepack pnpm start
```