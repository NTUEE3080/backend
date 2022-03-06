﻿FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PitaPairing.csproj", "./"]
RUN dotnet restore "PitaPairing.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "PitaPairing.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PitaPairing.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PitaPairing.dll"]
