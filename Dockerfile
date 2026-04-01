FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

COPY DunIt.sln ./
COPY src/DunIt.Web/DunIt.Web.csproj src/DunIt.Web/
COPY tests/DunIt.Testing/DunIt.Testing.csproj tests/DunIt.Testing/
COPY tests/DunIt.UnitTests/DunIt.UnitTests.csproj tests/DunIt.UnitTests/
RUN dotnet restore

COPY . .
EXPOSE 5000
ENTRYPOINT ["dotnet", "watch", "run", "--project", "src/DunIt.Web/DunIt.Web.csproj", "--urls", "http://0.0.0.0:5000"]
