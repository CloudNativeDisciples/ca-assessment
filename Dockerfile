FROM mcr.microsoft.com/dotnet/sdk:6.0 AS BUILDER

COPY . /app

WORKDIR /app

RUN mkdir "/app/output"
RUN dotnet publish CA.Assessment.WebAPI --configuration Release --self-contained false --output /app/output --runtime linux-x64

FROM mcr.microsoft.com/dotnet/aspnet:6.0

EXPOSE 8090

ENV ASPNETCORE_URLS="http://+:8090"
ENV CA_ASSESSMENT_DATABASE="Data Source = /ca_assessment/data/app.db"
ENV CA_ASSESSMENT_IMAGE_STORE__FOLDER="/ca_assessment/images/"
ENV CA_ASSESSMENT_IMAGE_STORE__OVERWRITE="true"

RUN mkdir --parent "/ca_assessment/images" 
RUN mkdir --parent "/ca_assessment/data" 

WORKDIR /ca_assessment

COPY --from=BUILDER /app/output .

RUN groupadd -r ca_assessment && useradd --no-log-init -r -g ca_assessment ca_assessment
RUN chown -R ca_assessment:ca_assessment /ca_assessment
USER ca_assessment

CMD ["dotnet", "CA.Assessment.WebAPI.dll"]