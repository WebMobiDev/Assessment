# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Assessment.sln ./
COPY src/Web/Assessment.Web.csproj src/Web/
RUN dotnet restore src/Web/Assessment.Web.csproj

COPY . .
RUN dotnet publish src/Web/Assessment.Web.csproj -c Release -o /app/publish

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Assessment.Web.dll"]
