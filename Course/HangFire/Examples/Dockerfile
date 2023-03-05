# Use the latest version of the mcr.microsoft.com/mssql/server image
FROM mcr.microsoft.com/mssql/server:latest

USER root

# Set environment variables
ENV ACCEPT_EULA=Y \
    SA_PASSWORD=YourStrong!Passw0rd

# Add Microsoft package repository
RUN wget -qO- https://packages.microsoft.com/keys/microsoft.asc | tee /etc/apt/trusted.gpg.d/microsoft.asc
RUN apt-get update && apt-get install -y software-properties-common
RUN add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/20.04/mssql-server-2022.list)"

# Install optional packages
RUN export DEBIAN_FRONTEND=noninteractive
RUN apt-get install -y mssql-server-ha
RUN apt-get install -y mssql-server-fts

# Cleanup the Dockerfile
RUN apt-get clean
RUN rm -rf /var/lib/apt/lists

# Expose the SQL Server port
EXPOSE 1433

# Start SQL Server
CMD ["/opt/mssql/bin/sqlservr"]