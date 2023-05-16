FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 7071

ENV ASPNETCORE_URLS=http://+:7071

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
COPY ["UnitTest/UnitTest.csproj", "UnitTest/"]
COPY ["EfcDataAccess/EfcDataAccess.csproj", "EfcDataAccess/"]

COPY . .
WORKDIR "/src/WebAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build-env /app/EfcDataAccess/Greenhouse.db .
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebAPI.dll"]
