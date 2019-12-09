FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build-env
WORKDIR //email_service

# Copy main csproj and restore as distinct layers
COPY Amazon.EmailService/*.csproj ./Amazon.EmailService/
COPY Amazon.EmailService/nuget.config ./Amazon.EmailService/
RUN cd Amazon.EmailService && dotnet restore /property:Configuration=Release

# Copy everything else and build
COPY . ./
WORKDIR //email_service/Amazon.EmailService
RUN dotnet publish -c Release -o //email_service/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.1
WORKDIR //email_service
COPY --from=build-env //email_service/out .

ENTRYPOINT ["dotnet", "Amazon.EmailService.dll"]