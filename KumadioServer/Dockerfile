# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1.a) Kopiraj solution i sve projekte (da bi restore koristio cache)
COPY ["KumadioServer.sln", "./"]
COPY ["Kumadio.Core/Kumadio.Core.csproj", "Kumadio.Core/"]
COPY ["Kumadio.Domain/Kumadio.Domain.csproj", "Kumadio.Domain/"]
COPY ["Kumadio.Infrastructure/Kumadio.Infrastructure.csproj", "Kumadio.Infrastructure/"]
COPY ["Kumadio.Web/Kumadio.Web.csproj", "Kumadio.Web/"]

# 1.b) Restore svih paketa za celo rešenje
RUN dotnet restore "KumadioServer.sln" --no-cache

# 1.c) Kopiraj sav ostatak koda
COPY . .

# 1.d) Publish samo Web projekat
WORKDIR "/src/Kumadio.Web"
RUN dotnet publish -c Release -o /app/publish

# 2) Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Add non-root user
RUN addgroup --system appgroup && adduser --system appuser --ingroup appgroup
USER appuser

COPY --from=build /app/publish ./

# SVE environment varijable koje ti trebaju
ENV ASPNETCORE_ENVIRONMENT=Production \
    ApplicationSettings__Secret="${APP_SECRET}" \
    ApplicationSettings__GoogleClientId="${GOOGLE_CLIENT_ID}" \
    OpenAI__ApiKey="${OPENAI_API_KEY}" \
    SmtpSettings__Username="${SMTP_USERNAME}" \
    SmtpSettings__Password="${SMTP_PASSWORD}" \
    ConnectionStrings__DefaultConnection="Server=db;Database=KumadioDb;User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True;"

HEALTHCHECK --interval=30s --timeout=5s --start-period=30s \
  CMD curl -f http://localhost/health || exit 1

EXPOSE 80
ENTRYPOINT ["dotnet", "Kumadio.Web.dll"]

