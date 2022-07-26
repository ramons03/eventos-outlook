#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["eventos-outlook/eventos-outlook.csproj", "eventos-outlook/"]
RUN dotnet restore "eventos-outlook/eventos-outlook.csproj"
COPY . .
WORKDIR "/src/eventos-outlook"
RUN dotnet build "eventos-outlook.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "eventos-outlook.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eventos-outlook.dll"]