FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY BookCatalog.slnx ./
COPY src/BookCatalog.Domain/ ./src/BookCatalog.Domain/
COPY src/BookCatalog.Application/ ./src/BookCatalog.Application/
COPY src/BookCatalog.Infrastructure/ ./src/BookCatalog.Infrastructure/
COPY src/BookCatalog.API/ ./src/BookCatalog.API/
RUN dotnet restore ./src/BookCatalog.API/BookCatalog.API.csproj
RUN dotnet publish ./src/BookCatalog.API/BookCatalog.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "BookCatalog.API.dll"]
