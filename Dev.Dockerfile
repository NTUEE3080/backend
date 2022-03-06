FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PitaPairing.csproj", "./"]
RUN dotnet restore "PitaPairing.csproj"
COPY . .
WORKDIR "/src/"
CMD dotnet run --no-launch-profile
