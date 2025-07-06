# 1) Build stage
FROM node:18-alpine AS builder
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci --silent

COPY . .
RUN npm run build -- --configuration production

# 2) Runtime stage
FROM nginx:stable-alpine
LABEL maintainer="kumadiobooks@gmail.com"

RUN rm -rf /usr/share/nginx/html/*
COPY --from=builder /app/www /usr/share/nginx/html

EXPOSE 80
STOPSIGNAL SIGTERM
CMD ["nginx", "-g", "daemon off;"]
