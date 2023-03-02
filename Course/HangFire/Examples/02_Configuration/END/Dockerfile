FROM mcr.microsoft.com/mssql/server:latest

ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=YourStrong!Passw0rd

# Set the user and password for the new user
ENV MSSQL_PID=Developer
ENV MSSQL_USER=tactics
ENV MSSQL_PASSWORD=tactics!password

# Create a new database
ENV MSSQL_DB=Tactics

# Start SQL Server and run the setup script
COPY scripts/setup.sql /var/opt/mssql/setup/

RUN /opt/mssql/bin/sqlservr --accept-eula & sleep 10 \
    && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${SA_PASSWORD} -i /var/opt/mssql/setup/setup.sql \
    && pkill sqlservr

CMD [ "/opt/mssql/bin/sqlservr" ]
