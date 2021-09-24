FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["SSED-Cli/SSED-Cli.csproj", "SSED-Cli/"]
RUN dotnet restore "SSED-Cli/SSED-Cli.csproj"
COPY . .
WORKDIR "/src/SSED-Cli"
RUN dotnet build "SSED-Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SSED-Cli.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SSED-Cli.dll"]
