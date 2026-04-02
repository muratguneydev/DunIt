FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY src/DunIt.Core/DunIt.Core.csproj src/DunIt.Core/
COPY src/DunIt.Web/DunIt.Web.csproj src/DunIt.Web/
RUN dotnet restore src/DunIt.Web/DunIt.Web.csproj

COPY src/DunIt.Core/ src/DunIt.Core/
COPY src/DunIt.Web/ src/DunIt.Web/
RUN dotnet publish src/DunIt.Web/DunIt.Web.csproj -c Release -o /publish

FROM nginx:alpine
COPY --from=build /publish/wwwroot /usr/share/nginx/html
EXPOSE 80
