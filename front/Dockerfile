FROM node:22-alpine AS development
WORKDIR /app
COPY package*.json ./
COPY . .
COPY docker-entrypoint.sh /usr/local/bin/docker-entrypoint
RUN sed -i 's/\r$//' /usr/local/bin/docker-entrypoint && chmod +x /usr/local/bin/docker-entrypoint
EXPOSE 4200
ENTRYPOINT ["docker-entrypoint"]

FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci --no-audit --no-fund
COPY . .
RUN cp src/environments/environment.docker.ts src/environments/environment.ts && npm run build

FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/loja/browser /usr/share/nginx/html
EXPOSE 80
