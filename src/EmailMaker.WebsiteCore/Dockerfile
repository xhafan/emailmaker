FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY EmailMaker.WebsiteCore/EmailMaker.WebsiteCore.csproj EmailMaker.WebsiteCore/
RUN dotnet restore EmailMaker.WebsiteCore/EmailMaker.WebsiteCore.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/EmailMaker.WebsiteCore
RUN dotnet publish EmailMaker.WebsiteCore.csproj -c Release -o /app

FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app .
COPY EmailMaker.Database /app/EmailMaker.Database
ENTRYPOINT ["dotnet", "EmailMaker.WebsiteCore.dll"]
