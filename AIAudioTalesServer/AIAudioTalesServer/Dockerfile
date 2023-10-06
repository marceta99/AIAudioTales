#1. this is the first development version image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS moj-build-env
WORKDIR /app
#for http
EXPOSE 80

#this will copy csproj which containes all nuget dependencies that we need 
#in this project and than dotnet restore will download that packages in our project
COPY *.csproj ./
RUN dotnet restore

#copy all source code to app workidrectory
#-c defines build configuration and one option is Release which is used for 
#production ready builds, -o Specifies the path for the output directory.
COPY . ./
RUN dotnet publish -c Release -o out

#2. this is the second final deploy version image where we take only published
#outcome from the first image, because whole first image is to big, and this
#second we want to make it smaller and we only need this published outcome from the first one
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS moj-final-env
WORKDIR /app
COPY --from=moj-build-env /app/out .
ENTRYPOINT [ "dotnet","AIAudioTalesServer.dll"]







