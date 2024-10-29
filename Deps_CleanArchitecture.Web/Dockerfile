# Base Image
FROM mcr.microsoft.com/dotnet/aspnet:6.0.4-focal-amd64 AS base
RUN apt update && apt install --allow-unauthenticated -y tzdata locales curl libgdiplus libc6-dev ca-certificates
ENV TIME_ZONE="America/Sao_Paulo"
ENV TZ="America/Sao_Paulo"
ENV LANG="pt_BR.UTF-8"
ENV LANGUAGE="pt_BR:pt"
ENV LC_ALL="pt_BR.UTF-8"

# JWT Environment Variables
ENV JWT_ISSUER="localhost:5151"
ENV JWT_AUDIENCE="localhost:4200"
ENV JWT_KEY="jfRVHqN7HJExsDokHJRA85tt4QexDeWT"

# PostgreSQL Connection String
ENV POSTGRES_CONNECTION="Server=postgres;Port=5432;Database=Deps_CleanArchitecture;UserId=postgres;Password=postgres;Pooling=true;TimeZone=America/Sao_Paulo"

RUN sed -i -e 's/# pt_BR.UTF-8 UTF-8/pt_BR.UTF-8 UTF-8/' /etc/locale.gen
RUN dpkg-reconfigure --frontend=noninteractive locales
RUN locale-gen
WORKDIR /app
EXPOSE 80

# Build Image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /Deps_CleanArchitecture
COPY ["Deps_CleanArchitecture/Deps_CleanArchitecture.Web/Deps_CleanArchitecture.Web.csproj", "src/Deps_CleanArchitecture.Web/"]
COPY ["Deps_CleanArchitecture/Deps_CleanArchitecture.Infrastructure/Deps_CleanArchitecture.Infrastructure.csproj", "src/Deps_CleanArchitecture.Infrastructure/"]
COPY ["Deps_CleanArchitecture/Deps_CleanArchitecture.SharedKernel/Deps_CleanArchitecture.SharedKernel.csproj", "src/Deps_CleanArchitecture.SharedKernel/"]
COPY ["Deps_CleanArchitecture/Deps_CleanArchitecture.Core/Deps_CleanArchitecture.Core.csproj", "src/Deps_CleanArchitecture.Core/"]
RUN dotnet restore "src/Deps_CleanArchitecture.Web/Deps_CleanArchitecture.Web.csproj"
COPY . .
WORKDIR "/Deps_CleanArchitecture/Deps_CleanArchitecture/Deps_CleanArchitecture.Web"
RUN dotnet build "Deps_CleanArchitecture.Web.csproj" -c Release -o /app/build

# Publish Image
FROM build AS publish
RUN dotnet publish "Deps_CleanArchitecture.Web.csproj" -c Release -o /app/publish

# Final Image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Deps_CleanArchitecture.Web.dll"]
