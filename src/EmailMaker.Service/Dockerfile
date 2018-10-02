FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY EmailMaker.Service/EmailMaker.Service.csproj EmailMaker.Service/
RUN dotnet restore EmailMaker.Service/EmailMaker.Service.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/EmailMaker.Service
RUN dotnet build EmailMaker.Service.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish EmailMaker.Service.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EmailMaker.Service.dll"]
