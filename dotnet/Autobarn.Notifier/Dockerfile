# To build the Docker image for Autobarn.Notifier,
# run docker build from the SOLUTION DIRECTORY (/dotnet)
# docker build -f Autobarn.Notifier/Dockerfile -t autobarn.notifier .
# 
# Then run the Notifier docker image using:
# docker run autobarn.notifier

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet build
RUN dotnet publish Autobarn.Notifier -c Release -o published

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/published ./
ENTRYPOINT ["dotnet", "Autobarn.Notifier.dll"]