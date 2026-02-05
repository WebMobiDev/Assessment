# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Assessment.sln ./
COPY src/Domain/Assessment.Domain.csproj src/Domain/
COPY src/Infrastructure/Assessment.Infrastructure.csproj src/Infrastructure/
COPY src/Api/Assessment.Api.csproj src/Api/

RUN dotnet restore src/Api/Assessment.Api.csproj

COPY . .
RUN dotnet publish src/Api/Assessment.Api.csproj -c Release -o /app/publish

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Assessment.Api.dll"]
