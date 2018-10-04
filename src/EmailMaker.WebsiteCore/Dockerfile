FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY EmailMaker.WebsiteCore/EmailMaker.WebsiteCore.csproj EmailMaker.WebsiteCore/
RUN dotnet restore EmailMaker.WebsiteCore/EmailMaker.WebsiteCore.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/EmailMaker.WebsiteCore
RUN dotnet build EmailMaker.WebsiteCore.csproj -c Release -o /app
COPY EmailMaker.Database /app/EmailMaker.Database

FROM build AS publish
RUN dotnet publish EmailMaker.WebsiteCore.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EmailMaker.WebsiteCore.dll"]
