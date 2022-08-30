FROM mcr.microsoft.com/mssql/server:2022-latest

ENV ACCEPT_EULA="Y"
ENV MSSQL_SA_PASSWORD=SqlServerPassword#&%¤2019
ENV SA_PASSWORD=SqlServerPassword#&%¤2019
ENV MSSQL_PID="Express"

ENV PATH=$PATH:/opt/mssql-tools/bin


########################
# Setup Northwind Data #
########################

COPY ./instnwnd.sql /usr/share/instnwnd.sql
RUN (/opt/mssql/bin/sqlservr --accept-eula --reset-sa-password &) \
	&& (/opt/mssql-tools/bin/sqlcmd -l 60 -S 127.0.0.1 -U sa -P "SqlServerPassword#&%¤2019" -i /usr/share/instnwnd.sql) 