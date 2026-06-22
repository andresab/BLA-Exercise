FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TaskManagement.slnx ./
COPY src/TaskManagement.Domain/ ./src/TaskManagement.Domain/
COPY src/TaskManagement.Application/ ./src/TaskManagement.Application/
COPY src/TaskManagement.Infrastructure/ ./src/TaskManagement.Infrastructure/
COPY src/TaskManagement.API/ ./src/TaskManagement.API/

RUN dotnet restore ./src/TaskManagement.API/TaskManagement.API.csproj
RUN dotnet publish ./src/TaskManagement.API/TaskManagement.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskManagement.API.dll"]
