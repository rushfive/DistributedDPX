#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

#FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["Services/Participant/Participant.Api/Participant.Api.csproj", "Services/Participant/Participant.Api/"]
RUN dotnet restore "Services/Participant/Participant.Api/Participant.Api.csproj"
COPY . .
WORKDIR "/src/Services/Participant/Participant.Api"
RUN dotnet build "Participant.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Participant.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Participant.Api.dll"]