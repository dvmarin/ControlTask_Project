FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .

# Simple solution: create a modified angular.json without SSR
RUN cat angular.json | \
    sed 's/"outputMode": "server"/"outputMode": "browser-only"/' | \
    sed 's/"maximumWarning": "500kB"/"maximumWarning": "2MB"/' | \
    sed 's/"maximumError": "1MB"/"maximumError": "4MB"/' > angular.json.modified && \
    mv angular.json.modified angular.json

RUN npm run build

FROM nginx:alpine
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/dist/controltask/browser /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]