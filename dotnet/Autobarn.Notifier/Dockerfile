FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# COPY Autobarn.sln .
COPY Autobarn.Notifier/*.csproj ./Autobarn.Notifier/
COPY Autobarn.Messages/*.csproj ./Autobarn.Messages/
RUN dotnet restore Autobarn.Notifier/Autobarn.Notifier.csproj

COPY Autobarn.Notifier/ ./Autobarn.Notifier/
COPY Autobarn.Messages/ ./Autobarn.Messages/
RUN dotnet build -c Release Autobarn.Notifier/Autobarn.Notifier.csproj
RUN dotnet publish Autobarn.Notifier -c Release -o published 

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/published ./
COPY Autobarn.Notifier/resolv.conf /etc/
ENTRYPOINT ["dotnet", "Autobarn.Notifier.dll"]

