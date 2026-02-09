#!/bin/bash
set -e

export MSYS_NO_PATHCONV=1
export MSYS2_ARG_CONV_EXCL="*"

NETWORK="controltask-network"
SCRIPTS_PATH="$(pwd)/scripts"
SERVER="sqlserver"
USER="sa"
PASSWORD="YourStrong@Passw0rd"
DB="TeamTasksSample"

echo "1. Creando base de datos..."
docker run --rm --network $NETWORK -v "$SCRIPTS_PATH:/scripts" \
  mcr.microsoft.com/mssql-tools \
  /opt/mssql-tools/bin/sqlcmd -S $SERVER -U $USER -P $PASSWORD -i /scripts/01_create_database.sql

echo "2. Creando tablas..."
docker run --rm --network $NETWORK -v "$SCRIPTS_PATH:/scripts" \
  mcr.microsoft.com/mssql-tools \
  /opt/mssql-tools/bin/sqlcmd -S $SERVER -U $USER -P $PASSWORD -d $DB -i /scripts/02_create_tables.sql

echo "3. Insertando datos iniciales..."
docker run --rm --network $NETWORK -v "$SCRIPTS_PATH:/scripts" \
  mcr.microsoft.com/mssql-tools \
  /opt/mssql-tools/bin/sqlcmd -S $SERVER -U $USER -P $PASSWORD -d $DB -i /scripts/03_insert_data.sql

echo "4. Creando procedimientos almacenados..."
docker run --rm --network $NETWORK -v "$SCRIPTS_PATH:/scripts" \
  mcr.microsoft.com/mssql-tools \
  /opt/mssql-tools/bin/sqlcmd -S $SERVER -U $USER -P $PASSWORD -d $DB -i /scripts/04_create_procedures.sql

echo "5. Ejecutando queries de validación..."
docker run --rm --network $NETWORK -v "$SCRIPTS_PATH:/scripts" \
  mcr.microsoft.com/mssql-tools \
  /opt/mssql-tools/bin/sqlcmd -S $SERVER -U $USER -P $PASSWORD -d $DB -i /scripts/05_queries.sql