#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ScrapingChallenge.csproj", ""]
COPY ["../Infrastructure/ScrapingChallenge.Infrastructure/ScrapingChallenge.Infrastructure.csproj", "../Infrastructure/ScrapingChallenge.Infrastructure/"]
COPY ["../Domain/ScrapingChallenge.Domain/ScrapingChallenge.Domain.csproj", "../Domain/ScrapingChallenge.Domain/"]
COPY ["../Application/ScrapingChallenge.Application/ScrapingChallenge.Application.csproj", "../Application/ScrapingChallenge.Application/"]
RUN dotnet restore "./ScrapingChallenge.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ScrapingChallenge.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScrapingChallenge.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScrapingChallenge.dll"]