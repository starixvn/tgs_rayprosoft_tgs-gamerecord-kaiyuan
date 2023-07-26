#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.Kaiyuan/Tgs.GameRecord.Kaiyuan.csproj", "Tgs.GameRecord.Kaiyuan/"]
COPY ["Tgs.GameRecord.Kaiyuan.Core/Tgs.GameRecord.Kaiyuan.Core.csproj", "Tgs.GameRecord.Kaiyuan.Core/"]
COPY ["Tgs.GameRecord.Kaiyuan.Common/Tgs.GameRecord.Kaiyuan.Common.csproj", "Tgs.GameRecord.Kaiyuan.Common/"]
RUN dotnet restore "Tgs.GameRecord.Kaiyuan/Tgs.GameRecord.Kaiyuan.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.Kaiyuan"
RUN dotnet build "Tgs.GameRecord.Kaiyuan.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.Kaiyuan.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.Kaiyuan.dll"]