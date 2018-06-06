FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY EmailMaker.WebsiteCore/EmailMaker.WebsiteCore.csproj EmailMaker.WebsiteCore/
COPY EmailMaker.Domain.Register.Castle/EmailMaker.Domain.Register.Castle.csproj EmailMaker.Domain.Register.Castle/
COPY EmailMaker.Domain/EmailMaker.Domain.csproj EmailMaker.Domain/
COPY EmailMaker.Dto/EmailMaker.Dtos.csproj EmailMaker.Dto/
COPY EmailMaker.Messages/EmailMaker.Messages.csproj EmailMaker.Messages/
COPY EmailMaker.Core/EmailMaker.Core.csproj EmailMaker.Core/
COPY EmailMaker.Queries.Register.Castle/EmailMaker.Queries.Register.Castle.csproj EmailMaker.Queries.Register.Castle/
COPY EmailMaker.Queries/EmailMaker.Queries.csproj EmailMaker.Queries/
COPY EmailMaker.Commands.Register.Castle/EmailMaker.Commands.Register.Castle.csproj EmailMaker.Commands.Register.Castle/
COPY EmailMaker.Commands/EmailMaker.Commands.csproj EmailMaker.Commands/
COPY EmailMaker.Controllers.Register.Castle/EmailMaker.Controllers.Register.Castle.csproj EmailMaker.Controllers.Register.Castle/
COPY EmailMaker.Controllers/EmailMaker.Controllers.csproj EmailMaker.Controllers/
COPY EmailMaker.Infrastructure.Register.Castle/EmailMaker.Infrastructure.Register.Castle.csproj EmailMaker.Infrastructure.Register.Castle/
COPY EmailMaker.Infrastructure/EmailMaker.Infrastructure.csproj EmailMaker.Infrastructure/
COPY EmailMaker.Domain.NhibernateMapping/EmailMaker.Domain.NhibernateMapping.csproj EmailMaker.Domain.NhibernateMapping/
RUN dotnet restore EmailMaker.WebsiteCore/EmailMaker.WebsiteCore.csproj
COPY . .
WORKDIR /src/EmailMaker.WebsiteCore
RUN dotnet build EmailMaker.WebsiteCore.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish EmailMaker.WebsiteCore.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EmailMaker.WebsiteCore.dll"]
