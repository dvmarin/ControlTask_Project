#!/bin/bash
set -e

echo "Iniciando SQL Server..."
/opt/mssql/bin/sqlservr &

echo "Esperando a que SQL Server esté listo..."
until /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" &> /dev/null
do
  sleep 2
done

echo "SQL Server listo"

echo "Ejecutando scripts SQL..."
for file in /scripts/*.sql
do
  echo "Ejecutando $file"
  /opt/mssql-tools/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P "$SA_PASSWORD" \
    -i "$file"
done

echo "Base de datos inicializada correctamente"

wait
